using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class MixedLogicScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public MixedLogicScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_execute_if_and_branch_steps_in_sequential_order()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddIfStep(1, 4, [2, 3])
            .AddStep(2, 3)
            .AddStep(3, 0)
            .AddStep(4, 5)
            .AddBranchStep(5, 0)
            .Build("if");

        _ = await _feature.Execute(definition);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(3, _stepExecutor.CallOrder[2]);
        Assert.Equal(4, _stepExecutor.CallOrder[3]);
        Assert.Equal(5, _stepExecutor.CallOrder[4]);
        Assert.Equal(1, _stepExecutor.CallOrder[5]);
        Assert.Equal(2, _stepExecutor.CallOrder[6]);
    }
}
