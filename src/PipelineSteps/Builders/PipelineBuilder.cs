using System.Linq.Expressions;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Logic;
using PipelineSteps.Steps;

namespace PipelineSteps.Builders;

public class PipelineBuilder<TData>(TData data, string workflowName = "Workflow")  : IPipelineBuilder<TData> where TData : class
{
    public string WorkflowName { get; } = workflowName;
    public TData Data { get; } = data;
    public Dictionary<int, IPipelineStep> Steps { get; } = [];
    public int LastStartWithStep { get; private set; }
    public int LastStep => Steps.Max(x => x.Key);
    
    public int AddStep(IPipelineStep step)
    {
        step.Id = Steps.Count + 1;
        Steps.Add(step.Id, step);
        return step.Id;
    }

    private void AddStartWithStep(IPipelineStep step)
    {
        step.Id = Steps.Count + 1;
        Steps.Add(step.Id, step);
        LastStartWithStep = step.Id;
    }
    
    public IPipelineBuilder<TData> CreateBranch(string branchName = "PipelineBranch")
    {
        return new PipelineBuilder<TData>(Data, branchName);
    }

    public IStepBuilder<TData, TStep> StartWith<TStep>() where TStep : IStepBody
    {
        var workflowStep = new PipelineStep<TStep>();
        AddStartWithStep(workflowStep);

        return new StepBuilder<TData, TStep>(this, workflowStep);
    }

    public IStepBuilder<TData, TStep> StartWith<TStep, T>(Expression<Func<TStep, T>> inputExpression, T inputValue) where TStep : IStepBody
    {
        var workflowStep = new PipelineStep<TStep, T>()
        {
            InputExpression = inputExpression,
            InputValue = inputValue
        };
        AddStartWithStep(workflowStep);

        return new StepBuilder<TData, TStep>(this, workflowStep);
    }

    public IStepBuilder<TData, InlineStep<TData>> StartWith(Func<TData, IStepResult> body)
    {
        PipelineInlineStep<TData> inline = new(body, Data);
        AddStartWithStep(inline);

        return new StepBuilder<TData, InlineStep<TData>>(this, inline);
    }

    public IStepBuilder<TData, EventStep> StartWithEvent(Expression<Func<EventStep, string>> inputExpression, string reference)
    {
        var workflowStep = new PipelineStep<EventStep, string>()
        {
            InputExpression = inputExpression,
            InputValue = reference
        };
        AddStartWithStep(workflowStep);

        return new StepBuilder<TData, EventStep>(this, workflowStep);
    }
}