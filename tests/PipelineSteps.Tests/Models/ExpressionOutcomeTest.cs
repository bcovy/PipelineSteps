using System.Linq.Expressions;
using NSubstitute;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Models;

public class ExpressionOutcomeTest
{
    private readonly MockData data;
    private readonly IBranchDefinition branchDefinition;

    public ExpressionOutcomeTest()
    {
        data = new();
        branchDefinition = Substitute.For<IBranchDefinition>();
    }

    [Fact]
    public void Matches_when_expression_returns_a_true_result()
    {
        Expression<Func<MockData, object, bool>> expression = (data, outcome) => data.Input2 == 12;
        ExpressionOutcome<MockData> feature = new(branchDefinition, expression);

        data.Input2 = 12;
        bool actual = feature.Matches("hello", data);

        Assert.True(actual);
    }
}