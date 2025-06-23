using System.Linq.Expressions;
using PipelineSteps.Models;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Models
{
    public class MemberMapParameterTest
    {
        [Fact]
        public void Should_assign_input_from_object()
        {
            Expression<Func<MockStep, string>> memberExpr = (x => x.Input1);
            Expression<Func<MyData, string>> valueExpr = (x => x.Value2);
            var feature = new MemberMapParameter(valueExpr, memberExpr);
            var data = new MyData
            {
                Value2 = "hello world"
            };
            var step = new MockStep();

            feature.AssignInput(data, step);

            Assert.Equal(data.Value2, step.Input1);
        }

        [Fact]
        public void Should_assign_output_to_target_objects_property()
        {
            Expression<Func<MyData, string>> memberExpr = (x => x.Value2);
            Expression<Func<MockStep, string>> valueExpr = (x => x.Input1);
            var subject = new MemberMapParameter(valueExpr, memberExpr);
            var data = new MyData
            {
                Value2 = "goodbye world"
            };

            var step = new MockStep();

            subject.AssignOutput(data, step);

            Assert.Equal(data.Value2, step.Input1);
        }

        class MyData
        {
            public string Value2 { get; set; }
        }
    }
}