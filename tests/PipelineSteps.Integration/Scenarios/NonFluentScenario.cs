using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class NonFluentWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int Step3Ticker = 0;

    public class Step1 : BaseSyncStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public class Step3 : BaseStepBody
    {
        public Step3() : base(a => Step3Ticker = a) { }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        var step1 = CreatePipeline.Builder().StartWith<Step1>();
        var step2 = step1.AddStep<Step2>();
        var step3 = step2.AddStep<Step3>();

        return step3.Compile();
    }
}

public class NonFluentScenario : PipelineTestHelper
{
    public NonFluentScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Can_add_fluent_and_non_fluent_items()
    {
        var result = await StartPipeline(new NonFluentWorkflow());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, NonFluentWorkflow.Step1Ticker);
        Assert.Equal(1, NonFluentWorkflow.Step2Ticker);
        Assert.Equal(1, NonFluentWorkflow.Step3Ticker);
    }
}