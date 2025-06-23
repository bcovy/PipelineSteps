namespace PipelineSteps.Interfaces;

public interface IStepBody
{
    string Name { get; }
    Task<IStepResult> RunAsync();
}