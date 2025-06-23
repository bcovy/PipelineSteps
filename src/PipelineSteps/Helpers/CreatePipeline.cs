using PipelineSteps.Builders;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;

namespace PipelineSteps.Helpers;

public static class CreatePipeline
{
    /// <summary>
    /// Returns default implementation of <see cref="IPipelineBuilder{TData}"/> workflow pipeline builder, using <see cref="DefaultModel"/> as the data model type.
    /// </summary>
    /// <param name="workflowName">Workflow name.</param>
    /// <returns>Default implementation of <see cref="IPipelineBuilder{TData}"/> workflow pipeline builder.</returns>
    public static IPipelineBuilder<DefaultModel> Builder(string workflowName = "Workflow") => new PipelineBuilder<DefaultModel>(new DefaultModel(), workflowName);
    /// <summary>
    /// Returns <see cref="IPipelineBuilder{TData}"/> workflow pipeline builder.
    /// </summary>
    /// <typeparam name="TData">Class data model type.</typeparam>
    /// <param name="data">Data model instance.</param>
    /// <param name="workflowName">Workflow name.</param>
    /// <returns><see cref="IPipelineBuilder{TData}"/> workflow pipeline builder.</returns>
    public static IPipelineBuilder<TData> Builder<TData>(TData data, string workflowName = "Workflow") where TData : class => new PipelineBuilder<TData>(data, workflowName);
}