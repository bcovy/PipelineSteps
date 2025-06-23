using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class InputOutputWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

    public string Id => "InputOutputWorkflow";

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public IPipelineDefinition<ModelData> Build()
    {
        ModelData data = new() { Input2 = 99 };

        return CreatePipeline.Builder(data)
            .StartWith<Step1>()
                .Input(step => step.InputValue, i => i.Input2)
                .Output(data => data.Output2, step => step.InputValue)
            .AddStep<Step2>()
                .Input(step => step.InputValue, i => i.Output2)
            .Compile();
    }
}

public class StepInputOutputScenario : PipelineTestHelper
{
    public StepInputOutputScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_assign_input_and_output_values_for_each_step()
    {
        _ = await StartPipeline(new InputOutputWorkflow());

        Assert.Equal(99, InputOutputWorkflow.Step1Ticker);
        Assert.Equal(99, InputOutputWorkflow.Step2Ticker);
    }
}