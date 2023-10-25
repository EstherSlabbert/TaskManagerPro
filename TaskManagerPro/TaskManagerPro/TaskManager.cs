using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagerPro
{
    public class TaskManager
    {
        public IUserInterface userInterface;
        public static List<TaskBase> Tasks = new List<TaskBase>();
        public static int CurrentTaskId;

        public TaskManager(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
            CurrentTaskId = 0;
        }

        public TaskManager()
        {
        }

        public static void LoadTasksFromFile()
        {
            string json = File.ReadAllText("tasks.txt");
            Tasks = JsonConvert.DeserializeObject<List<TaskBase>>(json);
        }

        public static void InitializeApp()
        {
            if (!File.Exists("tasks.txt"))
            {
                File.WriteAllText("tasks.txt", "[]");
                CurrentTaskId = 0;
            }

            else if (File.Exists("tasks.txt"))
            {
                LoadTasksFromFile();
                CurrentTaskId = Tasks.Count > 0 ? Tasks.Max(task => task.TaskId) : 0;
            }
        }

        public static DateTime ValidatedUserDateInput(IUserInterface userInterface)
        {
            DateTime dueDate;
            bool validDueDate = false;

            do
            {
                userInterface.WriteLine("Enter the due date (YYYY-MM-DD): ");
                string dueDateInput = userInterface.ReadLine();

                if (DateTime.TryParse(dueDateInput, out dueDate))
                {
                    validDueDate = true;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.");
                }
            } while (!validDueDate);

            return dueDate;
        }

        public TaskBase NewTask()
        {
            int newTaskId = ++CurrentTaskId;

            userInterface.WriteLine("Enter Task Title: ");
            string title = userInterface.ReadLine();

            userInterface.WriteLine("Enter Task Description: ");
            string description = userInterface.ReadLine();

            DateTime dueDate = ValidatedUserDateInput(userInterface);

            TaskBase newTask = new TaskBase(newTaskId, title, description, dueDate);
            return newTask;
        }

        public void AddTask(TaskBase newTask)
        {
            Tasks.Add(newTask);
        }

        public static void SaveTasksToFile()
        {
            // Serialize tasks to JSON and save to the file
            string json = JsonConvert.SerializeObject(Tasks);
            File.WriteAllText("tasks.txt", json);
        }

        public static void DisplayTask(TaskBase task)
        {
            Console.WriteLine("");
            Console.WriteLine($"Task ID: {task.TaskId}");
            if (task.DueDate <= DateTime.Now & !task.TaskIsComplete) Console.ForegroundColor = ConsoleColor.Red;
            else if (task.DueDate >= DateTime.Now & !task.TaskIsComplete) Console.ForegroundColor = ConsoleColor.Blue;
            else Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Due: {task.DueDate.ToString("dd-MM-yyyy")}");
            Console.ResetColor();
            Console.ForegroundColor = task.TaskIsComplete ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Completion status: {task.TaskIsComplete}");
            Console.ResetColor();
            Console.WriteLine($"Title: {task.TaskTitle}");
            Console.WriteLine($"Description: {task.TaskDescription}");
            Console.WriteLine("");
        }

        public static void DisplayAllTasks()
        {
            if (Tasks.Count > 0)
            {
                Console.WriteLine("\nAll Tasks:");
                foreach (var task in Tasks)
                {
                    DisplayTask(task);
                }
            }
            else
            {
                Console.WriteLine("No tasks found. Your task list is empty.");
            }
        }

        public static string ValidatedEditSelection(IUserInterface userInterface)
        {
            userInterface.WriteLine("Enter your choice (1-6): ");
            string editChoice = userInterface.ReadLine();
            while (!new[] { "1", "2", "3", "4", "5", "6" }.Any(choice => choice == editChoice))
            {
                userInterface.WriteLine("Invalid option. Please try again.");
                userInterface.WriteLine("Enter your choice (1-6): ");
                editChoice = userInterface.ReadLine();
            }
            return editChoice;
        }

        public static bool TaskIdExists(int taskId)
        {
            return Tasks.Any(task => task.TaskId == taskId);
        }

        public static bool ValidateTaskId(string userInput, out int taskId)
        {
            if (int.TryParse(userInput, out taskId))
            {
                if (TaskIdExists(taskId))
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetValidTaskId(IUserInterface userInterface)
        {
            int taskId;
            userInterface.WriteLine("Enter Task ID (0 to return to main menu): ");
            string userInput = userInterface.ReadLine();
            if (userInput == "0") return 0;
            else if (ValidateTaskId(userInput, out taskId))
            {
                return taskId;
            }
            else
            {
                return -1;
            }
        }

        public static TaskBase FindTaskById(int taskId)
        {
            return Tasks.Find(task => task.TaskId == taskId);
        }

        public static bool TaskTitleExists(string taskTitle)
        {
            return Tasks.Any(task => task.TaskTitle == taskTitle);
        }

        public static TaskBase FindTaskByTitle(string taskTitle)
        {
            return Tasks.Find(task => task.TaskTitle == taskTitle);
        }

        public static bool TaskDueDateExists(DateTime dueDate)
        {
            return Tasks.Any(task => task.DueDate == dueDate);
        }

        public static List<TaskBase> FindTasksByDueDate(DateTime dueDate)
        {
            return Tasks
                .Where(task => task.DueDate == dueDate)
                .ToList();
        }

        public static List<TaskBase> FindTasksByPartialTitle(string partialTitle)
        {
            List<TaskBase> matchingTasks = Tasks
                .Where(task => task.TaskTitle.IndexOf(partialTitle, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            return matchingTasks;
        }

        public static List<TaskBase> FindTasksByPartialDescription(string partialDescription)
        {
            List<TaskBase> matchingTasks = Tasks
                .Where(task => task.TaskDescription.IndexOf(partialDescription, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            return matchingTasks;
        }

        public static List<TaskBase> FindTasksByCompletionStatus(bool completionStatus)
        {
            return Tasks
                .Where(task => task.TaskIsComplete == completionStatus)
                .ToList();
        }

        public static void DeleteTask(int taskId)
        {
            Tasks.Remove(FindTaskById(taskId));
            Console.WriteLine($"Task with ID {taskId} has been deleted.");
        }

        public static void EditTask(TaskBase taskToEdit, string editChoice, IUserInterface userInterface)
        {
            switch (editChoice)
            {
                case "1":
                    Console.Write("Enter the new task title: ");
                    taskToEdit.TaskTitle = userInterface.ReadLine();
                    SaveTasksToFile();
                    userInterface.WriteLine("\nTask Title successfully updated!");
                    break;
                case "2":
                    Console.Write("Enter the new task description: ");
                    taskToEdit.TaskDescription = userInterface.ReadLine();
                    SaveTasksToFile();
                    userInterface.WriteLine("\nTask Description successfully updated!");
                    break;
                case "3":
                    taskToEdit.DueDate = ValidatedUserDateInput(userInterface);
                    SaveTasksToFile();
                    userInterface.WriteLine("\nTask Due Date successfully updated!");
                    break;
                case "4":
                    taskToEdit.TaskIsComplete = !taskToEdit.TaskIsComplete;
                    SaveTasksToFile();
                    userInterface.WriteLine("\nTask Completion Status successfully updated!");
                    break;
                case "5":
                    DeleteTask(taskToEdit.TaskId);
                    SaveTasksToFile();
                    userInterface.WriteLine("\nTask has been deleted.");
                    break;
                case "6":
                    userInterface.WriteLine("\nEdit has been cancelled.");
                    break;
                default:
                    userInterface.WriteLine("\nInvalid choice. Try again.");
                    break;
            }
        }

        public static string ValidatedTaskCompletionStatusSelection(IUserInterface userInterface)
        {
            userInterface.WriteLine("Enter your choice (1-3): ");
            string userChoice = userInterface.ReadLine();
            while (!new[] { "1", "2", "3" }.Any(choice => choice == userChoice))
            {
                userInterface.WriteLine("Invalid option. Please try again.");
                userInterface.WriteLine("Enter your choice (1-3): ");
                userChoice = userInterface.ReadLine();
            }
            return userChoice;
        }

        public static void DisplayTasksByCompletion(string userSelection)
        {
            switch (userSelection)
            {
                case "1":
                    if (FindTasksByCompletionStatus(true).Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nNo tasks found.\nPlease try again.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("\nSearch Results:\n");
                        foreach (var task in FindTasksByCompletionStatus(true))
                        {
                            DisplayTask(task);
                        }
                    }
                    break;
                case "2":
                    if (FindTasksByCompletionStatus(true).Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nNo tasks found.\nPlease try again.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("\nSearch Results:\n");
                        foreach (var task in FindTasksByCompletionStatus(false))
                        {
                            DisplayTask(task);
                        }
                    }
                    break;
                case "3":
                    Console.WriteLine("Search cancelled. Returning to main menu...");
                    break;
                default:
                    Console.WriteLine("Invalid Input. Try again.");
                    break;
            }
        }

        public static void SearchForTasks(string userChoice, IUserInterface userInterface)
        {
            switch (userChoice)
            {
                case "1":
                    UserInterface.DisplaySearchSubMenuTitle("ID");
                    int potentialId = GetValidTaskId(userInterface);
                    if (potentialId == 0) userInterface.WriteLine("Search cancelled. Returning to main menu...");
                    else if (potentialId == -1)
                    {
                        userInterface.WriteLine("Invalid Task ID. Try again.");
                        SearchForTasks("1", userInterface);
                    }
                    else DisplayTask(FindTaskById(potentialId));
                    break;
                case "2":
                    UserInterface.DisplaySearchSubMenuTitle("Title");
                    userInterface.WriteLine("Enter the title/partial title you would like to search for:");
                    var potentialTasksByTitle = FindTasksByPartialTitle(userInterface.ReadLine());
                    if (potentialTasksByTitle.Count == 0)
                    {
                        UserInterface.DisplayConfirmationToContinueSearchOrExit("Title");
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Spacebar) break;
                        else SearchForTasks("2", userInterface);
                    }
                    else
                    {
                        userInterface.WriteLine("\nSearch Results:\n");
                        foreach (var task in potentialTasksByTitle)
                        {
                            DisplayTask(task);
                        }
                    }
                    break;
                case "3":
                    UserInterface.DisplaySearchSubMenuTitle("Description");
                    userInterface.WriteLine("Enter the description/partial description you would like to search for:");
                    var potentialTasksByDescription = FindTasksByPartialDescription(userInterface.ReadLine());
                    if (potentialTasksByDescription.Count == 0)
                    {
                        UserInterface.DisplayConfirmationToContinueSearchOrExit("Description");
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Spacebar) break;
                        else SearchForTasks("3", userInterface);
                    }
                    else
                    {
                        userInterface.WriteLine("\nSearch Results:\n");
                        foreach (var task in potentialTasksByDescription)
                        {
                            DisplayTask(task);
                        }
                    }
                    break;
                case "4":
                    UserInterface.DisplaySearchSubMenuTitle("Due Date");
                    userInterface.WriteLine("Enter the due date you would like to search for:");                    
                    var potentialTasksByDueDate = FindTasksByDueDate(ValidatedUserDateInput(userInterface));
                    if (potentialTasksByDueDate.Count == 0)
                    {
                        UserInterface.DisplayConfirmationToContinueSearchOrExit("Due Date");
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Spacebar) break;
                        else SearchForTasks("4", userInterface);
                    }
                    else
                    {
                        userInterface.WriteLine("\nSearch Results:\n");
                        foreach (var task in potentialTasksByDueDate)
                        {
                            DisplayTask(task);
                        }
                    }
                    break;
                case "5":
                    UserInterface.DisplaySearchSubMenuTitle("Completion Status");
                    UserInterface.DisplayCompletionStatusSearchSubSubMenu();
                    userInterface.WriteLine("Enter the completion status you would like to search for: ");
                    var userSelection = ValidatedTaskCompletionStatusSelection(userInterface);
                    DisplayTasksByCompletion(userSelection);
                    break;
                case "6":
                    userInterface.WriteLine("Search has been cancelled");
                    break;
                default:
                    userInterface.WriteLine("\nInvalid choice. Try again.");
                    break;
            }
        }
    }
}
