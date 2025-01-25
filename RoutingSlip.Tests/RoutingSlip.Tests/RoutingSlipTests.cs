using RoutingSlip.Tests.TestModels;

namespace RoutingSlip.Tests
{
    [TestFixture]
    public class RoutingSlipTests
    {
        /// <summary>
        /// Test checks that all tasks are executed successfully
        /// and that rollback was not triggered.
        /// </summary>
        [Test]
        public async Task Execute_AllTasks_Success()
        {
            // Arrange
            var routingSlip = new RoutingSlip();

            var task1 = new TestTask("Task1");
            var task2 = new TestTask("Task2");
            var task3 = new TestTask("Task3");

            routingSlip.AddTask(task1);
            routingSlip.AddTask(task2);
            routingSlip.AddTask(task3);

            // Act
            await routingSlip.ExecuteAsync();

            // Assert
            Assert.IsTrue(task1.Executed);
            Assert.IsTrue(task2.Executed);
            Assert.IsTrue(task3.Executed);

            Assert.IsFalse(task1.RolledBack);
            Assert.IsFalse(task2.RolledBack);
            Assert.IsFalse(task3.RolledBack);
        }

        /// <summary>
        /// Test checks that if one of the tasks "fails",
        /// all previously executed tasks are rolled back.
        /// </summary>
        [Test]
        public async Task Execute_TaskFails_RollbackPreviousTasks()
        {
            // Arrange
            var routingSlip = new RoutingSlip();

            var task1 = new TestTask("Task1");
            var task2WithError = new TestTask("Task2WithError", throwErrorOnExecute: true);
            var task3 = new TestTask("Task3");

            routingSlip.AddTask(task1);
            routingSlip.AddTask(task2WithError);
            routingSlip.AddTask(task3);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await routingSlip.ExecuteAsync();
            });

            Assert.That(ex, Is.Not.Null);

            Assert.IsTrue(task1.Executed);
            Assert.IsTrue(task1.RolledBack);

            Assert.IsTrue(task2WithError.Executed);
            Assert.IsFalse(task2WithError.RolledBack);

            Assert.IsFalse(task3.Executed);
            Assert.IsFalse(task3.RolledBack);
        }

        /// <summary>
        /// Test checks a situation where rolling back one of the tasks
        /// encounters an error, but other tasks are still rolled back.
        /// </summary>
        [Test]
        public async Task Execute_RollbackFails_ShouldContinueRollback()
        {
            // Arrange
            var routingSlip = new RoutingSlip();

            var task1RollbackError = new TestTask("Task1") 
            { 
                ThrowErrorOnRollback = true 
            };

            var task2 = new TestTask("Task2");

            var failingTask = new TestTask("FailingTask", throwErrorOnExecute: true);

            routingSlip.AddTask(task1RollbackError);
            routingSlip.AddTask(task2);
            routingSlip.AddTask(failingTask);

            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await routingSlip.ExecuteAsync();
            });
            Assert.That(ex, Is.Not.Null);

            // Assert
            Assert.IsTrue(task1RollbackError.Executed);
            Assert.IsTrue(task2.Executed);

            Assert.IsTrue(task1RollbackError.RolledBack);
            Assert.IsTrue(task2.RolledBack);

            Assert.IsTrue(failingTask.Executed);
            Assert.IsFalse(failingTask.RolledBack);
        }
    }
}
