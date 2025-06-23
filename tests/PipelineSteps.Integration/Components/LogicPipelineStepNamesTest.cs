using PipelineSteps.Interfaces;
using PipelineSteps.Logic;
using PipelineSteps.Models;
using PipelineSteps.Steps;

namespace PipelineSteps.Integration.Components;

public class LogicPipelineStepNamesTest
{
    [Fact]
    public void Updates_if_default_step_body_name_with_input_assignment_value()
    {
        PipelineStep<IfStep> feature = new();
        Action<IfStep, string> name = (x, y) => x.Name = "hello world";
        feature.Inputs.Add(new ActionParameter<IfStep, string>(name));
        IStepBody body = new IfStep();
        string data = "hello world";

        foreach (var item in feature.Inputs)
            item.AssignInput(data, body);

        Assert.Equal(data, body.Name);
    }

    [Fact]
    public void Updates_decide_default_step_body_name_with_input_assignment_value()
    {
        PipelineStep<Decide> feature = new();
        Action<Decide, string> name = (x, y) => x.Name = "hello world";
        feature.Inputs.Add(new ActionParameter<Decide, string>(name));
        IStepBody body = new Decide();
        string data = "hello world";

        foreach (var item in feature.Inputs)
            item.AssignInput(data, body);

        Assert.Equal(data, body.Name);
    }
}
