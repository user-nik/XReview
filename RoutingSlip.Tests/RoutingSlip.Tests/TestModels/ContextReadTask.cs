using RoutingSlip;

public class ContextReadTask(string taskName, string key) : IRoutingSlipTask
{
    public bool Executed { get; private set; }
    public bool RolledBack { get; private set; }

    public object? ReadValue { get; private set; }

    public async Task ExecuteAsync(IDictionary<string, object> context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        if (context.TryGetValue(key, out var value))
        {
            ReadValue = value;
            Console.WriteLine($"{taskName} read [{key}] = {value}");
        }
        else
        {
            Console.WriteLine($"{taskName} did not find key '{key}' in context.");
        }

        Executed = true;
    }

    public async Task RollbackAsync(IDictionary<string, object> context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(30, cancellationToken);

        RolledBack = true;
    }
}