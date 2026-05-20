namespace AiStudyPlanGenerator.Api.Skills;

public sealed class BeginnerLearningSkill : IPromptSkill
{
    public string Name => "Beginner Learning Skill";

    public bool CanHandle(string level)
    {
        return string.Equals(level, "Beginner", StringComparison.OrdinalIgnoreCase);
    }

    public string GetInstructions()
    {
        return """
        The learner is a beginner.

        Instructions:
        - Explain topics simply.
        - Avoid unnecessary jargon.
        - Include small practical tasks.
        - Build from fundamentals before advanced concepts.
        - Keep each day realistic and not overloaded.
        """;
    }
}