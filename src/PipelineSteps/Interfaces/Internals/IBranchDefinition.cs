namespace PipelineSteps.Interfaces.Internals;

public interface IBranchDefinition
{
    /// <summary>
    /// Branch name. 
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Branch ID.
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Count of steps in branch.
    /// </summary>
    int StepsCount { get; }
    /// <summary>
    /// Collection of steps in branch.
    /// </summary>
    Dictionary<int, IPipelineStep> Steps { get; }
}