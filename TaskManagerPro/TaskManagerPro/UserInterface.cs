using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerPro
{
    public class UserInterface
    {
        public static void DisplayMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("--------Welcome to the To-Do List Task Manager Program!--------");
            Console.ResetColor();
            Console.WriteLine("\nPlease make a selection from the menu below:\n");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Menu:");
            Console.WriteLine("1 - Add a new task");
            Console.WriteLine("2 - Update/Edit a task");
            Console.WriteLine("3 - View all tasks");
            Console.WriteLine("4 - Search for task(s)");
            Console.WriteLine("5 - Exit");
            Console.ResetColor();
            Console.WriteLine("\nWhen making a selection just enter the \x1b[1mnumber\x1b[0m of your selection.\n");
        }

        public static void DisplayMainMenuChoiceOpeningStatement(string option)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n-----You selected " + option + "-----\n");
            Console.ResetColor();
        }

        public static void DisplayContinueToMainMenuMessage()
        {
            Console.WriteLine("\nPress 'Enter' when you are ready to continue to the main menu.");
            Console.ReadLine();
        }
    }
}
