using PipelineSteps.Enums;
using PipelineSteps.Interfaces;
using PipelineSteps.Middleware;

namespace PipelineSteps.Integration.Mocks;

public class MiddleError1 : IPipelineMiddleware
{
    public MiddlewarePhase Phase { get; set; } = MiddlewarePhase.Error;

    public async Task InvokeAsync(IPipelineResult context, NextMiddleware next)
    {
        context.Message += "Before_" + nameof(MiddleError1);

        await next();

        context.Message += "After_" + nameof(MiddleError1);
    }
}