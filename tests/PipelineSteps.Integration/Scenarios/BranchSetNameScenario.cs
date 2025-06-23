using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Enums;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Steps;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Middleware;
using PipelineSteps.TestHelper;
using static PipelineSteps.Integration.Scenarios.BranchSetNameWorkflow;

namespace PipelineSteps.Integration.Scenarios;

public class BranchSetNameWorkflow : IPipeline<ModelData>
{
    internal static int Step2Ticker = 0;
    internal static bool decideNameCalled;

    private readonly ModelData _data;

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public class MiddleStep : IStepMiddleware
    {
        public async Task InvokeAsync(IPipelineStep workflowStep, string stepBodyName, IStepResult result, NextMiddleware next)
        {
            if (result.Result is StepResultType.Branch)
                decideNameCalled = true;

            await next();
        }
    }

    public BranchSetNameWorkflow(int branchId)
    {
        _data = new ModelData() { BranchOutcome = branchId };
    }

    public IPipelineDefinition<ModelData> Build()
    {
        var builder = CreatePipeline.Builder(_data);
        var branch1 = builder.CreateBranch().StartWith<Step2>();

        return builder.StartWith<MyStep1>()
            .Decide(a => a.BranchOutcome)
            .SetStepName("hello world")
            .Branch(1, branch1)
            .Compile(useStepMiddleware: true);
    }
}

public class BranchSetNameScenario : PipelineTestHelper
{
    public BranchSetNameScenario()
    {
        Setup();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddStepMiddleware<MiddleStep>();
    }

    [Fact]
    public async Task If_step_name_has_been_updated_from_default_value()
    {
        var result = await StartPipeline(new BranchSetNameWorkflow(1));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, Step2Ticker);
        Assert.True(decideNameCalled);
    }
}
