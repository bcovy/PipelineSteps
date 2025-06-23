using NSubstitute;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Models;

public class ValueOutcomeTest
{
    private readonly MockData _data;
    private readonly IBranchDefinition _branchDefinition;

    public ValueOutcomeTest()
    {
        _data = new();
        _branchDefinition = Substitute.For<IBranchDefinition>();
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(2, false)]
    public void Matches_int_type_outcome(int input, bool expected)
    {
        ValueOutcome<int> feature = new(_branchDefinition, 10);

        bool actual = feature.Matches(input, _data);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void Matches_bool_type_outcome(bool input, bool expected)
    {
        ValueOutcome<bool> feature = new(_branchDefinition, true);

        bool actual = feature.Matches(input, _data);

        Assert.Equal(expected, actual);
    }
}