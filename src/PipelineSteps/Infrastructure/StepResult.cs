using PipelineSteps.Enums;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Infrastructure;

public class StepResult : IStepResult
{
    public bool IsSuccess { get; private init; }
    public StepResultType Result { get; private init; }
    public string? ErrorMessage { get; private init; }
    public string? Reference { get; private init; }
    public string? Description { get; private init; }
    public object? OutcomeValue { get; private init; }
    public static StepResult Next() => new() { Result = StepResultType.Success, IsSuccess = true };

    public static StepResult Completed() => new() { Result = StepResultType.Success, IsSuccess = true };
    public static StepResult Fail(string error) => new() { ErrorMessage = error, Result = StepResultType.Failure, IsSuccess = false};
    public static StepResult Branch(object outcomeValue) => new() { Result = StepResultType.Branch, IsSuccess = true, OutcomeValue = outcomeValue };
    public static StepResult IfCondition() => new() { Result = StepResultType.IfCondition, IsSuccess = true };
    public static StepResult Event(string reference, string description, object? outcomeValue = null) => new()
    {
        Result = StepResultType.Event,
        IsSuccess = true,
        Reference = reference,
        Description = description,
        OutcomeValue = outcomeValue
    };
    public static StepResult EndWorkflow() => new() { Result = StepResultType.EndWorkflow, IsSuccess = true };
}