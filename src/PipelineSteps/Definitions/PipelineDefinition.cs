using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Definitions;

public class PipelineDefinition<TData> : IPipelineDefinition<TData> where TData : class
{
    public string Name { get; set; }
    public Guid ContextId { get; }
    public int StepsCount { get; private set; }
    public TData Data { get; set; }
    public bool UseErrorMiddleware { get; set; }
    public bool UsePreWorkflowMiddleware { get; set; }
    public bool UsePostWorkflowMiddleware { get; set; }
    public bool UseStepMiddleware { get; set; }
    public bool OptOutOfPersistEvents { get; set; }
    public Dictionary<int, IPipelineStep> Steps { get; set; }

    public PipelineDefinition(TData data, string name, Dictionary<int, IPipelineStep> steps,
        bool useErrorMiddleware = false,
        bool usePreWorkflowMiddleware = false,
        bool usePostWorkflowMiddleware = false,
        bool useStepMiddleware = false, 
        bool optOutOfPersistEvents = false)
    {
        ContextId = Guid.NewGuid();
        Data = data;
        Name = name;
        Steps = steps;
        StepsCount = steps.Count;
        UseErrorMiddleware = useErrorMiddleware;
        UsePreWorkflowMiddleware = usePreWorkflowMiddleware;
        UsePostWorkflowMiddleware = usePostWorkflowMiddleware;
        UseStepMiddleware = useStepMiddleware;
        OptOutOfPersistEvents = optOutOfPersistEvents;
    }
}