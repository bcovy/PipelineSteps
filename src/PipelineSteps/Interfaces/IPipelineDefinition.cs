using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Interfaces;

/// <summary>
/// Contains the workflow steps, settings, and attributes that define a single workflow.
/// </summary>
/// <typeparam name="TData">Data model type.</typeparam>
public interface IPipelineDefinition<out TData> where TData : class
{
    string Name { get; }
    Guid ContextId { get; }
    int StepsCount { get; }
    TData Data { get; }
    bool UseErrorMiddleware { get; set; }
    bool UsePreWorkflowMiddleware { get; set; }
    bool UsePostWorkflowMiddleware { get; set; }
    bool UseStepMiddleware { get; set; }
    bool OptOutOfPersistEvents { get; set; }
    Dictionary<int, IPipelineStep> Steps { get; }
}