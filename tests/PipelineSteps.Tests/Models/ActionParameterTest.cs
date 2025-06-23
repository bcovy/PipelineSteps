using PipelineSteps.Models;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Models
{
    public class ActionParameterTest
    {
        [Fact]
        public void Should_assign_input_from_object()
        {
            Action<MockStep, MyData> action = (x, y) => x.Input1 = "hello world";
            var feature = new ActionParameter<MockStep, MyData>(action);
            MockStep step = new();
            MyData data = new();

            feature.AssignInput(data, step);

            Assert.Equal("hello world", step.Input1);
        }

        class MyData
        {
            public string Value2 { get; set; }
        }
    }
}