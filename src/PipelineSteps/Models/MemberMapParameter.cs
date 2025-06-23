using System.Linq.Expressions;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Models;

public class MemberMapParameter : IStepParameter
{
    private readonly LambdaExpression _source;
    private readonly LambdaExpression _target;

    public MemberMapParameter(LambdaExpression source, LambdaExpression target)
    {
        if (target.Body.NodeType != ExpressionType.MemberAccess)
            throw new NotSupportedException();

        _source = source;
        _target = target;
    }

    private static void Assign(object sourceObject, LambdaExpression sourceExpr, object targetObject, LambdaExpression targetExpr)
    {
        object? resolvedValue = sourceExpr.Compile().DynamicInvoke(sourceObject);

        if (resolvedValue == null)
        {
            var defaultAssign = Expression.Lambda(Expression.Assign(targetExpr.Body, Expression.Default(targetExpr.ReturnType)), targetExpr.Parameters.Single());

            defaultAssign.Compile().DynamicInvoke(targetObject);

            return;
        }

        var valueExpr = Expression.Convert(Expression.Constant(resolvedValue), targetExpr.ReturnType);
        var assign = Expression.Lambda(Expression.Assign(targetExpr.Body, valueExpr), targetExpr.Parameters.Single());
        assign.Compile().DynamicInvoke(targetObject);
    }

    public void AssignInput(object data, IStepBody body)
    {
        Assign(data, _source, body, _target);
    }

    public void AssignOutput(object data, IStepBody body)
    {
        Assign(body, _source, data, _target);
    }
}