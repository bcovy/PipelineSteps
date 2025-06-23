namespace PipelineSteps.Definitions;

public class PipelineExecutionContext(string workflowName, Guid contextId, bool usesStepMiddleware = false)
{
    public string WorkflowName { get; private set; } = workflowName;
    public Guid ContextId { get; private set; } = contextId;
    public bool IsBranch { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool UsesStepMiddleware { get; set; } = usesStepMiddleware;
}
