using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Logic;

public class IfStep : IStepBody
{
    public string Name { get; set; } = "If logic step";
    public bool Condition { get; set; }

    public async Task<IStepResult> RunAsync()
    {
        await Task.CompletedTask;

        return Condition ? StepResult.IfCondition() : StepResult.Next();
    }
}