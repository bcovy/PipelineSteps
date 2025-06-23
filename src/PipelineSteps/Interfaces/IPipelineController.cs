using PipelineSteps.Events;

namespace PipelineSteps.Interfaces;

public interface IPipelineController
{
    /// <summary>
    /// Event handler delegate associated with <see cref="Logic.EventStep"/> step.  
    /// Subscribe to event to receive notifications raised by the <see cref="Logic.EventStep"/>.
    /// </summary>
    event StepEventHandlerAsync StepEventAsync;
    /// <summary>
    /// Runs the workflow pipeline in the <paramref name="pipeline"/> parameter.
    /// </summary>
    /// <typeparam name="TData">Data model type.</typeparam>
    /// <param name="pipeline">Finalized pipeline.</param>
    /// <returns>Result of the pipeline steps.</returns>
    Task<IPipelineResult> StartPipeline<TData>(IPipeline<TData> pipeline) where TData : class;
}