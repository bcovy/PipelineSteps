using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class InlineWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public IPipelineDefinition<ModelData> Build()
    {
        ModelData data = new();

        return CreatePipeline.Builder(data)
            .StartWith(a =>
            {
                Step1Ticker = data.Input2;

                return StepResult.Next();
            })
            .Input((step, data) => step.Data.Input2 = 99)
            .AddStep<MyStep3>()
            .Compile();
    }
}

public class StartWithInlineScenario : PipelineTestHelper
{
    public StartWithInlineScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Can_start_with_inline_step()
    {
        _ = await StartPipeline(new InlineWorkflow());

        Assert.Equal(99, InlineWorkflow.Step1Ticker);
    }
}