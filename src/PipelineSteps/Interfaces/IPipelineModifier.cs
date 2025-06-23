using System.Linq.Expressions;

namespace PipelineSteps.Interfaces;

public interface IPipelineModifier<TData, TStepBody> where TData : class where TStepBody : IStepBody
{
    /// <summary>
    /// Sets the name for the modification step.
    /// </summary>
    /// <remarks>
    /// For use when step middleware and/or event persistence options are in use.
    /// </remarks>
    /// <param name="name">Step name.</param>
    /// <returns><typeparamref name="TStepBody"/>step to allow user to chain methods.</returns>
    IPipelineModifier<TData, TStepBody> SetStepName(string name);
    /// <summary>
    /// Branch to execute if decision criteria matches <see cref="IStepBuilder{TData, TStepBody}.Decide(Expression{Func{TData, object}})"/> outcome value.
    /// </summary>
    /// <typeparam name="TStep">Step type.</typeparam>
    /// <typeparam name="TValue">Comparison value of type struct.</typeparam>
    /// <param name="value">Branch execution comparison value.</param>
    /// <param name="branch">Branch workflow.</param>
    /// <returns><typeparamref name="TStepBody"/>step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Branch<TStep, TValue>(TValue value, IStepBuilder<TData, TStep> branch) where TStep : IStepBody where TValue : struct;
    /// <summary>
    /// Branch to execute if decision criteria matches <see cref="IStepBuilder{TData, TStepBody}.Decide(Expression{Func{TData, object}})"/> outcome value.
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <param name="expression">Branch execution comparison expression.</param>
    /// <param name="branch">Branch workflow.</param>
    /// <returns><typeparamref name="TStepBody"/>step to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Branch<TStep>(Expression<Func<TData, object, bool>> expression, IStepBuilder<TData, TStep> branch) where TStep : IStepBody;
    /// <summary>
    /// Will execute the child steps in the <paramref name="builder"/> parameter if the preceding step
    /// <typeparamref name="TStepBody"/> criteria has been met.  Otherwise, the workflow will continue on
    /// the parent branch.
    /// </summary>
    /// <param name="builder">Child step actions.</param>
    /// <returns><typeparamref name="TStepBody"/> step body to allow user to chain methods.</returns>
    IStepBuilder<TData, TStepBody> Do(Action<IPipelineBuilder<TData>> builder);
}