namespace RoutingSlip;

/// <summary>
/// RoutingSlip manages a list of tasks and ensures
/// that previously completed tasks will be rolled back in case of an error.
/// </summary>
public class RoutingSlip
{
    private readonly List<IRoutingSlipTask> _tasks = new();

    /// <summary>
    /// Add a task to the queue.
    /// </summary>
    public void AddTask(IRoutingSlipTask task)
    {
        _tasks.Add(task);
    }

    /// <summary>
    /// Execute tasks in sequence.
    /// Roll back completed tasks in case of an error.
    /// </summary>
    public async Task ExecuteAsync(IDictionary<string, object>? context, CancellationToken token)
    {
        context ??= new Dictionary<string, object>();
        
        var executedTasks = new Stack<IRoutingSlipTask>();

        try
        {
            foreach (var task in _tasks)
            {
                token.ThrowIfCancellationRequested();

                await task.ExecuteAsync(context, token);
                executedTasks.Push(task);
            }
        }
        catch (Exception ex)
        {
            while (executedTasks.Count > 0)
            {
                var lastTask = executedTasks.Pop();
                try
                {
                    await lastTask.RollbackAsync(context, token);
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
            }

            throw new InvalidOperationException($"Task execution error {ex.Message}", ex);
        }
    }
}