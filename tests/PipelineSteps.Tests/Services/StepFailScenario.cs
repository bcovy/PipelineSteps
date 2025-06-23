using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class StepFailScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public StepFailScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_not_continue_workflow_after_failed_step()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 2).AddErrorStep(2, 3).AddStep(3, 0).Build("fail");

        _ = await _feature.Execute(definition);

        Assert.True(_stepExecutor.CallOrder.Count == 2);
        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
    }
}
