using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;

namespace PipelineSteps.Models;

public class ActionParameter<TStepBody, TData>(Action<TStepBody, TData> action) : IStepParameter
{
    private void Assign(object data, IStepBody step)
    {
        action.Invoke((TStepBody)step, (TData)data);
    }

    public void AssignInput(object data, IStepBody body)
    {
        Assign(data, body);
    }

    public void AssignOutput(object data, IStepBody body)
    {
        Assign(data, body);
    }
}