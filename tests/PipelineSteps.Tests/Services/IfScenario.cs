using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class IfScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public IfScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_execute_child_steps_when_if_condition_result_is_returned()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddIfStep(1, 4, [2, 3]).AddStep(2, 3).AddStep(3, 0).AddStep(4, 0).Build("if");

        _ = await _feature.Execute(definition);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(3, _stepExecutor.CallOrder[2]);
        Assert.Equal(4, _stepExecutor.CallOrder[3]);
    }

    [Fact]
    public async Task Should_skip_child_steps_when_if_condition_result_is_not_returned()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 4).AddStep(2, 3).AddStep(3, 0).AddStep(4, 0).Build("if");

        _ = await _feature.Execute(definition);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(4, _stepExecutor.CallOrder[1]);
    }
}
