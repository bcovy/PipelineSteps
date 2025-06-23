using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;

namespace PipelineSteps.Integration.Scenarios;

public class BasicModel : IPipeline<ModelData>
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;

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
        return CreatePipeline.Builder(new ModelData())
            .StartWith<Step1>()
            .AddStep<Step2>()
            .Compile();
    }
}

public class BasicModelWorkflow : PipelineTestHelper
{
    public BasicModelWorkflow()
    {
        Setup();
    }

    [Fact]
    public async Task Can_run_basic_workflow()
    {
        var result = await StartPipeline(new BasicModel());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, BasicModel.Step1Ticker);
        Assert.Equal(1, BasicModel.Step2Ticker);
    }
}