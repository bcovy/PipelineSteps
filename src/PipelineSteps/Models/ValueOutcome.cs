using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Models;

public class ValueOutcome<TValue>(IBranchDefinition definition, TValue value) : IBranchStep where TValue : struct
{
    public IBranchDefinition Definition { get; } = definition;
    public TValue Value { get; } = value;

    public bool Matches(object outcomeValue, object data)
    {
        return Equals(outcomeValue, Value);
    }
}