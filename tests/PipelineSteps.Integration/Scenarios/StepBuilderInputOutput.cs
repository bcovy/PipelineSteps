using NSubstitute;
using PipelineSteps.Builders;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.Steps;

namespace PipelineSteps.Integration.Scenarios;

public class StepBuilderInputOutput
{
    private readonly ModelData data;
    private readonly IPipelineBuilder<ModelData> workflowBuilder;
    private readonly StepBuilder<ModelData, MyStep3> stepBuilder;

    public StepBuilderInputOutput()
    {
        data = new ModelData() { Input1 = "hello world", Input2 = 99 };
        workflowBuilder = Substitute.For<IPipelineBuilder<ModelData>>();
        stepBuilder = new(workflowBuilder, new PipelineStep<MyStep1>());
    }

    [Fact]
    public void Input_should_apply_action_value_to_associated_step_body_properties()
    {
        MyStep3 body = new();

        stepBuilder.Input((step, data) => step.Input1 = "goodbye");
        foreach (var input in stepBuilder.Step.Inputs)
            input.AssignInput(data, body);

        Assert.Equal("goodbye", body.Input1);
    }

    [Fact]
    public void Input_should_apply_member_map_value_to_associated_step_body_properties()
    {
        MyStep3 body = new();

        stepBuilder.Input(data => data.Input1, s => s.Input1);
        foreach(var input in stepBuilder.Step.Inputs)
            input.AssignInput(data, body);

        Assert.Equal(data.Input1, body.Input1);
    }

    [Fact]
    public void Input_should_apply_multiple_member_map_values_to_associated_step_body_properties()
    {
        MyStep3 body = new();

        stepBuilder.Input(data => data.Input1, s => s.Input1);
        stepBuilder.Input(data => data.Input2, s => s.Input2);
        foreach (var input in stepBuilder.Step.Inputs)
            input.AssignInput(data, body);

        Assert.Equal(data.Input1, body.Input1);
        Assert.Equal(data.Input2, body.Input2);
    }

    [Fact]
    public void Output_should_apply_member_map_value_to_workflow_instance()
    {
        MyStep3 body = new();

        stepBuilder.Output(data => data.Input1, s => s.Output1);
        foreach (var output in stepBuilder.Step.Outputs)
            output.AssignOutput(data, body);

        Assert.Equal(data.Input1, data.Output1);
    }
}