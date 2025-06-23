using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Logic;

namespace PipelineSteps.Steps;

public class PipelineInlineStep<TData>(Func<TData, IStepResult> body, TData data) : IPipelineStep where TData : class
{
    public Func<TData, IStepResult> Body { get; init; } = body;
    public TData Data { get; init; } = data;
    public Type BodyType { get; set; } = typeof(Logic.InlineStep<TData>);
    public string BodyName { get; set; } = "InlineStep";
    public int Id { get; set; }
    public int NextStepId { get; set; }
    public bool RetryOnFailure { get; set; }
    public TimeSpan RetryInterval { get; set; }
    public IEnumerable<int> ChildStepsId { get; set; }
    public List<IStepParameter> Inputs { get; set; } = [];
    public List<IStepParameter> Outputs { get; set; } = [];
    public List<IBranchStep> StepBranches { get; set; } = [];

    public IStepBody ConstructBody(IServiceProvider serviceProvider)
    {
        return new InlineStep<TData>(Body, Data);
    }
}