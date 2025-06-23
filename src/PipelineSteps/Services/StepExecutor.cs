using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipelineSteps.Definitions;
using PipelineSteps.Enums;
using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Services;

public class StepExecutor : IStepExecutor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMiddlewareController _middleware;
    private readonly IEventPersistence _eventPersistence;
    private readonly ILogger<StepExecutor> _logger;

    public bool PersistStepEventsActive { get; private set; }

    public StepExecutor(IServiceScopeFactory scopeFactory, IMiddlewareController middleware, IEventPersistence eventPersistence, ILogger<StepExecutor> logger)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _middleware = middleware;
        _eventPersistence = eventPersistence;
        PersistStepEventsActive = _eventPersistence.PersistStepEventsActive;
    }

    public async Task PersistEvents() => await _eventPersistence.PersistEvents();

    public async Task<IStepResult> ExecuteStep<TData>(IPipelineStep step, TData data, PipelineExecutionContext context) where TData : class
    {
        using var scope = _scopeFactory.CreateScope();
        var body = step.ConstructBody(scope.ServiceProvider);

        if (body == null)
            return StepResult.Fail($"Unable to construct step {step.BodyName}, ID {step.Id}");

        foreach (var input in step.Inputs)
            input.AssignInput(data, body);

        IStepResult result = await body.RunAsync();

        if (result.Result == StepResultType.Failure)
        {
            _logger.LogError("Error at step {BodyName}: {Name}, step ID {Id}", step.BodyName, body.Name, step.Id);

            if (step.RetryOnFailure)
            {
                _logger.LogError("Attempting to re-run step {BodyName}: {Name}, step ID {Id} after {TotalMinutes} minutes", step.BodyName, body.Name, step.Id, step.RetryInterval.TotalMinutes);

                await Task.Delay(step.RetryInterval);

                result = await body.RunAsync();
            }
        }
        else if (result.Result != StepResultType.Failure)
        {
            foreach (var output in step.Outputs)
                output.AssignOutput(data, body);
        }

        if (context.UsesStepMiddleware)
            await _middleware.RunStepPipeline(step, body.Name, result);

        if (_eventPersistence.PersistStepEventsActive)
            _eventPersistence.AddEvent(context, step.Id, step.BodyName, body.Name, result.Result, data);

        return result;
    }
}
