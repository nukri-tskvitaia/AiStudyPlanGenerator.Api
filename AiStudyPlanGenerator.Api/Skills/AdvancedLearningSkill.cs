namespace AiStudyPlanGenerator.Api.Skills;

public sealed class AdvancedLearningSkill : IPromptSkill
{
    public string Name => "Advanced Learning Skill";

    public bool CanHandle(string level)
    {
        return string.Equals(level, "Advanced", StringComparison.OrdinalIgnoreCase);
    }

    public string GetInstructions()
    {
        return """
        The learner is advanced.

        Instructions:
        - Focus on architecture, trade-offs, and deeper reasoning.
        - Include realistic project-style tasks.
        - Avoid explaining basic terminology unless necessary.
        - Add debugging, optimization, and best-practice exercises.
        - Encourage comparison between alternative approaches.
        """;
    }
}