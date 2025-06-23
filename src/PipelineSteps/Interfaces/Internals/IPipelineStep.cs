namespace PipelineSteps.Interfaces.Internals;

public interface IPipelineStep
{
    Type BodyType { get; set; }
    string BodyName { get; set; }
    int Id { get; set; }
    int NextStepId { get; set; }
    bool RetryOnFailure { get; set; }
    TimeSpan RetryInterval { get; set; }
    IEnumerable<int> ChildStepsId { get; set; }
    List<IStepParameter> Inputs { get; set; }
    List<IStepParameter> Outputs { get; set; }
    List<IBranchStep> StepBranches { get; set; }
    /// <summary>
    /// Returns an instance of the step body from the <paramref name="serviceProvider"/>, or <see langword="null"/>
    /// if no matching instance can be found. 
    /// </summary>
    /// <remarks>
    /// If step was not registered or found in the service container, method will search for a parameterless
    /// constructor to instantiate.
    /// </remarks>   
    /// <param name="serviceProvider">Service provider implementation.</param>
    /// <returns>Instance of the step, or <see langword="null"/> if no matching instance can be found.</returns>
    IStepBody? ConstructBody(IServiceProvider serviceProvider);
}