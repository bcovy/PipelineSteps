namespace PipelineSteps.Interfaces.Internals;

public interface IStepParameter
{
    void AssignInput(object data, IStepBody body);
    void AssignOutput(object data, IStepBody body);
}