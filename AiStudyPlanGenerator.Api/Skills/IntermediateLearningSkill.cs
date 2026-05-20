namespace AiStudyPlanGenerator.Api.Skills;

public sealed class IntermediateLearningSkill : IPromptSkill
{
    public string Name => "Intermediate Learning Skill";

    public bool CanHandle(string level)
    {
        return string.Equals(level, "Intermediate", StringComparison.OrdinalIgnoreCase);
    }

    public string GetInstructions()
    {
        return """
        The learner has some existing knowledge.

        Instructions:
        - Avoid overexplaining basics.
        - Focus on practical application.
        - Include small implementation exercises.
        - Add review and self-check tasks.
        - Keep the plan challenging but realistic.
        """;
    }
}