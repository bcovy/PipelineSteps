using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Definitions;
using PipelineSteps.Helpers;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Logic;
using PipelineSteps.Models;
using PipelineSteps.Steps;

namespace PipelineSteps.Builders;

public class StepBuilder<TData, TStepBody>(IPipelineBuilder<TData> workflow, IPipelineStep step) : IStepBuilder<TData, TStepBody>, IPipelineModifier<TData, TStepBody>
    where TData : class
    where TStepBody : IStepBody
{
    public IPipelineBuilder<TData> Workflow { get; } = workflow;
    public IPipelineStep Step { get; } = step;

    #region Add step
    public IStepBuilder<TData, TStep> AddStep<TStep>() where TStep : IStepBody
    {
        var workflowStep = new PipelineStep<TStep>();

        Step.NextStepId = Workflow.AddStep(workflowStep);

        return new StepBuilder<TData, TStep>(Workflow, workflowStep);
    }

    public IStepBuilder<TData, TStep> AddStep<TStep, T>(Expression<Func<TStep, T>> inputExpression, T inputValue) where TStep : IStepBody
    {
        var workflowStep = new PipelineStep<TStep, T>()
        {
            InputExpression = inputExpression,
            InputValue = inputValue
        };

        Step.NextStepId = Workflow.AddStep(workflowStep);

        return new StepBuilder<TData, TStep>(Workflow, workflowStep);
    }
    
    #endregion

    #region Publish event
    public IStepBuilder<TData, EventStep> PublishEvent(Expression<Func<EventStep, string>> inputExpression, string reference)
    {
        var workflowStep = new PipelineStep<EventStep, string>()
        {
            InputExpression = inputExpression,
            InputValue = reference
        };

        Step.NextStepId = Workflow.AddStep(workflowStep);

        return new StepBuilder<TData, EventStep>(Workflow, workflowStep);
    }
    
    #endregion

    #region Input
    public IStepBuilder<TData, TStepBody> Input(Action<TStepBody, TData> action)
    {
        Step.Inputs.Add(new ActionParameter<TStepBody, TData>(action));

        return this;
    }

    public IStepBuilder<TData, TStepBody> Input<TInput>(Expression<Func<TStepBody, TInput>> stepProperty, Expression<Func<TData, TInput>> value)
    {
        Step.Inputs.Add(new MemberMapParameter(value, stepProperty));

        return this;
    }
    
    #endregion
    
    #region Output
    public IStepBuilder<TData, TStepBody> Output<TOutput>(Expression<Func<TData, TOutput>> dataProperty, Expression<Func<TStepBody, TOutput>> value)
    {
        Step.Outputs.Add(new MemberMapParameter(value, dataProperty));

        return this;
    }
    
    #endregion

    #region Retry
    public IStepBuilder<TData, TStepBody> RetryOnError(TimeSpan retryInterval)
    {
        Step.RetryOnFailure = true;
        Step.RetryInterval = retryInterval;

        return this;
    }
    
    #endregion

    #region Modifiers if
    public IPipelineModifier<TData, IfStep> If(bool value)
    {
        var newStep = new PipelineStep<IfStep, bool>()
        {
            InputExpression = (Expression<Func<IfStep, bool>>)(x => x.Condition),
            InputValue = value
        };

        Step.NextStepId = Workflow.AddStep(newStep);

        return new StepBuilder<TData, IfStep>(Workflow, newStep);
    }

    public IPipelineModifier<TData, IfStep> If(Expression<Func<TData, bool>> condition)
    {
        var newStep = new PipelineStep<IfStep>();

        newStep.Inputs.Add(new MemberMapParameter(condition, (Expression<Func<IfStep, bool>>)(x => x.Condition)));

        Step.NextStepId = Workflow.AddStep(newStep);

        return new StepBuilder<TData, IfStep>(Workflow, newStep);
    }

    public IPipelineModifier<TData, IfStep> If<T>(T data, Expression<Func<T, bool>> condition)
    {
        var newStep = new PipelineStep<IfStep, bool>()
        {
            InputExpression = (Expression<Func<IfStep, bool>>)(x => x.Condition),
            InputValue = condition.Compile().Invoke(data)
        };

        Step.NextStepId = Workflow.AddStep(newStep);

        return new StepBuilder<TData, IfStep>(Workflow, newStep);
    }
    
    public IStepBuilder<TData, TStepBody> Do(Action<IPipelineBuilder<TData>> builder)
    {
        builder.Invoke(Workflow);
        Queue<int> children = new();

        for (int count = Workflow.LastStartWithStep; count <= Workflow.LastStep; count++)
            children.Enqueue(count);

        Step.ChildStepsId = children.ToArray();
        //returns current step instance so that next step id in the chain after child steps is linked.
        return this;
    }
    
    #endregion

    #region Set step name

    public IPipelineModifier<TData, TStepBody> SetStepName(string name)
    {
        if (typeof(TStepBody) == typeof(IfStep))
        {
            Step.Inputs.Add(new ActionParameter<IfStep, TData>((x, _) => x.Name = name));
        }
        else if (typeof(TStepBody) == typeof(Decide))
        {
            Step.Inputs.Add(new ActionParameter<Decide, TData>((x, _) => x.Name = name));
        }

        return this;
    }

    #endregion

    #region Inline Step
    public IStepBuilder<TData, InlineStep<TData>> InlineStep(Func<TData, IStepResult> body)
    {
        PipelineInlineStep<TData> inline = new(body, Workflow.Data);

        Step.NextStepId = Workflow.AddStep(inline);

        return new StepBuilder<TData, InlineStep<TData>>(Workflow, inline);
    }

    #endregion

    #region End workflow
    public IStepBuilder<TData, EndStep> EndWorkflow()
    {
        var workflowStep = new PipelineStep<EndStep>();

        Step.NextStepId = Workflow.AddStep(workflowStep);

        return new StepBuilder<TData, EndStep>(Workflow, workflowStep);
    }
    #endregion
    
    #region Branch
    public IPipelineModifier<TData, Decide> Decide(Expression<Func<TData, object>> condition)
    {
        var workflowStep = new PipelineStep<Decide>();

        workflowStep.Inputs.Add(new MemberMapParameter(condition, (Expression<Func<Decide, object>>)(x => x.OutcomeValue)));

        Step.NextStepId = Workflow.AddStep(workflowStep);

        return new StepBuilder<TData, Decide>(Workflow, workflowStep);
    }

    public IStepBuilder<TData, TStepBody> Branch<TStep, TValue>(TValue value, IStepBuilder<TData, TStep> branch) where TStep : IStepBody where TValue : struct
    {
        ValueOutcome<TValue> outcome = new(BranchDefinition.Create(branch.Workflow.WorkflowName, branch.Workflow.Steps), value);
        Step.StepBranches.Add(outcome);

        return this;
    }

    public IStepBuilder<TData, TStepBody> Branch<TStep>(Expression<Func<TData, object, bool>> expression, IStepBuilder<TData, TStep> branch) where TStep : IStepBody
    {
        ExpressionOutcome<TData> outcome = new(BranchDefinition.Create(branch.Workflow.WorkflowName, branch.Workflow.Steps), expression);
        Step.StepBranches.Add(outcome);

        return this;
    }
    
    #endregion
    
    #region Compile
    /// <summary>
    /// Compiles workflow steps into a <see cref="IPipelineDefinition{TData}"/> object, using the <typeparamref name="TData"/>
    /// as the data type to pass and/or store data between steps.  Arguments can be used to override the middleware pipeline options that 
    /// were set at dependency injection setup.
    /// </summary>
    /// <remarks>
    /// By default, the method will not make any middleware changes unless its associated argument is set to true. 
    /// </remarks>
    /// <param name="useErrorMiddleware">If true, will run the error middleware pipeline if the error is raised during step execution.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="usePreWorkflowMiddleware">If true, will run the pre-workflow middleware pipeline before executing steps.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="usePostWorkflowMiddleware">If true, will run the post-workflow middleware pipeline after all steps have been executed.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddWorkflowMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="useStepMiddleware">If true, will run the post-step execution middleware pipeline.  Assumes associated 
    /// middleware item(s) are registered in DI container using the <see cref="DiConfiguration.AddStepMiddleware{IMiddleware}(IServiceCollection)"/> helper class.</param>
    /// <param name="optOutOfPersistEvents">If true, will override the persist event steps option and stop the library from adding event data to the target database.</param>
    /// <returns>Compiled workflow object of type <see cref="IPipelineDefinition{TData}"/>.</returns>
    public IPipelineDefinition<TData> Compile(bool useErrorMiddleware = false,
        bool usePreWorkflowMiddleware = false,
        bool usePostWorkflowMiddleware = false,
        bool useStepMiddleware = false,
        bool optOutOfPersistEvents = false)
    {
        return new PipelineDefinition<TData>(Workflow.Data, Workflow.WorkflowName, Workflow.Steps,
            useErrorMiddleware, usePreWorkflowMiddleware, usePostWorkflowMiddleware, useStepMiddleware, optOutOfPersistEvents);
    }
    
    #endregion
}