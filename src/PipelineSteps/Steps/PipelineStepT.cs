using System.Linq.Expressions;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Steps;

public class PipelineStep<TStepBody, T> : IPipelineStep where TStepBody : IStepBody
{
    public LambdaExpression InputExpression { get; init; }
    public T InputValue { get; init; }
    public Type BodyType { get; set; }
    public string BodyName { get; set; }
    public int Id { get; set; }
    public int NextStepId { get; set; }
    public bool RetryOnFailure { get; set; }
    public TimeSpan RetryInterval { get; set; }
    public IEnumerable<int> ChildStepsId { get; set; }
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
        if (serviceProvider.GetService(BodyType) is IStepBody body)
        {
            AssignValue(body, InputExpression, InputValue);
            return body;
        }
        //Step was not registered or found in the service container; see if target has parameterless constructor.
        var stepCtor = BodyType.GetConstructor([]);

        if (stepCtor == null || stepCtor.Invoke(null) is not IStepBody body2) return null;
        AssignValue(body2, InputExpression, InputValue);

        return body2;
    }

    public void AssignValue(IStepBody body, LambdaExpression expression, T value)
    {
        var valueExpr = Expression.Convert(Expression.Constant(value, typeof(T)), expression.ReturnType);
        var assignment = Expression.Assign(expression.Body, valueExpr);
        var assign = Expression.Lambda(assignment, expression.Parameters.Single());

        assign.Compile().DynamicInvoke(body);
    }
}