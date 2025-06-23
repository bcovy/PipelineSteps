using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class StepInlineWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWith<MyStep3>()
            .InlineStep(a =>
            {
                Step1Ticker = 1;

                return StepResult.Next();
            })
            .AddStep<Step2>()
            .Compile();
    }
}

public class InlineStepScenario : PipelineTestHelper
{
    public InlineStepScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_execute_user_inline_step()
    {
        _ = await StartPipeline(new StepInlineWorkflow());

        Assert.Equal(1, StepInlineWorkflow.Step1Ticker);
        Assert.Equal(1, StepInlineWorkflow.Step2Ticker);
    }
}