using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Enums;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Middleware;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;
using static PipelineSteps.Integration.Scenarios.IfSetNameChangeWorkflow;

namespace PipelineSteps.Integration.Scenarios;

public class IfSetNameChangeWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int MiddleBeforeTicker = 0;
    internal static int MiddleAfterTicker = 0;
    internal static bool ifNameCalled;

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public class MiddleStep : IStepMiddleware
    {
        public async Task InvokeAsync(IPipelineStep workflowStep, string stepBodyName, IStepResult result, NextMiddleware next)
        {
            if (result.Result is StepResultType.IfCondition)
                ifNameCalled = true;

            MiddleBeforeTicker += 1;
            await next();
            MiddleAfterTicker += 1;
        }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWith<Step1>()
            .If(true).SetStepName("hello world").Do(d => d.StartWith<Step2>())
            .Compile(useStepMiddleware: true);
    }
}
public class IfSetNameChangeScenario : PipelineTestHelper
{
    public IfSetNameChangeScenario()
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
        var result = await StartPipeline(new IfSetNameChangeWorkflow());

        Assert.True(result.IsSuccess);

        Assert.Equal(1, Step1Ticker);
        Assert.Equal(1, Step2Ticker);
        Assert.Equal(3, MiddleBeforeTicker);
        Assert.Equal(3, MiddleAfterTicker);
        Assert.True(ifNameCalled);
    }
}
