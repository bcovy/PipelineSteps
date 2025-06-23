using PipelineSteps.Interfaces;

namespace PipelineSteps.Logic;

public class InlineStep<TData>(Func<TData, IStepResult> body, TData data) : IStepBody where TData : class
{
    public TData Data { get; set; } = data;
    public string Name => "Inline step";

    public async Task<IStepResult> RunAsync()
    {
        return await Task.FromResult(body(Data));
    }
}