using PipelineSteps.Builders;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.Logic;
using PipelineSteps.Models;
using PipelineSteps.Steps;

namespace PipelineSteps.Integration.Components;

public class StepBuilderTest
{
    private readonly IPipelineBuilder<DefaultModel> _workflowBuilder;
    private readonly StepBuilder<DefaultModel, MyStep1> _feature;

    public StepBuilderTest()
    {
        _workflowBuilder = new PipelineBuilder<DefaultModel>(new DefaultModel());
        _feature = new(_workflowBuilder, new PipelineStep<MyStep1>());
        _feature.Workflow.AddStep(_feature.Step);
    }

    [Fact]
    public void AddStep_method_should_apply_current_id_as_next_step_id_of_last_step_in_chain()
    {
        var actual = _feature.AddStep<MyStep3>();
        var firstStep = actual.Workflow.Steps.First();

        Assert.Equal(2, firstStep.Value.NextStepId);
    }

    [Fact]
    public void If_step_should_assign_start_with_step_to_workflow_instance()
    {
        ModelData data = new();

        var actual = _feature.If(data, a => a.Input2 == 0).Do(b => b.StartWith<MyStep3>());

        Assert.Equal(3, _workflowBuilder.Steps.Count);
        var stepIf = Assert.IsAssignableFrom<PipelineStep<IfStep, bool>>(actual.Step);
        Assert.Equal([3], stepIf.ChildStepsId);
        Assert.Equal(3, _feature.Workflow.LastStartWithStep);
    }

    [Fact]
    public void If_method_step_should_link_to_the_next_step_after_the_do_child_steps()
    {
        ModelData data = new();

        var actual = _feature
            .If(data, a => a.Input2 == 0)
            .Do(b => b.StartWith<MyStep3>())
            .AddStep<MyStep3>();

        Assert.Equal(4, _workflowBuilder.Steps.Count);
        var stepIf = Assert.IsAssignableFrom<PipelineStep<IfStep, bool>>(_workflowBuilder.Steps[2]);
        Assert.Equal(stepIf.NextStepId, actual.Step.Id);
    }

    [Fact]
    public void If_method_step_with_multiple_children_should_link_to_the_next_then_method_in_the_workflow_instance()
    {
        ModelData data = new();

        var actual = _feature
            .If(data, a => a.Input2 == 0)
            .Do(b => b.StartWith<MyStep3>().AddStep<MyStep1>())
            .AddStep<MyStep3>();

        Assert.Equal(5, _workflowBuilder.Steps.Count);
        var stepIf = Assert.IsAssignableFrom<PipelineStep<IfStep, bool>>(_workflowBuilder.Steps[2]);
        Assert.Equal(stepIf.NextStepId, actual.Step.Id);
        Assert.Equal([3, 4],  stepIf.ChildStepsId);
    }
}