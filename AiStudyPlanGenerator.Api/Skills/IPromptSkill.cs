namespace AiStudyPlanGenerator.Api.Skills;

public interface IPromptSkill
{
    string Name { get; }

    bool CanHandle(string level);

    string GetInstructions();
}