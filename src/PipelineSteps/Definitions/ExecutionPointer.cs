namespace PipelineSteps.Definitions;
/// <summary>
/// Tracks the next step to be executed.  Represents a last-in-first-out stack.
/// </summary>
public class ExecutionPointer
{
    private readonly Stack<int> _stack;

    public ExecutionPointer(int startingId)
    {
        _stack = new Stack<int>();
        _stack.Push(startingId);
    }
    /// <summary>
    /// Set the next step ID to be executed. 
    /// </summary>
    /// <param name="id"></param>
    public void SetNextId(int id)
    {
        if (id != 0)
            _stack.Push(id);
    }
    /// <summary>
    /// Returns the next step ID to be executed.
    /// </summary>
    /// <returns>ID of next step to execute.</returns>
    public int GetNextId() => _stack.TryPop(out int result) ? result : 0;
}
