using Microsoft.Extensions.DependencyInjection;
using PipelineSteps.Enums;
using PipelineSteps.Helpers;
using PipelineSteps.Integration.Fixtures;
using PipelineSteps.Interfaces;
using PipelineSteps.Middleware;
using PipelineSteps.Models;
using PipelineSteps.TestHelper;
using static PipelineSteps.Integration.Scenarios.PreAndPostMiddlewareWorkflow;

namespace PipelineSteps.Integration.Scenarios;

public class PreAndPostMiddlewareWorkflow : IPipeline
{
    internal static int Step1Ticker = 0;
    internal static int Step2Ticker = 0;
    internal static int PreBeforeTicker = 0;
    internal static int PreAfterTicker = 0;
    internal static int PostBeforeTicker = 0;
    internal static int PostAfterTicker = 0;

    public class Step1 : BaseStepBody
    {
        public Step1() : base(a => Step1Ticker = a) { }
    }

    public class Step2 : BaseStepBody
    {
        public Step2() : base(a => Step2Ticker = a) { }
    }

    public class PreMiddle1 : IPipelineMiddleware
    {
        public MiddlewarePhase Phase { get; set; } = MiddlewarePhase.PreWorkflow;

        public async Task InvokeAsync(IPipelineResult context, NextMiddleware next)
        {
            PreBeforeTicker = 1;
            await next();
            PreAfterTicker = 1;
        }
    }

    public class PostMiddle1 : IPipelineMiddleware
    {
        public MiddlewarePhase Phase { get; set; } = MiddlewarePhase.PreWorkflow;

        public async Task InvokeAsync(IPipelineResult context, NextMiddleware next)
        {
            PostBeforeTicker = 1;
            await next();
            PostAfterTicker = 1;
        }
    }

    public IPipelineDefinition<DefaultModel> Build()
    {
        return CreatePipeline.Builder()
            .StartWith<Step1>()
            .AddStep<Step2>()
            .Compile(usePreWorkflowMiddleware: true, usePostWorkflowMiddleware: true);
    }
}

public class PreAndPostMiddlewareScenario : PipelineTestHelper
{
    public PreAndPostMiddlewareScenario()
    {
        Setup();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddWorkflowMiddleware<PreMiddle1>();
        services.AddWorkflowMiddleware<PostMiddle1>();
    }

    [Fact]
    public async Task Should_run_middleware_pre_and_post_workflow()
    {
        var result = await StartPipeline(new PreAndPostMiddlewareWorkflow());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, Step1Ticker);
        Assert.Equal(1, Step2Ticker);
        Assert.Equal(1, PreBeforeTicker);
        Assert.Equal(1, PreAfterTicker);
        Assert.Equal(1, PostBeforeTicker);
        Assert.Equal(1, PostAfterTicker);
    }
}