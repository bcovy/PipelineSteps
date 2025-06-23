using PipelineSteps.Definitions;

namespace PipelineSteps.Interfaces.Internals;

public interface IStepExecutor
{
    bool PersistStepEventsActive { get; }
    Task PersistEvents();
    Task<IStepResult> ExecuteStep<TData>(IPipelineStep step, TData data, PipelineExecutionContext context) where TData : class;
}
