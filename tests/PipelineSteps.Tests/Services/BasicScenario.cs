﻿using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class BasicScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public BasicScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_execute_steps_in_sequential_order()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 2).AddStep(2, 3).AddStep(3, 0).Build("basic");

        _ = await _feature.Execute(definition);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(3, _stepExecutor.CallOrder[2]);
    }
}
