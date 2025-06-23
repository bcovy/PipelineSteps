using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class DoLastWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public IPipelineDefinition<ModelData> Build()
    {
        ModelData data = new() { Input2 = 99 };

        return CreatePipeline.Builder(data)
            .StartWith<MyStep3>()
                .Input(step => step.Input1, i => i.Input1)
            .If(a => a.Input2 == 99)
                .Do(d => d.StartWith<Step1>())
            .Compile();
    }
}

public class DoIsLastStepScenario : PipelineTestHelper
{
    public DoIsLastStepScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_run_if_step_and_return_success_when_do_is_last_step()
    {
        _ = await StartPipeline(new DoLastWorkflow());

        Assert.Equal(1, DoLastWorkflow.Step1Ticker);
    }
}