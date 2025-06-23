using PipelineSteps.Enums;

namespace PipelineSteps.Events;

public class StepEvent
{
    public string WorkflowName { get; set; }
    public Guid ContextId { get; set; }
    public int StepId { get; set; }
    public StepResultType Result { get; set; }
    public DateTime EventTimeUtc { get; set; }
    public string BodyName { get; set; }
    public string StepName { get; set; }
    public int IsBranch { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public string DataJson { get; set; }
}