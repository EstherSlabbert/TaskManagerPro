using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            var taskManager = new TaskManager();
            TaskManager.CurrentTaskId = 0;
            TaskBase task = new TaskBase(1, "Sample task", "Sample description", DateTime.Now);

            // Act
            taskManager.AddTask(task);

            // Assert
            Assert.IsTrue(TaskManager.Tasks.Contains(task));
        }

        [TestMethod]
        public void ValidatedUserDateInput_ParsesValidDate()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            mockUserInterface.Setup(ui => ui.ReadLine()).Returns("2023-10-20");

            // Act
            DateTime result = TaskManager.ValidatedUserDateInput(mockUserInterface.Object);

            // Assert
            Assert.AreEqual(new DateTime(2023, 10, 20), result);
        }

        [TestMethod]
        public void ValidatedUserDateInput_InvalidInputRetries()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            mockUserInterface.SetupSequence(ui => ui.ReadLine())
                .Returns("invalid")
                .Returns("2023-10-20");

            // Act
            DateTime result = TaskManager.ValidatedUserDateInput(mockUserInterface.Object);

            // Assert
            Assert.AreEqual(new DateTime(2023, 10, 20), result);
        }

        [TestMethod]
        public void NewTask_CreatesTask()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            mockUserInterface.SetupSequence(ui => ui.ReadLine())
            .Returns("Sample Title")
            .Returns("Sample Description")
            .Returns("2023-10-12");

            var taskManager = new TaskManager(mockUserInterface.Object);

            // Act
            var newTask = taskManager.NewTask();

            // Assert
            Assert.AreEqual(1, newTask.TaskId);
            Assert.AreEqual("Sample Title", newTask.TaskTitle);
            Assert.AreEqual("Sample Description", newTask.TaskDescription);
            Assert.AreEqual(DateTime.Parse("2023-10-12"), newTask.DueDate);
        }
    }
}
