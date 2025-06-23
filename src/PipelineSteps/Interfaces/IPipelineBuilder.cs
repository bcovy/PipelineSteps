using System.Linq.Expressions;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Logic;

namespace PipelineSteps.Interfaces;

public interface IPipelineBuilder<TData> where TData : class
{
    string WorkflowName { get; }
    TData Data { get; }
    Dictionary<int, IPipelineStep> Steps { get; }
    int LastStartWithStep { get; }
    int LastStep { get; }
    int AddStep(IPipelineStep step);
    /// <summary>
    /// Creates a workflow branch of type <typeparamref name="TData"/>.
    /// </summary>
    /// <param name="branchName">Branch name.</param>
    /// <returns><see cref="IPipelineBuilder{TData}"/> workflow pipeline builder.</returns>
    IPipelineBuilder<TData> CreateBranch(string branchName = "WorkflowBranch");
    /// <summary>
    /// Adds the first step that starts the workflow chain.
    /// </summary>
    /// <typeparam name="TStep">Step type.</typeparam>
    /// <returns><typeparamref name="TStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStep> StartWith<TStep>() where TStep : IStepBody;
    /// <summary>
    /// Adds the first step that starts the workflow chain.
    /// </summary>
    /// <typeparam name="TStep">Step body type.</typeparam>
    /// <typeparam name="T">Input data type.</typeparam>
    /// <param name="inputExpression">Expression that assigns value to associated property value.</param>
    /// <param name="inputValue">Input value.</param>
    /// <returns><typeparamref name="TStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStep> StartWith<TStep, T>(Expression<Func<TStep, T>> inputExpression, T inputValue) where TStep : IStepBody;
    /// <summary>
    /// Adds the first step that starts the workflow chain.  Uses user supplied function to create ad hoc step body. 
    /// </summary>
    /// <param name="body">Ad hoc step body function.</param>
    /// <returns><see cref="InlineStep{TData}"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, InlineStep<TData>> StartWith(Func<TData, IStepResult> body);
    /// <summary>
    /// Step that publishes an Event for the <see cref="IPipelineController.StepEventAsync"/> event handler.
    /// </summary>
    /// <remarks>
    /// Requires that subscription be made to the <see cref="IPipelineController.StepEventAsync"/> event delegate.
    /// </remarks>
    /// <param name="inputExpression">Input expression.</param>
    /// <param name="reference">Event reference value.</param>
    /// <returns><see cref="EventStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, EventStep> StartWithEvent(Expression<Func<EventStep, string>> inputExpression, string reference);
}