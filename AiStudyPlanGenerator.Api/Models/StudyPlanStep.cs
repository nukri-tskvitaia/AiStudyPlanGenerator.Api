namespace AiStudyPlanGenerator.Api.Models;

public sealed class StudyPlanStep
{
    public int Day { get; set; }

    public string Topic { get; set; } = string.Empty;

    public string Task { get; set; } = string.Empty;
}