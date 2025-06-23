using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Logic;

public class Decide : IStepBody
{
    public string Name { get; set; } = "Decide branch logic step";
    public object? OutcomeValue { get; set; } = null;

    public async Task<IStepResult> RunAsync()
    {
        await Task.CompletedTask;

        return StepResult.Branch(OutcomeValue);
    }
}