using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class InlineOutputWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public IPipelineDefinition<ModelData> Build()
    {
        ModelData data = new() { Input2 = 99 };

        return CreatePipeline.Builder(data)
            .StartWith(a =>
            {
                Step1Ticker = data.Input2;
                a.Output2 = 66;

                return StepResult.Next();
            }).Output(data => data.Output2, value => value.Data.Output2)
            .InlineStep(d =>
            {
                Step2Ticker = data.Output2;

                return StepResult.Next();
            })
            .Compile();
    }
}

public class InlineStepWithOutputScenario : PipelineTestHelper
{
    public InlineStepWithOutputScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Start_step_should_apply_output_value_to_ensuing_step()
    {
        _ = await StartPipeline(new InlineOutputWorkflow());

        Assert.Equal(99, InlineOutputWorkflow.Step1Ticker);
        Assert.Equal(66, InlineOutputWorkflow.Step2Ticker);
    }
}