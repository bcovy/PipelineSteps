using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class SynchronousWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public class Step1 : BaseSyncStepBody
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
            .AddStep<Step2>()
            .Compile();
    }
}

public class SynchronousStepScenario : PipelineTestHelper
{
    public SynchronousStepScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Can_run_workflow_with_synchronous_step()
    {
        _ = await StartPipeline(new SynchronousWorkflow());

        Assert.Equal(1, SynchronousWorkflow.Step1Ticker);
        Assert.Equal(1, SynchronousWorkflow.Step2Ticker);
    }
}