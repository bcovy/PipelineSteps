using NSubstitute;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;
using PipelineSteps.Services;

namespace PipelineSteps.Tests.Fixtures;

public static class PipelineFactory
{
    public static PipelineController Create(StepExecutorFixture stepExecutorFixture)
    {
        return new PipelineController(Substitute.For<IMiddlewareController>(), stepExecutorFixture, new PipelineOptions());
    }
}
