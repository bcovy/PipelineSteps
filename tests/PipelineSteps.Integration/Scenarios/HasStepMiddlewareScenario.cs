using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Middleware;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;
using static PipelineSteps.Integration.Scenarios.HasStepMiddlewareWorkflow;

namespace PipelineSteps.Integration.Scenarios;

public class HasStepMiddlewareWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int MiddleBeforeTicker = 0;
    internal static int MiddleAfterTicker = 0;

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
            MiddleBeforeTicker += 1;
            await next();
            MiddleAfterTicker += 1;
        }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWith<Step1>()
            .AddStep<Step2>()
            .Compile(useStepMiddleware: true);
    }
}

public class HasStepMiddlewareScenario : PipelineTestHelper
{
    public HasStepMiddlewareScenario()
    {
        Setup();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddStepMiddleware<MiddleStep>();
    }

    [Fact]
    public async Task Should_run_middleware_after_step_execution()
    {
        var result = await StartPipeline(new HasStepMiddlewareWorkflow());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, Step1Ticker);
        Assert.Equal(1, Step2Ticker);
        Assert.Equal(2, MiddleBeforeTicker);
        Assert.Equal(2, MiddleAfterTicker);
    }
}