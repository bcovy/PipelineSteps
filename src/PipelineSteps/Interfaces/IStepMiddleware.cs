using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Middleware;

namespace PipelineSteps.Interfaces;

public interface IStepMiddleware
{
    Task InvokeAsync(IPipelineStep workflowStep, string stepBodyName, IStepResult result, NextMiddleware next);
}