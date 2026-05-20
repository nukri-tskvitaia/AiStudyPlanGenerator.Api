namespace AiStudyPlanGenerator.Api.Models;

public sealed class StudyPlanResponse
{
    public string Summary { get; set; } = string.Empty;

    public List<StudyPlanStep> Steps { get; set; } = [];
}