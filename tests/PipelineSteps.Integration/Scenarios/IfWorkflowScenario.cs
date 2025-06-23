using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class IfWorkflow : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int Step3Ticker = 0;

    private readonly ModelData _data;
    private readonly ModelData2 _data2;

    public class Step1 : BaseStepBody
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

    public IfWorkflow(int model1Input, int model2Input)
    {
        _data = new() { Input2 = model1Input };
        _data2 = new() { Input2 = model2Input };
    }

    public IPipelineDefinition<ModelData> Build()
    {
        return CreatePipeline.Builder(_data)
            .StartWith<MyStep3>()
            .If(a => a.Input2 == 99)
                .Do(d => d.StartWith<Step1>().Input(a => a.InputValue, i => i.Input2))
            .If(_data2, a => a.Input2 == 99)
                .Do(d => d.StartWith<Step2>().Input(a => a.InputValue, i => i.Input2))
            .AddStep<Step3>()
            .Compile();
    }
}

public class IfWorkflowScenario : PipelineTestHelper
{
    public IfWorkflowScenario()
    {
        Setup();
    }

    [Fact]
    public async Task Should_run_multiple_true_if_conditions()
    {
        var result = await StartPipeline(new IfWorkflow(99, 99));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, IfWorkflow.Step1Ticker);
        Assert.Equal(99, IfWorkflow.Step2Ticker);
        Assert.Equal(1, IfWorkflow.Step3Ticker);
    }

    [Fact]
    public async Task Should_run_2_steps_when_an_if_condition_is_false()
    {
        var result = await StartPipeline(new IfWorkflow(99, 0));

        Assert.True(result.IsSuccess);
        Assert.Equal(99, IfWorkflow.Step1Ticker);
        Assert.Equal(0, IfWorkflow.Step2Ticker);
        Assert.Equal(1, IfWorkflow.Step3Ticker);
    }
}