using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerPro;

namespace TaskManagerPro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IUserInterface consoleUserInterface = new ConsoleUserInterface();

            // Create an instance of TaskManager with the consoleUserInterface
            TaskManager taskManager = new TaskManager(consoleUserInterface);
            
            // Initialize the application (for persistence)
            TaskManager.InitializeApp();

            while (true)
            {
                UserInterface.DisplayMainMenu();
                Console.Write("Enter your choice: ");
                // Get user input and perform actions based on the input
                string userInput = Console.ReadLine();
                
                switch (userInput)
                {
                    case "1":
                        UserInterface.DisplayMainMenuChoiceOpeningStatement("Option 1 - Add New Task");
                        // Add a new task
                        TaskBase newTask = taskManager.NewTask();
                        taskManager.AddTask(newTask);
                        TaskManager.SaveTasksToFile();

                        UserInterface.DisplayContinueToMainMenuMessage();
                        break;
                    case "2":
                        UserInterface.DisplayMainMenuChoiceOpeningStatement("Option 2 - Update/Edit Task");

                        var taskToEditId = TaskManager.GetValidTaskId(consoleUserInterface);
                        if (taskToEditId == 0)
                        {
                            UserInterface.DisplayContinueToMainMenuMessage();
                            break;
                        }
                        else
                        {
                            while (taskToEditId == -1)
                            {
                                Console.WriteLine("Invalid Task ID or Task does not exist.\nPlease try again or enter 0 to exit to the main menu.");
                                taskToEditId = TaskManager.GetValidTaskId(consoleUserInterface);
                            }
                            TaskBase taskToEdit = TaskManager.FindTaskById(taskToEditId);
                            UserInterface.DisplayUpdateSubMenu();
                            var editChoice = TaskManager.ValidatedEditSelection(consoleUserInterface);
                            // Update/Edit a task
                            TaskManager.EditTask(taskToEdit, editChoice, consoleUserInterface);
                            UserInterface.DisplayContinueToMainMenuMessage();
                            break;
                        }
                    case "3":
                        UserInterface.DisplayMainMenuChoiceOpeningStatement("Option 3 - View All Task(s)");
                        // View all tasks
                        TaskManager.DisplayAllTasks();
                        UserInterface.DisplayContinueToMainMenuMessage();
                        break;
                    case "4":
                        UserInterface.DisplayMainMenuChoiceOpeningStatement("Option 4 - Search For Task(s)");
                        // Search for task(s)
                            // find task
                            // display task
                        // SearchTasks();
                        UserInterface.DisplayContinueToMainMenuMessage();
                        break;
                    case "5":
                        // Exit the program
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a valid menu option.");
                        break;
                }
            }
        }
    }
}
