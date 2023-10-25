using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        [TestMethod]
        public void DisplayAllTasks_NoTasks()
        {
            // Arrange
            TaskManager.Tasks.Clear(); // Clear any existing tasks

            // Act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                TaskManager.DisplayAllTasks();
                string result = sw.ToString().Trim(); // Remove leading/trailing whitespace

                // Assert
                Assert.AreEqual("No tasks found. Your task list is empty.", result);
            }
        }

        [TestMethod]
        public void DisplayAllTasks_WithTasks()
        {
            // Arrange
            TaskManager.Tasks = new List<TaskBase>
        {
            new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
            new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
        };

            // Act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                TaskManager.DisplayAllTasks();
                string result = sw.ToString().Trim();

                // Assert
                StringAssert.Contains(result, "Task ID: 1");
                StringAssert.Contains(result, "Task ID: 2");
                StringAssert.Contains(result, "Description: Description 1");
                StringAssert.Contains(result, "Description: Description 2");
                StringAssert.Contains(result, "Completion status: True");
                StringAssert.Contains(result, "Completion status: False");
            }
        }

        [TestMethod]
        public void DisplayTask()
        {
            // Arrange
            TaskBase task = new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true);

            // Act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                TaskManager.DisplayTask(task);
                string result = sw.ToString().Trim();

                // Assert
                StringAssert.Contains(result, "Task ID: 1");
                StringAssert.Contains(result, "Completion status: True");
            }
        }

        [TestMethod]
        public void ValidatedEditSelection_WhenValidChoiceIsEntered_ShouldReturnChoice()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("1");

            TaskManager taskManager = new TaskManager();

            // Act
            string result = TaskManager.ValidatedEditSelection(mockConsole.Object);

            // Assert
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void ValidatedEditSelection_WhenInvalidChoiceIsEntered_ShouldRetry()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("random") // Invalid choice
                .Returns("7")  // Invalid choice
                .Returns("3"); // Valid choice

            TaskManager taskManager = new TaskManager();

            // Act
            string result = TaskManager.ValidatedEditSelection(mockConsole.Object);

            // Assert
            Assert.AreEqual("3", result);
        }

        [TestMethod]
        public void TaskIdExists_WhenTaskExists_ShouldReturnTrue()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;
            // Act
            bool result = TaskManager.TaskIdExists(1);

            // Assert
            Assert.IsTrue(result, "Task with ID 1 should exist.");
        }

        [TestMethod]
        public void TaskIdExists_WhenTaskDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            bool result = TaskManager.TaskIdExists(3);

            // Assert
            Assert.IsFalse(result, "Task with ID 3 should not exist.");
        }

        [TestMethod]
        public void FindTaskById_WhenTaskExists_ShouldReturnTask()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            TaskBase foundTask = TaskManager.FindTaskById(1);

            // Assert
            Assert.IsNotNull(foundTask, "Task with ID 1 should be found.");
            Assert.AreEqual(1, foundTask.TaskId, "Task ID should match.");
        }

        [TestMethod]
        public void FindTaskById_WhenTaskDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            TaskBase foundTask = TaskManager.FindTaskById(3);

            // Assert
            Assert.IsNull(foundTask, "Task with ID 3 should not be found.");
        }

        [TestMethod]
        public void TaskTitleExists_WhenTaskTitleExists_ShouldReturnTrue()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            bool result = TaskManager.TaskTitleExists("Task 1");

            // Assert
            Assert.IsTrue(result, "Task with title 'Task 1' should exist.");
        }

        [TestMethod]
        public void TaskTitleExists_WhenTaskTitleDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            bool result = TaskManager.TaskTitleExists("Task 3");

            // Assert
            Assert.IsFalse(result, "Task with title 'Task 3' should not exist.");
        }

        [TestMethod]
        public void FindTaskByTitle_WhenTaskExists_ShouldReturnTask()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            TaskBase foundTask = TaskManager.FindTaskByTitle("Task 1");

            // Assert
            Assert.IsNotNull(foundTask, "Task with title 'Task 1' should be found.");
            Assert.AreEqual("Task 1", foundTask.TaskTitle, "Task title should match.");
        }

        [TestMethod]
        public void FindTaskByTitle_WhenTaskDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            TaskBase foundTask = TaskManager.FindTaskByTitle("Task 3");

            // Assert
            Assert.IsNull(foundTask, "Task with title 'Task 3' should not be found.");
        }

        [TestMethod]
        public void TaskDueDateExists_WhenDueDateExists_ShouldReturnTrue()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", new DateTime(2023, 10, 1), true),
                new TaskBase(2, "Task 2", "Description 2", new DateTime(2023, 10, 2), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            bool result = TaskManager.TaskDueDateExists(new DateTime(2023, 10, 1));

            // Assert
            Assert.IsTrue(result, "Task with due date '2023-10-01' should exist.");
        }

        [TestMethod]
        public void TaskDueDateExists_WhenDueDateDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", new DateTime(2023, 10, 1), true),
                new TaskBase(2, "Task 2", "Description 2", new DateTime(2023, 10, 2), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            bool result = TaskManager.TaskDueDateExists(new DateTime(2023, 10, 3));

            // Assert
            Assert.IsFalse(result, "Task with due date '2023-10-03' should not exist.");
        }

        [TestMethod]
        public void FindTasksByDueDate_WhenDueDateExists_ShouldReturnMatchingTasks()
        {
            // Arrange
            var tasks = new List<TaskBase>
        {
            new TaskBase(1, "Task 1", "Description 1", new DateTime(2023, 10, 1), true),
            new TaskBase(2, "Task 2", "Description 2", new DateTime(2023, 10, 2), false),
            new TaskBase(3, "Task 3", "Description 3", new DateTime(2023, 10, 1), true),
        };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByDueDate(new DateTime(2023, 10, 1));

            // Assert
            Assert.AreEqual(2, matchingTasks.Count, "Two tasks with due date '2023-10-01' should be found.");
        }

        [TestMethod]
        public void FindTasksByDueDate_WhenDueDateDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBase>
        {
            new TaskBase(1, "Task 1", "Description 1", new DateTime(2023, 10, 1), true),
            new TaskBase(2, "Task 2", "Description 2", new DateTime(2023, 10, 2), false),
            new TaskBase(3, "Task 3", "Description 3", new DateTime(2023, 10, 1), true),
        };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByDueDate(new DateTime(2023, 10, 3));

            // Assert
            Assert.AreEqual(0, matchingTasks.Count, "No tasks with due date '2023-10-03' should be found.");
        }

        [TestMethod]
        public void SearchTasksByPartialTitle_WhenPartialTitleExists_ShouldReturnMatchingTasks()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(3, "Another Title 1", "Description 3", DateTime.Now.AddDays(2), true),
                new TaskBase(4, "Another Title 2", "Description 4", DateTime.Now.AddDays(10), false),
                new TaskBase(5, "Another Task 1", "Description 5", DateTime.Now.AddDays(2), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByPartialTitle("Task");

            // Assert
            Assert.AreEqual(3, matchingTasks.Count, "Three tasks with 'Task' in their title should be found.");
        }

        [TestMethod]
        public void SearchTasksByPartialTitle_WhenPartialTitleDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(3, "Another Title 1", "Description 3", DateTime.Now.AddDays(2), true),
            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByPartialTitle("NonExistent");

            // Assert
            Assert.AreEqual(0, matchingTasks.Count, "No tasks with 'NonExistent' in their title should be found.");
        }

        [TestMethod]
        public void SearchTasksByPartialDescription_WhenPartialTitleExists_ShouldReturnMatchingTasks()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1 magic word", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(3, "Another Title 1", "Description 3", DateTime.Now.AddDays(2), true),
                new TaskBase(4, "Another Title 2", "Description 4 magic word", DateTime.Now.AddDays(10), false),
                new TaskBase(5, "Another Task 1", "Description 5 magic word", DateTime.Now.AddDays(2), false),
            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByPartialDescription("magic");

            // Assert
            Assert.AreEqual(3, matchingTasks.Count, "Three tasks with 'magic' in their title should be found.");
        }

        [TestMethod]
        public void SearchTasksByPartialDescription_WhenPartialTitleDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(3, "Another Title 1", "Description 3", DateTime.Now.AddDays(2), true),
            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingTasks = TaskManager.FindTasksByPartialDescription("NonExistent");

            // Assert
            Assert.AreEqual(0, matchingTasks.Count, "No tasks with 'NonExistent' in their title should be found.");
        }

        [TestMethod]
        public void FindTasksByCompletionStatus_WhenMatchingTasksExist_ShouldReturnMatchingTasks()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(3, "Task 3", "Description 3", DateTime.Now.AddDays(2), true),
            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingCompletedTasks = TaskManager.FindTasksByCompletionStatus(true);
            List<TaskBase> matchingIncompleteTasks = TaskManager.FindTasksByCompletionStatus(false);

            // Assert
            Assert.AreEqual(2, matchingCompletedTasks.Count, "Two tasks with completion status 'true' should be found.");
            Assert.AreEqual(1, matchingIncompleteTasks.Count, "One task with completion status 'false' should be found.");
        }

        [TestMethod]
        public void FindTasksByCompletionStatus_WhenNoMatchingTasksExist_ShouldReturnEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {

            };

            TaskManager.Tasks = tasks;

            // Act
            List<TaskBase> matchingIncompleteTasks = TaskManager.FindTasksByCompletionStatus(false);
            List<TaskBase> matchingCompleteTasks = TaskManager.FindTasksByCompletionStatus(true);

            // Assert
            Assert.AreEqual(0, matchingIncompleteTasks.Count, "No task with completion status 'false' should be found.");
            Assert.AreEqual(0, matchingCompleteTasks.Count, "No task with completion status 'true' should be found.");
        }

        [TestMethod]
        public void DeleteTask_WhenTaskExists_ShouldRemoveTaskAndDisplayMessage()
        {
            // Arrange
            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
            };

            TaskManager.Tasks = tasks;

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            TaskManager.DeleteTask(1);

            // Assert
            Assert.AreEqual(1, TaskManager.Tasks.Count, "One task should be removed.");
            Equals("Task with ID 1 has been deleted.", consoleOutput.ToString());
        }

        [TestMethod]
        public void GetValidTaskId_WhenValidInputIsProvided_ShouldReturnTaskId()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("1"); // Valid task ID

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            int result = TaskManager.GetValidTaskId(mockConsole.Object);

            // Assert
            Assert.AreEqual(1, result, "Valid task ID should be returned.");
        }

        [TestMethod]
        public void GetValidTaskId_WhenInvalidInputIsProvided_ShouldReturnMinusOne()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("invalid"); // Invalid input

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            int result = TaskManager.GetValidTaskId(mockConsole.Object);

            // Assert
            Assert.AreEqual(-1, result, "Invalid input should result in -1.");
        }

        [TestMethod]
        public void GetValidTaskId_WhenZeroIsProvided_ShouldReturnZero()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("0"); // Zero, indicating return to the main menu

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            int result = TaskManager.GetValidTaskId(mockConsole.Object);

            // Assert
            Assert.AreEqual(0, result, "Zero input should result in 0.");
        }

        [TestMethod]
        public void EditTask_WhenChoiceIsOne_ShouldUpdateTaskTitle()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            mockUserInterface.SetupSequence(u => u.ReadLine())
                .Returns("New Title"); // Simulate user input

            TaskManager taskManager = new TaskManager(mockUserInterface.Object);

            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Original Title", "Description", DateTime.Now, false),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(5, "Title 5", "Description 5", DateTime.Now, false),
            };

            TaskManager.Tasks = tasks;

            var taskToEdit = tasks[0];

            // Act
            TaskManager.EditTask(taskToEdit, "1", mockUserInterface.Object);

            // Assert
            Assert.AreEqual("New Title", taskToEdit.TaskTitle, "Task Title should be updated.");
            Equals("Task Title successfully updated!", consoleOutput.ToString());
        }

        [TestMethod]
        public void EditTask_WhenChoiceIsTwo_ShouldUpdateTaskDescription()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var tasks = new List<TaskBase>
            {
                new TaskBase(2, "Title 1", "Original Description", DateTime.Now, false),
                new TaskBase(3, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(5, "Title 5", "Description 5", DateTime.Now, false),
            };

            TaskManager.Tasks = tasks;

            var taskToEdit = tasks[0];
            var userInput = "New Description";

            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(userInput);

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            TaskManager.EditTask(taskToEdit, "2", mockConsole.Object);

            // Assert
            Assert.AreEqual(userInput, taskToEdit.TaskDescription, "Task Description should be updated.");
            Equals("Task Description successfully updated!", consoleOutput.ToString());
        }

        [TestMethod]
        public void EditTask_WhenChoiceIsThree_ShouldUpdateTaskDueDate()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Title 2", "Description 2", DateTime.Now, false),
                new TaskBase(3, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(5, "Title 5", "Description 5", DateTime.Now, true),
            };

            TaskManager.Tasks = tasks;

            var taskToEdit = tasks[0];
            var userInput = "2012-12-12";

            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(userInput);

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            TaskManager.EditTask(taskToEdit, "3", mockConsole.Object);

            // Assert
            Assert.AreEqual(DateTime.Parse(userInput), tasks.Find(task => task.TaskId == 1).DueDate, "Task Due Date should be updated.");
            Equals("Task Due Date successfully updated!", consoleOutput.ToString());
        }

        [TestMethod]
        public void EditTask_WhenChoiceIsFour_ShouldToggleTaskCompletionStatus()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var taskToEdit = new TaskBase(1, "Title 2", "Description 2", DateTime.Now, false);

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            TaskManager.EditTask(taskToEdit, "4", mockConsole.Object);

            // Assert
            Assert.AreEqual(true, taskToEdit.TaskIsComplete, "Task Completion Status should be updated.");
            Equals("Task Completion Status successfully updated!", consoleOutput.ToString());
        }

        [TestMethod]
        public void EditTask_WhenChoiceIsFive_ShouldDeleteTask()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var tasks = new List<TaskBase>
            {
                new TaskBase(1, "Task 1", "Description 1", DateTime.Now, true),
                new TaskBase(2, "Task 2", "Description 2", DateTime.Now.AddDays(1), false),
                new TaskBase(5, "Title 5", "Description 5", DateTime.Now, false),
            };

            TaskManager.Tasks = tasks;

            var taskToEdit = new TaskBase(5, "Title 5", "Description 5", DateTime.Now, false);

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            TaskManager.EditTask(taskToEdit, "5", mockConsole.Object);

            // Assert
            Assert.AreEqual(2, TaskManager.Tasks.Count, "One task should be removed.");
            Equals("Task Completion Status successfully updated!", consoleOutput.ToString());
        }
        

        [TestMethod]
        public void EditTask_WhenChoiceIsInvalid_ShouldDisplayErrorMessage()
        {
            // Arrange
            var mockConsole = new Mock<IUserInterface>();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var taskToEdit = new TaskBase(1, "Original Title", "Description", DateTime.Now, false);
            var userInput = "Invalid Choice"; // Invalid user input

            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(userInput);

            TaskManager taskManager = new TaskManager(mockConsole.Object);

            // Act
            TaskManager.EditTask(taskToEdit, "InvalidChoice", mockConsole.Object);

            // Assert
            Assert.AreEqual("Original Title", taskToEdit.TaskTitle, "Task Title should not be updated.");
            Equals("Invalid choice. Try again.", consoleOutput.ToString());
        }
    }
}
