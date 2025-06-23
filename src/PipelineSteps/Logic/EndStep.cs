using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Logic;

public class EndStep : IStepBody
{
    public string Name => "End step";

    public async Task<IStepResult> RunAsync()
    {
        return await Task.FromResult(StepResult.EndWorkflow());
    }
}