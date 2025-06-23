using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Tests.Mocks
{
    public class MockStep : IStepBody
    {
        public string Name => "Mock step.";
        public string Input1 { get; set; }

        public async Task<IStepResult> RunAsync()
        {
            await Task.CompletedTask;

            return StepResult.Next();
        }
    }
}