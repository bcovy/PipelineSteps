using NSubstitute;
using PipelineSteps.Builders;
using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Steps;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Builders;

public class PipelineBuilderTest
{
    private readonly PipelineBuilder<MockData> _feature;
    private readonly IPipelineStep _workflowStep;

    public PipelineBuilderTest()
    {
        _feature = new PipelineBuilder<MockData>(new MockData());
        _workflowStep = Substitute.For<IPipelineStep>();
    }

    [Fact]
    public void AddStep_should_add_step_and_update_id()
    {
        _feature.AddStep(_workflowStep);

        Assert.True(_feature.Steps.Any());
        Assert.Equal(1, _feature.Steps[1].Id);
    }

    [Fact]
    public void StartWith_with_adds_initial_step()
    {
        var actual = _feature.StartWith<MockStep>();

        Assert.IsType<PipelineStep<MockStep>>(actual.Step, exactMatch: false);
        Assert.Single(_feature.Steps);
        Assert.Equal(1, _feature.LastStartWithStep);
    }

    [Fact]
    public void StartWith_with_input_function_should_add_initial_step_of_type_T()
    {
        string value = "hello world";
        var actual = _feature.StartWith<MockStep, string>(a => a.Input1, value);

        Assert.IsType<PipelineStep<MockStep, string>>(actual.Step, exactMatch: false);
        Assert.Single(_feature.Steps);
        Assert.Equal(1, _feature.LastStartWithStep);
    }

    [Fact]
    public void StartWith_with_inline_step()
    {
        var actual = _feature.StartWith(a => StepResult.Next());

        Assert.IsType<PipelineInlineStep<MockData>>(actual.Step, exactMatch: false);
    }
}