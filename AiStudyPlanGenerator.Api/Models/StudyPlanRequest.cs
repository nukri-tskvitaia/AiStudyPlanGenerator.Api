using System.ComponentModel.DataAnnotations;

namespace AiStudyPlanGenerator.Api.Models;

public sealed class StudyPlanRequest
{
    [Required]
    [MinLength(3)]
    public string Goal { get; set; } = string.Empty;

    [Range(1, 30)]
    public int Days { get; set; } = 7;

    [Required]
    public string Level { get; set; } = "Beginner";
}