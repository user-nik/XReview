# RoutingSlip

A minimal implementation of the “Routing Slip” pattern. You have a list of tasks, each with two methods:

- **`ExecuteAsync()`** – performs the main action.
- **`RollbackAsync()`** – undoes it if needed.

## Overview

- **`IRoutingSlipTask`** — interface for tasks.
- **`RoutingSlip`** — executes tasks in order; if one fails, previously executed tasks are rolled back in reverse.
- **`TestTask`** — sample task for testing (can simulate errors).
- **`RoutingSlipTests`** — NUnit tests covering success/failure/rollback scenarios.

## Example

```csharp
var routingSlip = new RoutingSlip();
routingSlip.AddTask(new TestTask("Task1"));
routingSlip.AddTask(new TestTask("Task2"));

// Execute
try
{
    await routingSlip.ExecuteAsync();
    // All tasks succeeded
}
catch (InvalidOperationException ex)
{
    // A task failed; completed tasks were rolled back
}
```