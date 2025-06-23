using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Interfaces;

public interface IPipelineResult
{
    string WorkflowName { get; }
    bool IsSuccess { get; }
    string ErrorMessage { get; }
    IPipelineStep CurrentStep { get; }
    string Message { get; set; }
    int StepsCount { get; set; }
}