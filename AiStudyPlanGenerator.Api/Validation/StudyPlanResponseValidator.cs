using AiStudyPlanGenerator.Api.Models;

namespace AiStudyPlanGenerator.Api.Validation;

public static class StudyPlanResponseValidator
{
    public static bool IsValid(
        StudyPlanResponse response,
        int expectedDays,
        out string error)
    {
        if (string.IsNullOrWhiteSpace(response.Summary))
        {
            error = "Summary is required.";
            return false;
        }

        if (response.Steps.Count != expectedDays)
        {
            error = $"Expected {expectedDays} steps, but got {response.Steps.Count}.";
            return false;
        }

        for (var i = 0; i < response.Steps.Count; i++)
        {
            var step = response.Steps[i];

            if (step.Day != i + 1)
            {
                error = $"Expected day {i + 1}, but got day {step.Day}.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(step.Topic))
            {
                error = $"Topic is required for day {step.Day}.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(step.Task))
            {
                error = $"Task is required for day {step.Day}.";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }
}