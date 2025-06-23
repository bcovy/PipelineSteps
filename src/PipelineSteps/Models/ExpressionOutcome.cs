using System.Linq.Expressions;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Models;

public class ExpressionOutcome<TData>(IBranchDefinition definition, Expression<Func<TData, object, bool>> expression) : IBranchStep where TData : class
{
    private readonly Func<TData, object, bool> _func = expression.Compile();

    public IBranchDefinition Definition { get; } = definition;

    public bool Matches(object outcomeValue, object data)
    {
        return _func((TData)data, outcomeValue);
    }
}