using PipelineSteps.Definitions;
using PipelineSteps.Services;
using PipelineSteps.Tests.Fixtures;

namespace PipelineSteps.Tests.Services;

public class BranchScenario
{
    private readonly PipelineController _feature;
    private readonly StepExecutorFixture _stepExecutor;

    public BranchScenario()
    {
        _stepExecutor = new StepExecutorFixture();
        _feature = PipelineFactory.Create(_stepExecutor);
    }

    [Fact]
    public async Task Should_execute_branch_and_return_to_parent()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 2).AddBranchStep(2, 3).AddStep(3, 0).Build("branch");

        _ = await _feature.Execute(definition);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(1, _stepExecutor.CallOrder[2]);
        Assert.Equal(2, _stepExecutor.CallOrder[3]);
        Assert.Equal(3, _stepExecutor.CallOrder[4]);
    }

    [Fact]
    public async Task ExecuteBranch_should_execute_child_if_steps()
    {
        WorkflowDefinitionFixture fixture = new();

        var definition = fixture.AddIfStep(1, 4, [2, 3]).AddStep(2, 3).AddStep(3, 0).AddStep(4, 0).Build("if");
        PipelineExecutionContext context = new(definition.Name, definition.ContextId);
        var branch = BranchDefinition.Create("branch1", definition.Steps);

        _ = await _feature.ExecuteBranch(branch, fixture.Data, context);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(2, _stepExecutor.CallOrder[1]);
        Assert.Equal(3, _stepExecutor.CallOrder[2]);
        Assert.Equal(4, _stepExecutor.CallOrder[3]);
    }

    [Fact]
    public async Task ExecuteBranch_should_skip_child_steps_when_if_condition_result_is_not_returned()
    {
        WorkflowDefinitionFixture fixture = new();
        var definition = fixture.AddStep(1, 4).AddStep(2, 3).AddStep(3, 0).AddStep(4, 0).Build("if");
        PipelineExecutionContext context = new(definition.Name, definition.ContextId);
        var branch = BranchDefinition.Create("branch1", definition.Steps);

        _ = await _feature.ExecuteBranch(branch, fixture.Data, context);

        Assert.Equal(1, _stepExecutor.CallOrder[0]);
        Assert.Equal(4, _stepExecutor.CallOrder[1]);
    }
}
