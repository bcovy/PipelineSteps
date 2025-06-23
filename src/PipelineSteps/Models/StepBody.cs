using PipelineSteps.Interfaces;

namespace PipelineSteps.Models;

/// <summary>
/// Abstract step class used to wrap a synchronous method in an asynchronous task.
/// </summary>
public abstract class StepBody : IStepBody
{
    public string Name { get; set; }

    public abstract IStepResult Run();

    public async Task<IStepResult> RunAsync()
    {
        return await Task.FromResult(Run());
    }
}