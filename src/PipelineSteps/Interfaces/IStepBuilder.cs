using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Helpers;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Logic;

namespace PipelineSteps.Interfaces;

public interface IStepBuilder<TData, TStepBody>
    where TData : class
    where TStepBody : IStepBody
{
    IPipelineBuilder<TData> Workflow { get; }
    /// <summary>
    /// The current/last workflow step that was added to the chain.
    /// </summary>
    IPipelineStep Step { get; }
    /// <summary>
    /// Add workflow step.
    /// </summary>
    /// <remarks>
    /// All steps will be executed in the order in which it was added.
    /// </remarks>
    /// <typeparam name="TStep">Step type.</typeparam>
    /// <returns><typeparamref name="TStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStep> AddStep<TStep>() where TStep : IStepBody;
    /// <summary>
    /// Add workflow step with associated input value.
    /// </summary>
    /// <remarks>
    /// All steps will be executed in the order in which it was added.
    /// </remarks>
    /// <typeparam name="TStep">Step body type.</typeparam>
    /// <typeparam name="T">Input data type.</typeparam>
    /// <param name="inputExpression">Expression that assigns value to associated property value.</param>
    /// <param name="inputValue">Input value.</param>
    /// <returns><typeparamref name="TStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStep> AddStep<TStep, T>(Expression<Func<TStep, T>> inputExpression, T inputValue) where TStep : IStepBody;
    /// <summary>
    /// Step that publishes an Event for the <see cref="IPipelineController.StepEventAsync"/> event handler.
    /// </summary>
    /// <remarks>
    /// Requires that subscription be made to the <see cref="IPipelineController.StepEventAsync"/> event delegate.
    /// </remarks>
    /// <param name="inputExpression">Input expression.</param>
    /// <param name="reference">Event reference value.</param>
    /// <returns><see cref="EventStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, EventStep> PublishEvent(Expression<Func<EventStep, string>> inputExpression, string reference);
    /// <summary>
    /// Uses <typeparamref name="TData"/> data model to apply input value to target step body context.  Property
    /// assignment is performed before execution of step body.
    /// </summary>
    /// <param name="action">Assignment function.</param>
    /// <returns><typeparamref name="TStepBody"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Input(Action<TStepBody, TData> action);
    /// <summary>
    /// Uses <typeparamref name="TData"/> data model to apply input value to target step body context.  Property
    /// assignment is performed before execution of step body.
    /// </summary>
    /// <typeparam name="TInput">Input value type.</typeparam>
    /// <param name="stepProperty">Step property source function.</param>
    /// <param name="value">Property value assignment function.</param>
    /// <returns><typeparamref name="TStepBody"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Input<TInput>(Expression<Func<TStepBody, TInput>> stepProperty, Expression<Func<TData, TInput>> value);
    /// <summary>
    /// Uses <typeparamref name="TStepBody"/> step body context to apply values to <typeparamref name="TData"/> data model.
    /// Data model assignment is performed after execution of step body.
    /// </summary>
    /// <remarks>
    /// Use this method if you want to persist changes made in the step body context to the <typeparamref name="TData"/> data model.
    /// </remarks>
    /// <typeparam name="TOutput">Output value type.</typeparam>
    /// <param name="dataProperty"><typeparamref name="TData"/> property value assignment function.</param>
    /// <param name="value">Step property source function.</param>
    /// <returns><typeparamref name="TStepBody"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Output<TOutput>(Expression<Func<TData, TOutput>> dataProperty, Expression<Func<TStepBody, TOutput>> value);
    /// <summary>
    /// Retry the step if it fails on the first execution.
    /// </summary>
    /// <remarks>
    /// Workflow will be paused at current step for the duration of the <paramref name="retryInterval"/>.
    /// </remarks>
    /// <param name="retryInterval">Next retry attempt.</param>
    /// <returns><typeparamref name="TStepBody"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> RetryOnError(TimeSpan retryInterval);
    /// <summary>
    /// Will apply ensuing step(s) in <see cref="IPipelineModifier{TData,TStepBody}.Do(Action{IPipelineBuilder{TData}})"/> method if
    /// <paramref name="value"/> is true.
    /// </summary>
    /// <remarks>
    /// Ensuing step(s) are added as children of parent.  As such, method should not be used in scenarios that include 
    /// conditional logic past the parent depth.  For more complex workflows that require nested if logic, use the 
    /// <see cref="Decide(Expression{Func{TData, object}})"/> step method.
    /// </remarks>
    /// <param name="value">Condition value.</param>
    /// <returns><see cref="IPipelineModifier{TData,TStepBody}"/> container to allow user to chain methods.</returns>
    IPipelineModifier<TData, IfStep> If(bool value);
    /// <summary>
    /// Will apply ensuing step(s) in <see cref="IPipelineModifier{TData,TStepBody}.Do(Action{IPipelineBuilder{TData}})"/> 
    /// method if <paramref name="condition"/> expression returns a true result.  Otherwise, ensuing child steps will be skipped. 
    /// </summary>
    /// <remarks>
    /// Ensuing step(s) are added as children of parent.  As such, method should not be used in scenarios that include 
    /// conditional logic past the parent depth.  For more complex workflows that require nested if logic, use the 
    /// <see cref="Decide(Expression{Func{TData, object}})"/> step method.
    /// </remarks>
    /// <param name="condition">Test condition to apply child steps.</param>
    /// <returns><see cref="IPipelineModifier{TData,TStepBody}"/> container to allow user to chain methods.</returns>
    IPipelineModifier<TData, IfStep> If(Expression<Func<TData, bool>> condition);
    /// <summary>
    /// Will apply ensuing step(s) in  <see cref="IPipelineModifier{TData,TStepBody}.Do(Action{IPipelineBuilder{TData}})"/> 
    /// method if <paramref name="condition"/> expression returns a true result.  Otherwise, ensuing child steps will be skipped.
    /// <see cref="IfStep"/>.
    /// </summary>
    /// <remarks>
    /// Ensuing step(s) are added as children of parent.  As such, method should not be used in scenarios that include 
    /// conditional logic past the parent depth.  For more complex workflows that require nested if logic, use the 
    /// <see cref="Decide(Expression{Func{TData, object}})"/> step method.
    /// </remarks>
    /// <typeparam name="T">Input type.</typeparam>
    /// <param name="data">Input data source.</param>
    /// <param name="condition">Test condition to apply child steps.</param>
    /// <returns><see cref="IPipelineModifier{TData,TStepBody}"/> container to allow user to chain methods.</returns>
    IPipelineModifier<TData, IfStep> If<T>(T data, Expression<Func<T, bool>> condition);
    /// <summary>
    /// Uses user supplied function to create ad hoc step body. 
    /// </summary>
    /// <param name="body">Ad hoc step body function.</param>
    /// <returns><see cref="InlineStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, InlineStep<TData>> InlineStep(Func<TData, IStepResult> body);
    /// <summary>
    /// Will end the workflow pipeline execution at the current step.
    /// </summary>
    /// <returns><see cref="EndStep"/> step to allow user to chain methods.</returns>
    IStepBuilder<TData, EndStep> EndWorkflow();
    /// <summary>
    /// Evaluate an expression and take a different path depending on the value.
    /// </summary>
    /// <remarks>Associated Branches will be executed in sequential order.  Processing will return to parent
    /// workflow after Branches have completed.</remarks>
    /// <param name="condition">Logic condition.</param>
    /// <returns><see cref="IPipelineModifier{TData,TStepBody}"/> step to allow user to chain methods.</returns>
    IPipelineModifier<TData, Decide> Decide(Expression<Func<TData, object>> condition);
    /// <summary>
    /// Branch to execute if decision criteria matches <see cref="IStepBuilder{TData, TStepBody}.Decide(Expression{Func{TData, object}})"/> outcome value.
    /// </summary>
    /// <typeparam name="TStep">Step type.</typeparam>
    /// <typeparam name="TValue">Comparison value of type struct.</typeparam>
    /// <param name="value">Branch execution comparison value.</param>
    /// <param name="branch">Branch workflow.</param>
    /// <returns><typeparamref name="TStepBody"/>step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Branch<TStep, TValue>(TValue value, IStepBuilder<TData, TStep> branch) where TStep : IStepBody where TValue : struct;
    /// <summary>
    /// Branch to execute if decision criteria matches <see cref="IStepBuilder{TData, TStepBody}.Decide(Expression{Func{TData, object}})"/> outcome value.
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <param name="expression">Branch execution comparison expression.</param>
    /// <param name="branch">Branch workflow.</param>
    /// <returns><typeparamref name="TStepBody"/>step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Branch<TStep>(Expression<Func<TData, object, bool>> expression, IStepBuilder<TData, TStep> branch) where TStep : IStepBody;
    /// <summary>
    /// Compiles workflow steps into a <see cref="IPipelineDefinition{TData}"/> object, using the <typeparamref name="TData"/>
    /// as its data type to pass and/or store data between steps.  In addition, arguments can be used to add additional middleware steps, or override 
    /// options that were set at dependency injection setup.
    /// </summary>
    /// <remarks>
    /// By default, method will not make any middleware changes unless its associated argument is set to true. 
    /// </remarks>
    /// <param name="useErrorMiddleware">If true, will run error middleware pipeline if error is raised during step execution.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="usePreWorkflowMiddleware">If true, will run pre-workflow middleware pipeline before executing steps.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="usePostWorkflowMiddleware">If true, will run post-workflow middleware pipeline after all steps have been executed.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="useStepMiddleware">If true, will run post-step execution middleware pipeline.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddStepMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="optOutOfPersistEvents">If true, will override the persist event steps option and stop the library from adding event data to the target database.</param>
    /// <returns>Compiled workflow object of type <see cref="IPipelineDefinition{TData}"/>.</returns>
    IPipelineDefinition<TData> Compile(bool useErrorMiddleware = false, bool usePreWorkflowMiddleware = false, bool usePostWorkflowMiddleware = false, bool useStepMiddleware = false, bool optOutOfPersistEvents = false);
}