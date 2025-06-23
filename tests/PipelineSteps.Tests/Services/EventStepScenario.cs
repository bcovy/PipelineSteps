using PipelineSteps.Events;
using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class EventStepScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    internal static string Reference1;

    public EventStepScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
        Reference1 = string.Empty;
    }

    internal async Task TestEvent(LifeCycleEvent evt)
    {
        Reference1 = evt.Reference;

        await Task.CompletedTask;
    }

    [Fact]
    public async Task Should_handle_event_step()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 2).AddEventStep(2, 3).AddStep(3, 0).Build("event");
        _feature.StepEventAsync += TestEvent;

        _ = await _feature.Execute(definition);

        Assert.Equal("hello", Reference1);
        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(3, _stepExecutor.CallOrder[2]);
    }
}
