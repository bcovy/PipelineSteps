using PipelineSteps.Enums;

namespace PipelineSteps.Interfaces;

public interface IStepResult
{
    StepResultType Result { get; }
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
    string? Reference { get; }
    string? Description { get; }
    object? OutcomeValue { get; }
}