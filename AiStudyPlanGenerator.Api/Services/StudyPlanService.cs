using System.Text;
using System.Text.Json;
using AiStudyPlanGenerator.Api.Exceptions;
using AiStudyPlanGenerator.Api.Models;
using AiStudyPlanGenerator.Api.Options;
using AiStudyPlanGenerator.Api.Skills;
using AiStudyPlanGenerator.Api.Validation;
using Microsoft.Extensions.Options;

namespace AiStudyPlanGenerator.Api.Services;

public sealed class StudyPlanService(
    HttpClient httpClient,
    IOptions<ClaudeOptions> options,
    IEnumerable<IPromptSkill> skills)
{
    private const string PlannerToolName = "create_study_plan_outline";
    private const string GeneratorToolName = "generate_study_plan";

    private readonly ClaudeOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<StudyPlanResponse> GenerateAsync(
        StudyPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var skill = SelectSkill(request.Level);

        var plannerResponse = await CreatePlanOutlineAsync(
            request,
            cancellationToken);

        var finalPlan = await GenerateFinalPlanAsync(
            request,
            plannerResponse,
            skill,
            cancellationToken);

        if (!StudyPlanResponseValidator.IsValid(finalPlan, request.Days, out var validationError))
            throw new InvalidOperationException($"Invalid study plan response: {validationError}");

        return finalPlan;
    }

    private IPromptSkill SelectSkill(string level)
    {
        return skills.FirstOrDefault(skill => skill.CanHandle(level))
               ?? throw new InvalidOperationException($"No prompt skill registered for level: {level}");
    }

    private async Task<PlannerResponse> CreatePlanOutlineAsync(
        StudyPlanRequest request,
        CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            temperature = _options.Temperature,
            system = """
            You are a planning assistant.

            Your job is to create a structured topic outline before the final study plan is generated.

            Rules:
            - Return only high-level study topics.
            - Number of topics should match the requested number of days.
            - Topics should be ordered from fundamentals to more advanced material.
            - Do not generate the final daily tasks yet.
            """,
            tools = new[]
            {
                new
                {
                    name = PlannerToolName,
                    description = "Creates an ordered topic outline for a study plan.",
                    input_schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            topics = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string"
                                }
                            }
                        },
                        required = new[] { "topics" }
                    }
                }
            },
            tool_choice = new
            {
                type = "tool",
                name = PlannerToolName
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"""
                    Goal:
                    {request.Goal}

                    Level:
                    {request.Level}

                    Days:
                    {request.Days}
                    """
                }
            }
        };

        var responseText = await SendClaudeRequestAsync(requestBody, cancellationToken);

        return ExtractToolInput<PlannerResponse>(
            responseText,
            PlannerToolName);
    }

    private async Task<StudyPlanResponse> GenerateFinalPlanAsync(
        StudyPlanRequest request,
        PlannerResponse plannerResponse,
        IPromptSkill skill,
        CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            temperature = _options.Temperature,
            system = $"""
            You are an expert study plan generator.

            First, follow this reusable learning skill:

            Skill name:
            {skill.Name}

            Skill instructions:
            {skill.GetInstructions()}

            Your job is to turn the provided outline into a final daily study plan.

            Rules:
            - Create exactly one step per day.
            - Day numbers must start at 1.
            - Keep tasks practical and specific.
            - Do not add more days than requested.
            - Do not ignore the provided outline.
            """,
            tools = new[]
            {
                new
                {
                    name = GeneratorToolName,
                    description = "Generates a final daily study plan.",
                    input_schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            summary = new
                            {
                                type = "string"
                            },
                            steps = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        day = new
                                        {
                                            type = "integer"
                                        },
                                        topic = new
                                        {
                                            type = "string"
                                        },
                                        task = new
                                        {
                                            type = "string"
                                        }
                                    },
                                    required = new[]
                                    {
                                        "day",
                                        "topic",
                                        "task"
                                    }
                                }
                            }
                        },
                        required = new[]
                        {
                            "summary",
                            "steps"
                        }
                    }
                }
            },
            tool_choice = new
            {
                type = "tool",
                name = GeneratorToolName
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"""
                    Goal:
                    {request.Goal}

                    Level:
                    {request.Level}

                    Days:
                    {request.Days}

                    Planner outline:
                    {JsonSerializer.Serialize(plannerResponse, JsonOptions)}
                    """
                }
            }
        };

        var responseText = await SendClaudeRequestAsync(requestBody, cancellationToken);

        return ExtractToolInput<StudyPlanResponse>(
            responseText,
            GeneratorToolName);
    }

    private async Task<string> SendClaudeRequestAsync(
        object requestBody,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            _options.MessagesEndpointUrl);

        request.Headers.Add("x-api-key", _options.ApiKey);
        request.Headers.Add("anthropic-version", _options.Version);

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(request, cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new AnthropicException((int)response.StatusCode, responseText);

        return responseText;
    }

    private static T ExtractToolInput<T>(
        string responseText,
        string toolName)
    {
        using var document = JsonDocument.Parse(responseText);

        var content = document.RootElement.GetProperty("content");

        foreach (var block in content.EnumerateArray())
        {
            if (!block.TryGetProperty("type", out var typeProperty))
                continue;

            if (!string.Equals(typeProperty.GetString(), "tool_use", StringComparison.OrdinalIgnoreCase))
                continue;

            if (!block.TryGetProperty("name", out var nameProperty))
                continue;

            if (!string.Equals(nameProperty.GetString(), toolName, StringComparison.Ordinal))
                continue;

            var input = block.GetProperty("input");

            return input.Deserialize<T>(JsonOptions)
                   ?? throw new InvalidOperationException($"Failed to deserialize tool input for tool: {toolName}");
        }

        throw new InvalidOperationException($"Claude did not return expected tool_use block: {toolName}");
    }
}