using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios
{
    public class EndWorkflow : IPipeline
    {
        internal static int Step1Ticker = 0;
        internal static int Step2Ticker = 0;

        public class Step1 : BaseStepBody
        {
            public Step1() : base(a => Step1Ticker = a) { }
        }

        public class Step2 : BaseStepBody
        {
            public Step2() : base(a => Step2Ticker = a) { }
        }

        public IPipelineDefinition<DefaultModel> Build()
        {
            return CreatePipeline.Builder()
                .StartWith<Step1>()
                .EndWorkflow()
                .AddStep<Step2>()
                .Compile();
        }
    }

    public class EndWorkflowScenario : PipelineTestHelper
    {
        public EndWorkflowScenario()
        {
            Setup();
        }

        [Fact]
        public async Task Should_stop_pipeline_at_endworkflow_step()
        {
            var result = await StartPipeline(new EndWorkflow());

            Assert.True(result.IsSuccess);
            Assert.Equal(1, EndWorkflow.Step1Ticker);
            Assert.Equal(0, EndWorkflow.Step2Ticker);
        }
    }
}