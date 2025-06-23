using PipelineSteps.Definitions;
using PipelineSteps.Enums;
using PipelineSteps.Events;
using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;

namespace PipelineSteps.Services;

public class PipelineController(IMiddlewareController middleware, IStepExecutor stepExecutor, PipelineOptions options) : IPipelineController
{
    public event StepEventHandlerAsync? StepEventAsync;

    public async Task HandleLifeCycleEventAsync(int stepId, string description, IStepResult result)
    {
        LifeCycleEvent message = new()
        {
            StepId = stepId,
            Description = description,
            EventTimeUtc = DateTime.UtcNow,
            OutcomeValue = result.OutcomeValue,
            Reference = result.Reference,
            Result = StepResultType.Success
        };

        if (StepEventAsync != null)
            await StepEventAsync.Invoke(message);
    }

    public async Task<IPipelineResult> StartPipeline<TData>(IPipeline<TData> pipeline) where TData : class
    {
        var definition = pipeline.Build();
        //Pre-workflow actions.
        if (definition.UsePreWorkflowMiddleware)
            await middleware.RunPreWorkflowPipeline(new PipelineResult(definition.Name, definition.StepsCount));

        //Start the pipeline.
        var result = await Execute(definition);
        //Error step middleware.
        if (!result.IsSuccess && definition.UseErrorMiddleware)
            await middleware.RunErrorPipeline(result);

        //Post pipeline actions.
        if (stepExecutor.PersistStepEventsActive && !definition.OptOutOfPersistEvents)
            await stepExecutor.PersistEvents();
        //Post pipeline actions.
        if (definition.UsePostWorkflowMiddleware)
        {
            if (options.RunPostWorkflowMiddlewareCondition == PostMiddleware.OnSuccess && !result.IsSuccess)
                return result;

            await middleware.RunPostWorkflowPipeline(result);
        }

        return result;
    }

    public async Task<IPipelineResult> Execute<TData>(IPipelineDefinition<TData> workflow) where TData : class
    {
        ExecutionPointer pointer = new(1);
        PipelineExecutionContext context = new(workflow.Name, workflow.ContextId, workflow.UseStepMiddleware);
        PipelineResult wr = new(workflow.Name, workflow.StepsCount);

        while (true)
        {
            int stepId = pointer.GetNextId();
            //End workflow condition.
            if (stepId == 0)
                break;

            var step = workflow.Steps[stepId];
            wr.CurrentStep = step;

            var result = await stepExecutor.ExecuteStep(step, workflow.Data, context);

            switch (result.Result)
            {
                case StepResultType.Failure:
                    wr.AddError(result.ErrorMessage);
                    return wr;
                case StepResultType.EndWorkflow:
                    return wr;
                case StepResultType.IfCondition:
                    //Add both the next step and the first child step to the pointer stack.
                    //Adding 'NextStepId' first ensures execution will continue on
                    //the parent pipeline after child 'if' steps are done executing.
                    pointer.SetNextId(step.NextStepId);
                    pointer.SetNextId(step.ChildStepsId.First());

                    continue;
                //No exit/continue logic at this point.
                case StepResultType.Branch:
                {
                    context.IsBranch = true;
                    //Start execution on the branch and continue on the parent after completion if no errors.
                    foreach (var branch in step.StepBranches.Where(x => x.Matches(result.OutcomeValue, workflow.Data)).Select(x => x.Definition))
                    {
                        context.BranchName = branch.Name;
                        context.BranchId = branch.Id;
                        var branchResult = await ExecuteBranch(branch, workflow.Data, context);

                        if (!branchResult.IsSuccess)
                            return branchResult;
                    }

                    context.IsBranch = false;
                    context.BranchId = 0;
                    context.BranchName = string.Empty;
                    break;
                }
                case StepResultType.Event:
                    await HandleLifeCycleEventAsync(step.Id, step.BodyType.Name, result);
                    break;
            }

            //Add the next step id to the pointer stack.
            pointer.SetNextId(step.NextStepId);
        }

        return wr;
    }

    public async Task<IPipelineResult> ExecuteBranch<TData>(IBranchDefinition branch, TData data, PipelineExecutionContext context) where TData : class
    {
        //Start the pointer.
        ExecutionPointer pointer = new(1);
        PipelineResult wr = new(branch.Name, branch.StepsCount);

        while (true)
        {
            int stepId = pointer.GetNextId();
            //End workflow condition.
            if (stepId == 0)
                break;

            IPipelineStep step = branch.Steps[stepId];
            var result = await stepExecutor.ExecuteStep(step, data, context);

            switch (result.Result)
            {
                case StepResultType.Failure:
                    wr.CurrentStep = step;
                    wr.AddError(result.ErrorMessage);

                    return wr;
                case StepResultType.EndWorkflow:
                    return wr;
                case StepResultType.IfCondition:
                    pointer.SetNextId(step.NextStepId);
                    pointer.SetNextId(step.ChildStepsId.First());
                    continue;
                //No exit/continue logic at this point.
                case StepResultType.Event:
                    await HandleLifeCycleEventAsync(step.Id, step.BodyType.Name, result);
                    break;
            }

            //Add the next step id to the pointer stack.
            pointer.SetNextId(step.NextStepId);
        }

        return wr;
    }
}
