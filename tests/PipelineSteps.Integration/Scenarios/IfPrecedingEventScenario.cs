using PipelineSteps.Events;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios
{
    public class IfPrecedingEventWorkflow : IPipeline<ModelData>
    {
        internal static int Step1Ticker = 0;
        internal static int Step2Ticker = 0;

        public class Step1 : BaseStepBody
        {
            public bool Proceed { get; set; }

            public Step1() : base(a => Step1Ticker = a) { }
        }

        public class Step2 : BaseStepBody
        {
            public Step2() : base(a => Step2Ticker = a) { }
        }

        public IPipelineDefinition<ModelData> Build()
        {
            return CreatePipeline.Builder(new ModelData())
                .StartWith<Step1, bool>(a => a.Proceed, true)
                    .Output(step => step.Proceed, i => i.Proceed)
                .PublishEvent(a => a.Reference, "hello1")
                .If(a => a.Proceed)
                    .Do(d => d.StartWith<Step2>().PublishEvent(a => a.Reference, "hello2"))
                .Compile();
        }
    }

    public class IfPrecedingEventScenario : PipelineTestHelper
    {
        internal static int EventStep1 = 0;
        internal static string Reference1;
        internal static string Reference2;

        public IfPrecedingEventScenario()
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

            var result = await StartPipeline(new IfPrecedingEventWorkflow());

            Assert.True(result.IsSuccess);
            Assert.Equal(1, IfPrecedingEventWorkflow.Step2Ticker);
            Assert.Equal("hello1", Reference1);
            Assert.Equal("hello2", Reference2);
        }
    }
}