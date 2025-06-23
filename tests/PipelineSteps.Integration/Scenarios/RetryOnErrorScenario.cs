using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class RetryOnErrorWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public IPipelineDefinition<ModelData> Build()
    {
        return CreatePipeline.Builder(new ModelData())
            .StartWith<Step1>()
            .InlineStep(a =>
            {
                Step2Ticker++;

                if (Step2Ticker <= 1)
                {
                    return StepResult.Fail("Some error");
                }

                return StepResult.Next();
            }).RetryOnError(TimeSpan.FromMilliseconds(1))
            .Compile();
    }
}

public class RetryOnErrorScenario : PipelineTestHelper
{
    public RetryOnErrorScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_rerun_step_after_first_failure()
    {
        _ = await StartPipeline(new RetryOnErrorWorkflow());

        Assert.Equal(1, RetryOnErrorWorkflow.Step1Ticker);
        Assert.Equal(2, RetryOnErrorWorkflow.Step2Ticker);
    }
}