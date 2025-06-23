using PipelineSteps.Events;
using PipelineSteps.Helpers;
using PipelineSteps.Models;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class StartEventWorkflow : IPipeline
{
    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWithEvent(a => a.Reference, "hello world")
            .Compile();
    }
}

public class StartWithEventScenario : PipelineTestHelper
{
    internal static int EventStep1 = 0;
    internal static string Reference1;

    public StartWithEventScenario()
    {
        Setup();
    }

    internal async Task TestEvent(LifeCycleEvent evt)
    {
        EventStep1 = evt.StepId;
        Reference1 = evt.Reference;

        await Task.CompletedTask;
    }

    [Fact]
    public async Task Should_run_publish_event_first_steps()
    {
        Controller.StepEventAsync += TestEvent;

        var result = await StartPipeline(new StartEventWorkflow());

        Assert.True(result.IsSuccess);

        Assert.Equal("hello world", Reference1);
    }
}