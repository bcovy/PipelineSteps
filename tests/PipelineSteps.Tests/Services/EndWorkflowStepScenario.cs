using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class EndWorkflowStepScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public EndWorkflowStepScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_not_continue_workflow_after_end_workflow_step_result()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 2).AddEndStep(2, 3).AddStep(3, 0).Build("end");

        _ = await _feature.Execute(definition);

        Assert.True(_stepExecutor.CallOrder.Count == 2);
        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
    }
}
