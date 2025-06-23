using PipelineSteps.Definitions;
using PipelineSteps.Enums;
using PipelineSteps.Events;

namespace PipelineSteps.Interfaces.Internals;

public interface IEventPersistence
{
    bool PersistStepEventsActive { get; }
    Queue<StepEvent> Events { get; }
    void AddEvent<TData>(PipelineExecutionContext context, int stepId, string bodyName, string stepName, StepResultType stepResult, TData data) where TData : class;
    Task PersistEvents();
}