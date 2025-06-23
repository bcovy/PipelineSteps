using PipelineSteps.Infrastructure;
using PipelineSteps.Interfaces;
using PipelineSteps.Models;

namespace PipelineSteps.Integration.Fixtures
{
    public abstract class BaseSyncStepBody : StepBody
    {
        private readonly Action<int> _ticker;

        public int InputValue { get; set; }

        protected BaseSyncStepBody(Action<int> ticker, int inputValue = 1)
        {
            _ticker = ticker;
            InputValue = inputValue;
            Name = "Base sync step";
        }

        public override IStepResult Run()
        {
            _ticker.Invoke(InputValue);

            return StepResult.Next();
        }
    }
}