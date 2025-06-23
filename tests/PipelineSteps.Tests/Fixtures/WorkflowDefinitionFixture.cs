using NSubstitute;
using PipelineSteps.Definitions;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;
using PipelineSteps.Tests.Mocks;

namespace PipelineSteps.Tests.Fixtures;

public class WorkflowDefinitionFixture
{
    public Dictionary<int, IPipelineStep> Steps { get; private set; }

    public MockData Data { get; init; }

    public WorkflowDefinitionFixture()
    {
        Steps = new Dictionary<int, IPipelineStep>();
        Data = new();
    }

    public WorkflowDefinitionFixture AddStep(int id, int nextStepId)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "next";

        Steps.Add(id, step);

        return this;
    }

    public WorkflowDefinitionFixture AddEventStep(int id, int nextStepId)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "event";
        step.BodyType.Returns(typeof(MockData));

        Steps.Add(id, step);

        return this;
    }

    public WorkflowDefinitionFixture AddEndStep(int id, int nextStepId)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "end";

        Steps.Add(id, step);

        return this;
    }

    public WorkflowDefinitionFixture AddErrorStep(int id, int nextStepId)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "fail";

        Steps.Add(id, step);

        return this;
    }

    public WorkflowDefinitionFixture AddIfStep(int id, int nextStepId, int[] childSteps)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "if";
        step.ChildStepsId = childSteps;

        Steps.Add(id, step);

        return this;
    }

    public WorkflowDefinitionFixture AddBranchStep(int id, int nextStepId)
    {
        IPipelineStep step = Substitute.For<IPipelineStep>();
        IPipelineStep branchStep1 = Substitute.For<IPipelineStep>();
        IPipelineStep branchStep2 = Substitute.For<IPipelineStep>();

        branchStep1.Id = 1;
        branchStep1.NextStepId = 2;
        branchStep1.BodyName = "next";

        branchStep2.Id = 2;
        branchStep2.NextStepId = 0;
        branchStep2.BodyName = "next";

        Dictionary<int, IPipelineStep> branchSteps = new()
        {
            { 1, branchStep1 },
            { 2, branchStep2 }
        };

        ValueOutcome<int> outcome = new(BranchDefinition.Create("branch1", branchSteps), 1);

        step.Id = id;
        step.NextStepId = nextStepId;
        step.BodyName = "branch";
        step.StepBranches.Returns([outcome]);

        Steps.Add(id, step);

        return this;
    }

    public PipelineDefinition<MockData> Build(string name)
    {
        return new PipelineDefinition<MockData>(Data, name, Steps);
    }
}
