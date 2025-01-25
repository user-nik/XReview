using RoutingSlip;

public class ContextWriteTask(string taskName, string key, object value) : IRoutingSlipTask
{
    private readonly string _taskName = taskName;

    public bool Executed { get; private set; }
    public bool RolledBack { get; private set; }

    public async Task ExecuteAsync(IDictionary<string, object> context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        context[key] = value;

        Executed = true;
    }

    public async Task RollbackAsync(IDictionary<string, object> context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(30, cancellationToken);

        if (context.ContainsKey(key))
            context.Remove(key);

        RolledBack = true;
    }
}