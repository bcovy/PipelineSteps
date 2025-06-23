using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Helpers;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;

namespace PipelineSteps.TestHelper;
/// <summary>
/// Base class that provides <see cref="IPipelineController"/> test fixture.
/// </summary>
public abstract class PipelineTestHelper
{
    protected IPipelineController Controller;
    /// <summary>
    /// Performs start up logic on components necessary to create a <see cref="IPipelineController"/>.
    /// </summary>
    protected virtual void Setup()
    {
        //setup dependency injection
        IServiceCollection services = new ServiceCollection();

        services.AddLogging();
        //library setup
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        Controller = serviceProvider.GetService<IPipelineController>();
    }
    /// <summary>
    /// Will run the <see cref="DiConfiguration.AddWorkflowPipeline(IServiceCollection, Action{PipelineOptions})"/> extension 
    /// method to add required WorkflowSteps classes to IoC container.
    /// </summary>
    /// <remarks>
    /// Override this method to register any additional class that may be required to run any Steps.  If overridden, be 
    /// sure to call its base method so required WorkflowSteps classes are registered.
    /// </remarks>
    /// <param name="services">Service collection.</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflowPipeline();
    }

    public async Task<IPipelineResult> StartPipeline<TData>(IPipeline<TData> workflow) where TData : class
    {
        return await Controller.StartPipeline(workflow);
    }
}
