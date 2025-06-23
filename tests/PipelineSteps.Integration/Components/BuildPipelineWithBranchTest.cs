using PipelineSteps.Helpers;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Integration.Components;

public class BuildPipelineWithBranchTest
{
    [Fact]
    public void Workflow_builder_is_able_to_create_new_builders_with_common_model_type()
    {
        var builder = CreatePipeline.Builder(new ModelData());
        var branch1 = builder.CreateBranch().StartWith<MyStep1>();
        var branch2 = builder.CreateBranch().StartWith<MyStep2>();

        Assert.IsAssignableFrom<IStepBuilder<ModelData, MyStep1>>(branch1);
        Assert.IsAssignableFrom<IStepBuilder<ModelData, MyStep2>>(branch2);
    }

    [Fact]
    public void CreateBranch_method_returns_new_workflow_with_first_step_id_of_1()
    {
        var builder = CreatePipeline.Builder(new ModelData());
        var branch1 = builder.CreateBranch().StartWith<MyStep1>();
        var branch2 = builder.CreateBranch().StartWith<MyStep2>();
        var parent = builder.StartWith<MyStep1>();

        Assert.Equal(1, branch1.Step.Id);
        Assert.Equal(1, branch2.Step.Id);
        Assert.Equal(1, parent.Step.Id);
    }

    [Fact]
    public void Should_be_able_to_add_decision_branches_using_fluent_methods()
    {
        ModelData data = new() { BranchOutcome = 2 };
        var builder = CreatePipeline.Builder(data);

        var branch1 = builder.CreateBranch().StartWith<MyStep1>();
        var branch2 = builder.CreateBranch().StartWith<MyStep2>();
        var actual = builder.StartWith<MyStep3>()
            .Decide(a => a.BranchOutcome)
                .Branch(1, branch1)
                .Branch(2, branch2)
                .Compile();
        var decideStep = actual.Steps[2];

        Assert.Equal(2, decideStep.StepBranches.Count);
        Assert.Equal(2, actual.StepsCount);
    }

    [Fact]
    public void Adding_branches_does_not_effect_parent_steps_collection()
    {
        var builder = CreatePipeline.Builder(new ModelData());
        var branch1 = builder.CreateBranch().StartWith<MyStep1>();
        var branch2 = builder.CreateBranch().StartWith<MyStep2>();

        var actual = builder.StartWith<MyStep3>()
            .Decide(a => a.BranchOutcome)
                .Branch(1, branch1)
                .Branch(2, branch2)
                .AddStep<MyStep1>()
                .Compile();

        Assert.Equal(3, actual.StepsCount);
    }
}