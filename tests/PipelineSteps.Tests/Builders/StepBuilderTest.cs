using NSubstitute;
using PipelineSteps.Builders;
using PipelineSteps.Interfaces;
using PipelineSteps.Logic;
using PipelineSteps.Steps;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Builders;

public class StepBuilderTest
{
    private readonly IPipelineBuilder<MockData> _workflowBuilder;
    private readonly StepBuilder<MockData, MockStep> _feature;

    public StepBuilderTest()
    {
        _workflowBuilder = Substitute.For<IPipelineBuilder<MockData>>();
        _feature = new(_workflowBuilder, new PipelineStep<MockStep>());
    }
    
    #region Init

    [Fact]
    public void Init_should_add_new_step_to_instance()
    {
        Assert.IsType<PipelineStep<MockStep>>(_feature.Step, exactMatch: false);
    }

    #endregion Init

    #region AddStep

    [Fact]
    public void AddStep_should_chain_new_workflow_step_and_return_same_step_builder_type()
    {
        var actual = _feature.AddStep<MockStep2>();

        Assert.IsType<PipelineStep<MockStep2>>(actual.Step, exactMatch: false);
    }

    [Fact]
    public void AddStep_of_value_type_T_should_chain_new_step_and_return_same_step_builder_type()
    {
        string value = "hello world";
        var actual = _feature.AddStep<MockStep2, string>(x => x.Input1, value);

        var result = Assert.IsType<PipelineStep<MockStep2, string>>(actual.Step, exactMatch: false);
        Assert.Equal(value, result.InputValue);
    }

    #endregion AddStep

    #region Input and output

    [Fact]
    public void Input_should_add_member_map_expression_for_last_step_in_chain()
    {
        _ = _feature.Input(d => d.Input1, i => i.Input1);

        Assert.NotEmpty(_feature.Step.Inputs);
    }

    [Fact]
    public void Output_should_add_member_map_expression_for_last_step_in_chain()
    {
        _ = _feature.Output(d => d.Input1, i => i.Input1);

        Assert.NotEmpty(_feature.Step.Outputs);
    }

    #endregion Input and output

    #region If do

    [Fact]
    public void If_step_using_raw_bool_value_should_chain_do_method_and_return_step_builder_of_type_if_logic()
    {
        var actual = _feature.If(true).Do(b => b.StartWith<MockStep2>());

        _workflowBuilder.Received().AddStep(Arg.Any<PipelineStep<IfStep, bool>>());
        _workflowBuilder.Received().StartWith<MockStep2>();
        Assert.IsType<PipelineStep<IfStep, bool>>(actual.Step, exactMatch: false);
    }

    [Fact]
    public void If_step_of_workflow_type_should_chain_do_method_and_return_step_builder_of_type_if_logic()
    {
        var actual = _feature.If(a => a.Input2 == 1).Do(b => b.StartWith<MockStep2>());

        _workflowBuilder.Received().AddStep(Arg.Any<PipelineStep<IfStep>>());
        _workflowBuilder.Received().StartWith<MockStep2>();
        Assert.IsType<PipelineStep<IfStep>>(actual.Step, exactMatch: false);
    }

    [Fact]
    public void If_step_of_user_type_should_chain_do_method_and_return_step_builder_of_type_if_logic()
    {
        MockData data = new();
        var actual = _feature.If(data, a => a.Input2 == 1).Do(b => b.StartWith<MockStep2>());

        _workflowBuilder.Received().AddStep(Arg.Any<PipelineStep<IfStep, bool>>());
        _workflowBuilder.Received().StartWith<MockStep2>();
        Assert.IsType<PipelineStep<IfStep, bool>>(actual.Step, exactMatch: false);
    }

    #endregion If do

    #region Decide branch

    [Fact]
    public void Decide_step_should_chain_new_workflow_step_and_return_same_step_builder_type()
    {
        var actual = _feature.Decide(a => a.Input2);

        Assert.IsType<IPipelineModifier<MockData, Decide>>(actual, exactMatch: false);
    }

    #endregion Decide branch
}