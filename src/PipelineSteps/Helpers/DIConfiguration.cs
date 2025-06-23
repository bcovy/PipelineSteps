using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;
using PipelineSteps.Services;

namespace PipelineSteps.Helpers;

public static class DiConfiguration
{
    /// <summary>
    /// Helper method to add required WorkflowSteps classes to IoC container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="setup">Action delegate of workflow options/settings.</param>
    /// <returns><see cref="IServiceCollection"/> to allow user to chain service methods.</returns>
    public static IServiceCollection AddWorkflowPipeline(this IServiceCollection services, Action<PipelineOptions>? setup = null)
    {
        PipelineOptions options = new();
        setup?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IEventPersistence, EventPersistence>();
        services.AddScoped<IPipelineController, PipelineController>();
        services.AddScoped<IMiddlewareController, MiddlewareController>();
        services.AddScoped<IStepExecutor, StepExecutor>();

        return services;
    }
    /// <summary>
    /// Adds a middleware item to be executed in a pipeline.  Specify the phase of the execution process
    /// to trigger the pipeline using <see cref="IPipelineMiddleware.Phase"/>.
    /// </summary>
    /// <remarks>
    /// Middleware will be run in the order it was added to the <paramref name="services"/> collection.
    /// </remarks>
    /// <typeparam name="TIMiddleware">Middleware class type.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns><see cref="IServiceCollection"/> to allow user to chain service methods.</returns>
    public static IServiceCollection AddWorkflowMiddleware<TIMiddleware>(this IServiceCollection services) where TIMiddleware : class, IPipelineMiddleware
    { 
        services.AddTransient<IPipelineMiddleware, TIMiddleware>();

        return services;
    }
    /// <summary>
    /// Adds a middleware item to be executed in a pipeline, after a step as executed with no errors.
    /// </summary>
    /// <remarks>
    /// Middleware will be run in the order it was added to the <paramref name="services"/> collection.
    /// </remarks>
    /// <typeparam name="TIMiddleware">Middleware class type.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns><see cref="IServiceCollection"/> to allow user to chain service methods.</returns>
    public static IServiceCollection AddStepMiddleware<TIMiddleware>(this IServiceCollection services) where TIMiddleware : class, IStepMiddleware
    {
        services.AddTransient<IStepMiddleware, TIMiddleware>();

        return services;
    }
}