namespace RoutingSlip;

/// <summary>
/// Interface for a task in a routing slip.
/// Any task must be able to execute and roll back.
/// </summary>
public interface IRoutingSlipTask
{
    /// <summary>
    /// Main task logic.
    /// </summary>
    Task ExecuteAsync(IDictionary<string, object> context, CancellationToken token);

    /// <summary>
    /// Actions to cancel or compensate if something goes wrong.
    /// </summary>
    Task RollbackAsync(IDictionary<string, object> context, CancellationToken token);
}