using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;

namespace PipelineSteps.Integration.Fixtures
{
    public abstract class BaseStepBody : IStepBody
    {
        private readonly Action<int> _ticker;

        public int InputValue { get; set; }

        public string Name => "Base step";

        protected BaseStepBody(Action<int> ticker, int inputValue = 1)
        {
            _ticker = ticker;
            InputValue = inputValue;
        }

        public async Task<IStepResult> RunAsync()
        {
            _ticker.Invoke(InputValue);
            return await Task.FromResult(StepResult.Next());
        }
    }
}