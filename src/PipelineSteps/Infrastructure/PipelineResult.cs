using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Infrastructure;
/// <summary>
/// Represents the completion status of the executed workflow.  Property 
/// <see cref="IsSuccess"/> is set to true on class instantiation.
/// </summary>
public class PipelineResult : IPipelineResult
{
    public string WorkflowName { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string ErrorMessage { get; set; }
    public IPipelineStep CurrentStep { get; set; }
    public string Message { get; set; }
    public int StepsCount { get; set; }

    public PipelineResult(string workflowName, int stepCount)
    {
        WorkflowName = workflowName;
        StepsCount = stepCount;
    }

    public PipelineResult(string workflowName, int stepCount, string errorMessage)
    {
        WorkflowName = workflowName;
        ErrorMessage = errorMessage;
        StepsCount = stepCount;
        IsSuccess = false;
    }

    public void AddError(string error)
    {
        ErrorMessage = error;
        IsSuccess = false;
    }
}