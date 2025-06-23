using System.Linq.Expressions;
using NSubstitute;
using PipelineSteps.Logic;
using PipelineSteps.Steps;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Steps;

public class PipelineStepTTest
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineStepTTest()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
    }
    
    [Fact]
    public void ConstructBody_resolves_unregistered_IStepBody()
    {
        var feature = new PipelineStep<MockStep, string>()
        {
            InputExpression = (Expression<Func<MockStep, string>>)((a) => a.Input1),
            InputValue = "hello world!!"
        };

        _ = Assert.IsType<MockStep>(feature.ConstructBody(_serviceProvider), exactMatch: false);
    }

    [Fact]
    public void ConstructBody_resolves_registered_IStepBody()
    {
        var feature = new PipelineStep<MockStep, string>()
        {
            InputExpression = (Expression<Func<MockStep, string>>)((a) => a.Input1),
            InputValue = "hello world!!"
        };

        _serviceProvider.GetService(Arg.Any<Type>()).Returns(new MockStep());

        _ = Assert.IsType<MockStep>(feature.ConstructBody(_serviceProvider), exactMatch: false);
    }

    [Fact]
    public void ConstructBody_assigns_string_input_parameter_value()
    {
        _serviceProvider.GetService(Arg.Any<Type>()).Returns(new MockStep());
        string value = "hello world!!";
        Expression<Func<MockStep, string>> target = (a) => a.Input1;

        var feature = new PipelineStep<MockStep, string>()
        {
            InputExpression = target,
            InputValue = value
        };
        var actual = Assert.IsType<MockStep>(feature.ConstructBody(_serviceProvider), exactMatch: false);

        Assert.Equal(value, actual.Input1);
    }

    [Fact]
    public void AssignValue_should_assign_associated_member_value()
    {
        MockStep input = new();
        var feature = new PipelineStep<MockStep, string>()
        {
            InputExpression = (Expression<Func<MockStep, string>>)((a) => a.Input1),
            InputValue = "hello world!!"
        };

        feature.AssignValue(input, feature.InputExpression, feature.InputValue);

        Assert.Equal(feature.InputValue, input.Input1);
    }

    [Fact]
    public void AssignValue_should_assign_associated_if_logic_step_result()
    {
        IfStep step = new();
        PipelineStep<IfStep, bool> subject = new();
        Expression<Func<IfStep, bool>> inputExpr = x => x.Condition;

        subject.AssignValue(step, inputExpr, true);

        Assert.True(step.Condition);
    }
}