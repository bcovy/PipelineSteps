using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Integration.Steps;

public class MyStep2 : IStepBody
{
    public string Name => "MyStep2";
    public string Input1 { get; set; }
    public string Output1 { get; set; }

    public async Task<IStepResult> RunAsync()
    {
        await Task.CompletedTask;

        return new StepResult();
    }
}