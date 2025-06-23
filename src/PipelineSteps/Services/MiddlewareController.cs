using Microsoft.Extensions.Logging;
using PipelineSteps.Enums;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Middleware;

namespace PipelineSteps.Services;

public class MiddlewareController(IEnumerable<IPipelineMiddleware> middlewares, IEnumerable<IStepMiddleware> stepMiddlewares, ILogger<MiddlewareController> logger) : IMiddlewareController
{
    private static readonly NextMiddleware CompletePipeline = () => Task.CompletedTask;

    public async Task RunStepPipeline(IPipelineStep workflowStep, string stepBodyName, IStepResult result)
    {
        if (stepMiddlewares.Any())
        {
            try
            {
                await stepMiddlewares
                    .Reverse()
                    .Aggregate(CompletePipeline, (previous, middleware) => () => middleware.InvokeAsync(workflowStep, stepBodyName, result, previous))();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MiddlewareController error: {Message}", ex.Message);
            }
        }
    }

    public async Task RunPreWorkflowPipeline(IPipelineResult context)
    {
        var pipeline = middlewares.Where(x => x.Phase == MiddlewarePhase.PreWorkflow).ToList();

        if (pipeline.Count != 0)
            await RunPipeline(pipeline, context);
    }

    public async Task RunPostWorkflowPipeline(IPipelineResult context)
    {
        var pipeline = middlewares.Where(x => x.Phase == MiddlewarePhase.PostWorkflow).ToList();

        if (pipeline.Count != 0)
            await RunPipeline(pipeline, context);
    }

    public async Task RunErrorPipeline(IPipelineResult context)
    {
        var pipeline = middlewares.Where(x => x.Phase == MiddlewarePhase.Error).ToList();

        if (pipeline.Count != 0)
            await RunPipeline(pipeline, context);
    }

    public Task RunPipeline(IEnumerable<IPipelineMiddleware> pipeline, IPipelineResult context)
    {
        try
        {
            return pipeline
                .Reverse()
                .Aggregate(CompletePipeline, (previous, middleware) => () => middleware.InvokeAsync(context, previous))();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MiddlewareController error: {Message}", ex.Message);
        }

        return Task.CompletedTask;
    }
}