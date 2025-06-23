using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Interfaces;

public interface IMiddlewareController
{
    Task RunStepPipeline(IPipelineStep workflowStep, string stepBodyName, IStepResult result);
    Task RunPreWorkflowPipeline(IPipelineResult context);
    Task RunPostWorkflowPipeline(IPipelineResult context);
    Task RunErrorPipeline(IPipelineResult context);
}