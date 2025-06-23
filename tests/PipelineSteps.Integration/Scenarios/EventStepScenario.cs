using PipelineSteps.Events;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios
{
    public class EventStepWorkflow : IPipeline
    {
        public IPipelineDefinition<DefaultModel> Build()
        {
            return CreatePipeline.Builder()
                .StartWith<MyStep3>()
                .PublishEvent(a => a.Reference, "hello1")
                .AddStep<MyStep3>()
                .PublishEvent(a => a.Reference, "hello2")
                .Compile();
        }
    }

    public class EventStepScenario : PipelineTestHelper
    {
        internal static int EventStep1 = 0;
        internal static string Reference1;
        internal static string Reference2;

        public EventStepScenario()
        {
            Setup();
        }

        internal async Task TestEvent(LifeCycleEvent evt)
        {
            EventStep1 = evt.StepId;
            Reference1 = evt.Reference == "hello1" ? evt.Reference : Reference1;
            Reference2 = evt.Reference == "hello2" ? evt.Reference : Reference2;

            await Task.CompletedTask;
        }

        [Fact]
        public async Task Should_run_publish_event_on_multiple_event_steps()
        {
            Controller.StepEventAsync += TestEvent;

            var result = await StartPipeline(new EventStepWorkflow());

            Assert.True(result.IsSuccess);
            Assert.Equal("hello1", Reference1);
            Assert.Equal("hello2", Reference2);
        }
    }
}