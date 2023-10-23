using Newtonsoft.Json;
using System;
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

        public void AddTask(TaskBase newTask)
        {
            Tasks.Add(newTask);
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

        public static void SaveTasksToFile()
        {
            // Serialize tasks to JSON and save to the file
            string json = JsonConvert.SerializeObject(Tasks);
            File.WriteAllText("tasks.txt", json);
        }

        public static void LoadTasksFromFile()
        {
            // Load tasks from the file and populate the tasks list
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
    }
}
