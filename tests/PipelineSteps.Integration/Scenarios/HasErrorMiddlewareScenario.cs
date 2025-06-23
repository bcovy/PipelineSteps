using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Enums;
using PipelineSteps.Helpers;
using PipelineSteps.Infrastructure;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Integration.Mocks;
using PipelineSteps.Interfaces;
using PipelineSteps.Middleware;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;
using static PipelineSteps.Integration.Scenarios.HasErrorMiddlewareWorkflow;

namespace PipelineSteps.Integration.Scenarios;

public class HasErrorMiddlewareWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int MiddleBeforeTicker = 0;
    internal static int MiddleAfterTicker = 0;
    internal static int PreBeforeTicker = 0;
    internal static int PreAfterTicker = 0;

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public class Step2 : IStepBody
    {
        public string Name => "Step 2";

        public async Task<IStepResult> RunAsync()
        {
            Step2Ticker = 1;
            return await Task.FromResult(StepResult.Fail("Step 2 failure."));
        }
    }

    public class MiddleError : IPipelineMiddleware
    {
        public MiddlewarePhase Phase { get; set; } = MiddlewarePhase.Error;

        public async Task InvokeAsync(IPipelineResult context, NextMiddleware next)
        {
            MiddleBeforeTicker = 1;
            await next();
            MiddleAfterTicker = 1;
        }
    }

    public class PreMiddle : IPipelineMiddleware
    {
        public MiddlewarePhase Phase { get; set; } = MiddlewarePhase.PreWorkflow;

        public async Task InvokeAsync(IPipelineResult context, NextMiddleware next)
        {
            PreBeforeTicker = 1;
            await next();
            PreAfterTicker = 1;
        }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWith<Step1>()
            .AddStep<Step2>()
            .Compile(true);
    }
}

public class HasErrorMiddlewareScenario : PipelineTestHelper
{
    public HasErrorMiddlewareScenario()
    {
        Setup();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddWorkflowMiddleware<MiddleError1>();
        services.AddWorkflowMiddleware<MiddleError>();
        services.AddWorkflowMiddleware<PreMiddle>();
    }

    [Fact]
    public async Task Should_run_middleware_after_error_step()
    {
        var result = await StartPipeline(new HasErrorMiddlewareWorkflow());

        Assert.False(result.IsSuccess);
        Assert.Equal(1, Step1Ticker);
        Assert.Equal(1, Step2Ticker);
        Assert.Equal(1, MiddleBeforeTicker);
        Assert.Equal(1, MiddleAfterTicker);
        Assert.Equal(0, PreBeforeTicker);
        Assert.Equal(0, PreAfterTicker);
    }
}