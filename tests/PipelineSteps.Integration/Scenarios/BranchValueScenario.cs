using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.TestHelper;
namespace PipelineSteps.Integration.Scenarios;

public class BranchValueWorkflow : IPipeline<ModelData>
{
    internal static int Step2Ticker = 0;
    internal static int Step3Ticker = 0;
    internal static int Step4Ticker = 0;

    private readonly ModelData _data;

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public class Step3 : BaseStepBody
    {
        public Step3() : base(a => Step3Ticker = a) { }
    }

    public class Step4 : BaseStepBody
    {
        public Step4() : base(a => Step4Ticker = a) { }
    }

    public BranchValueWorkflow(int branchId)
    {
        _data = new ModelData() { BranchOutcome = branchId };
    }

    public IPipelineDefinition<ModelData> Build()
    {
        var builder = CreatePipeline.Builder(_data);
        var branch1 = builder.CreateBranch().StartWith<Step2>();
        var branch2 = builder.CreateBranch().StartWith<Step3>();

        return builder.StartWith<MyStep1>()
            .Decide(a => a.BranchOutcome)
            .Branch(1, branch1)
            .Branch(2, branch2)
            .AddStep<Step4>()
            .Compile();
    }
}

public class BranchValueScenario : PipelineTestHelper
{
    public BranchValueScenario()
    {
        Setup();
    }

    internal async Task Start(int branchId)
    {
        BranchValueWorkflow.Step2Ticker = 0;
        BranchValueWorkflow.Step3Ticker = 0;
        BranchValueWorkflow.Step4Ticker = 0;

        _ = await StartPipeline(new BranchValueWorkflow(branchId));
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(2, 0, 1)]
    [InlineData(3, 0, 0)]
    public async Task Should_run_target_branch(int branchId, int expected1, int expected2)
    {
        await Start(branchId);

        Assert.Equal(expected1, BranchValueWorkflow.Step2Ticker);
        Assert.Equal(expected2, BranchValueWorkflow.Step3Ticker);
        Assert.Equal(1, BranchValueWorkflow.Step4Ticker);
    }
}