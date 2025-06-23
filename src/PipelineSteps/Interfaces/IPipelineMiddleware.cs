using PipelineSteps.Enums;
using PipelineSteps.Middleware;

namespace PipelineSteps.Interfaces;

/// <summary>
/// Middleware that can be set up to run pre- or post-workflow execution, or in the 
/// event of an in process error.
/// </summary>
public interface IPipelineMiddleware
{
    /// <summary>
    /// Phase for which item should run.
    /// </summary>
    MiddlewarePhase Phase { get; }
    /// <summary>
    /// Runs the middleware item.
    /// </summary>
    /// <param name="context">Workflow result context.</param>
    /// <param name="next">Next middleware item in the chain.</param>
    /// <returns>A <see cref="Task"/> that completes asynchronously once the middleware chain finishes running.</returns>
    Task InvokeAsync(IPipelineResult context, NextMiddleware next);
}