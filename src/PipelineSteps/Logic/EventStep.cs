using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Logic;

public class EventStep : IStepBody
{
    public string Name => "Publish Event step";
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public object? OutcomeValue { get; set; }

    public async Task<IStepResult> RunAsync()
    {
        await Task.CompletedTask;

        return StepResult.Event(Reference, Description, OutcomeValue);
    }
}