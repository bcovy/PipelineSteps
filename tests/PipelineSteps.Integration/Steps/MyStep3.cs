using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Integration.Steps
{
    public class MyStep3 : IStepBody
    {
        public string Name => "Fixture step 1.";
        public string Input1 { get; set; }
        public int Input2 { get; set; }
        public string Output1 { get; set; }

        public async Task<IStepResult> RunAsync()
        {
            await Task.CompletedTask;

            return new StepResult();
        }
    }
}