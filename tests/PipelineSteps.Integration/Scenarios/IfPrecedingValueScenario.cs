using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios
{
    public class IfPrecedingWorkflow : IPipeline<ModelData>
    {
        internal static int Step1Ticker = 0;
        internal static int Step2Ticker = 0;

        private readonly bool _proceed;

        public class Step1 : BaseStepBody
        {
            public bool Proceed { get; set; }

            public Step1() : base(a => Step1Ticker = a) { }
        }

        public class Step2 : BaseStepBody
        {
            public Step2() : base(a => Step2Ticker = a) { }
        }

        public IfPrecedingWorkflow(bool proceed)
        {
            _proceed = proceed;
        }

        public IPipelineDefinition<ModelData> Build()
        {
            return CreatePipeline.Builder(new ModelData())
                .StartWith<Step1, bool>(a => a.Proceed, _proceed)
                    .Output(step => step.Proceed, i => i.Proceed)
                .If(a => a.Proceed)
                    .Do(d => d.StartWith<Step2>())
                .Compile();
        }
    }

    public class IfPrecedingValueScenario : PipelineTestHelper
    {
        public IfPrecedingValueScenario()
        {
            Setup();
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public async Task Should_run_next_step_based_on_results_from_preceding_step(bool proceed, int expected)
        {
            IfPrecedingWorkflow.Step2Ticker = 0;

            _ = await StartPipeline(new IfPrecedingWorkflow(proceed));

            Assert.Equal(expected, IfPrecedingWorkflow.Step2Ticker);
        }
    }
}