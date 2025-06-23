using PipelineSteps.Definitions;
using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Tests.Fixtures;

public class StepExecutorFixture : IStepExecutor
{
    public List<int> CallOrder { get; set; }
    public bool UsesStepMiddleware { get; set; }
    public bool PersistStepEventsActive { get; init; }
    public string WorkflowName { get; set; }
    public Guid ContextId { get; set; }

    public StepExecutorFixture()
    {
        CallOrder = new List<int>();
    }

    public async Task PersistEvents()
    {
        await Task.CompletedTask;
    }

    public async Task<IStepResult> ExecuteStep<TData>(IPipelineStep step, TData data, PipelineExecutionContext context) where TData : class
    {
        await Task.CompletedTask;

        CallOrder.Add(step.Id);

        return step.BodyName switch
        {
            "if" => StepResult.IfCondition(),
            "branch" => StepResult.Branch(1),
            "fail" => StepResult.Fail("some error"),
            "end" => StepResult.EndWorkflow(),
            "event" => StepResult.Event("hello", "world", "helloWorld"),
            _ => StepResult.Next(),
        };
    }
}
