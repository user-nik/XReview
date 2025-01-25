namespace RoutingSlip.Tests.TestModels;

/// <summary>
/// Auxiliary class for testing:
/// records execution and rollback,
/// supports optional failure during execution and rollback.
/// </summary>
internal class TestTask : IRoutingSlipTask
{
    private readonly string _name;
    private readonly bool _throwErrorOnExecute;

    public bool Executed { get; private set; }
    public bool RolledBack { get; private set; }
    public bool ThrowErrorOnRollback { get; set; }

    public TestTask(string name, bool throwErrorOnExecute = false)
    {
        _name = name;
        _throwErrorOnExecute = throwErrorOnExecute;
    }

    public async Task ExecuteAsync()
    {
        Executed = true;
        await Task.Delay(10);

        if (_throwErrorOnExecute)
        {
            throw new Exception($"Error in task {_name}");
        }
    }

    public async Task RollbackAsync()
    {
        RolledBack = true;
        await Task.Delay(10);

        if (ThrowErrorOnRollback)
        {
            throw new Exception($"Rollback error in task {_name}");
        }
    }
}