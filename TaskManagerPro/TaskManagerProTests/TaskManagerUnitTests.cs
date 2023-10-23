using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TaskManagerPro;

namespace TaskManagerProTests
{
    [TestClass]
    public class TaskManagerUnitTests
    {
        [TestMethod]
        public void AddTask_ShouldAddTaskToTaskList()
        {
            // Arrange
            TaskManager taskManager = new TaskManager();
            taskManager.CurrentTaskId = 0;
            TaskBase task = new TaskBase(1, "Sample task", "Sample description", DateTime.Now);

            // Act
            taskManager.AddTask(task);

            // Assert
            Assert.IsTrue(taskManager.Tasks.Contains(task));
        }
    }
}
