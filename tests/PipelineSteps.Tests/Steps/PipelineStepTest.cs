using NSubstitute;
using PipelineSteps.Steps;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Steps
{
    public class PipelineStepTest
    {
        private readonly PipelineStep<MockStep> _feature;
        private readonly IServiceProvider _serviceProvider;

        public PipelineStepTest()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _feature = new PipelineStep<MockStep>();
        }

        [Fact]
        public void ConstructBody_resolves_unregistered_IStepBody()
        {
            _ = Assert.IsType<MockStep>(_feature.ConstructBody(_serviceProvider), exactMatch: false);
        }

        [Fact]
        public void ConstructBody_resolves_registered_IStepBody()
        {
            _serviceProvider.GetService(Arg.Any<Type>()).Returns(new MockStep());

            _ = Assert.IsType<MockStep>(_feature.ConstructBody(_serviceProvider), exactMatch: false);
        }
    }
}