using PipelineSteps.Enums;

namespace PipelineSteps.Events;

public class LifeCycleEvent
{
    public int StepId { get; set; }
    public StepResultType Result { get; set; }
    public DateTime EventTimeUtc { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public object? OutcomeValue { get; set; }
}