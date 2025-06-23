namespace PipelineSteps.Infrastructure;

public static class BranchIdGenerator
{
    private static readonly Random Rnd = new();

    public static int NewId => Rnd.Next(1, 1265);
}
