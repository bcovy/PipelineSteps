using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Steps;
/// <summary>
/// Class that encapsulates a step in the pipeline, along with its settings.
/// </summary>
/// <typeparam name="TStepBody">Step body type.</typeparam>
public class PipelineStep<TStepBody> : IPipelineStep where TStepBody : IStepBody
{
    public Type BodyType { get; set; }
    public string BodyName { get; set; }
    public int Id { get; set; }
    public int NextStepId { get; set; }
    public bool RetryOnFailure { get; set; }
    public TimeSpan RetryInterval { get; set; }
    public IEnumerable<int> ChildStepsId { get; set; } = [];
    public List<IStepParameter> Inputs { get; set; } = [];
    public List<IStepParameter> Outputs { get; set; } = [];
    public List<IBranchStep> StepBranches { get; set; } = [];

    public PipelineStep()
    {
        BodyType = typeof(TStepBody);
        BodyName = BodyType.Name;
    }

    public IStepBody? ConstructBody(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(BodyType) is IStepBody body) return body;
        //Step was not registered or found in the service container; see if target has parameterless constructor.
        var stepCtor = BodyType.GetConstructor([]);

        if (stepCtor != null)
            return stepCtor.Invoke(null) as IStepBody ?? null;

        return null;
    }
}