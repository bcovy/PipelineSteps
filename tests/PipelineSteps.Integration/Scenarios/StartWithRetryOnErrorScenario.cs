using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class StartWithRetryOnErrorWorkflow : IPipeline<ModelData>
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
            .StartWith(a =>
            {
                Step2Ticker++;

                if (Step2Ticker <= 1)
                {
                    return StepResult.Fail("Some error");
                }

                return StepResult.Next();
            }).RetryOnError(TimeSpan.FromMilliseconds(1))
            .AddStep<Step1>()
            .Compile();
    }
}

public class StartWithRetryOnErrorScenario : PipelineTestHelper
{
    public StartWithRetryOnErrorScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_rerun_start_step_after_first_failure()
    {
        _ = await StartPipeline(new StartWithRetryOnErrorWorkflow());

        Assert.Equal(1, StartWithRetryOnErrorWorkflow.Step1Ticker);
        Assert.Equal(2, StartWithRetryOnErrorWorkflow.Step2Ticker);
    }
}