namespace PipelineSteps.Interfaces.Internals;

public interface IBranchStep
{
    IBranchDefinition Definition { get; }
    bool Matches(object outcomeValue, object data);
}