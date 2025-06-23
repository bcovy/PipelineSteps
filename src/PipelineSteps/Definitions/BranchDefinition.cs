using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Definitions;
/// <summary>
/// Represents the definition of a workflow branch, including its name, unique identifier, steps,
/// and the number of steps it contains.
/// </summary>
/// <param name="name">Branch name.</param>
/// <param name="steps">Steps contained in the branch.</param>
public class BranchDefinition(string name, Dictionary<int, IPipelineStep> steps) : IBranchDefinition
{
    public string Name { get; } = name;
    public int Id { get; } = BranchIdGenerator.NewId;
    public Dictionary<int, IPipelineStep> Steps { get; } = steps;
    public int StepsCount { get; } = steps.Count;

    public static BranchDefinition Create(string name, Dictionary<int, IPipelineStep> steps) => new(name, steps);
}