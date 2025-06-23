using PipelineSteps.Models;

namespace PipelineSteps.Interfaces;

public interface IPipeline<out TData> where TData : class
{
    public IPipelineDefinition<TData> Build();
}

public interface IPipeline : IPipeline<DefaultModel>;