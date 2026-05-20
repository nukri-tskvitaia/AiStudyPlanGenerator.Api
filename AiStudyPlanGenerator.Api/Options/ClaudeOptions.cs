using System.ComponentModel.DataAnnotations;

namespace AiStudyPlanGenerator.Api.Options;

public sealed class ClaudeOptions
{
    public const string SectionName = "Claude";

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string Version { get; init; } = "2023-06-01";

    [Required]
    public string Model { get; init; } = "claude-haiku-4-5-20251001";

    [Range(1, 4096)]
    public int MaxTokens { get; init; } = 2000;

    [Range(0, 1)]
    public decimal Temperature { get; init; } = 0.6m;

    [Required]
    [Url]
    public string MessagesEndpointUrl { get; init; } = "https://api.anthropic.com/v1/messages";
}