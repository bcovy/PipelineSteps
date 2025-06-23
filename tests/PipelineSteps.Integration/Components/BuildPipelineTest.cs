using PipelineSteps.Helpers;
using PipelineSteps.Integration.Steps;

namespace PipelineSteps.Integration.Components;

public class BuildWorkflowTest
{
    [Fact]
    public void Fluent_methods_should_build_definition_with_correct_linked_step_order()
    {
        var workflow = CreatePipeline.Builder().StartWith<MyStep1>()
            .AddStep<MyStep2>()
            .AddStep<MyStep3>()
            .Compile();

        Assert.Equal(3, workflow.StepsCount);
        Assert.Equal(2, workflow.Steps[1].NextStepId);
        Assert.Equal(3, workflow.Steps[2].NextStepId);
        Assert.Equal(0, workflow.Steps[3].NextStepId);
    }

    [Fact]
    public void Non_fluent_logic_should_build_definition_with_correct_linked_step_order()
    {
        var step1 = CreatePipeline.Builder().StartWith<MyStep1>();
        var step2 = step1.AddStep<MyStep2>();
        var step3 = step2.AddStep<MyStep3>();

        var actual = step3.Compile();

        Assert.Equal(3, actual.StepsCount);
        Assert.Equal(2, actual.Steps[1].NextStepId);
        Assert.Equal(3, actual.Steps[2].NextStepId);
        Assert.Equal(0, actual.Steps[3].NextStepId);
    }
}