using System;

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

        public static void DisplayUpdateSubMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Select an option to edit:");
            Console.WriteLine("1 - Edit Title");
            Console.WriteLine("2 - Edit Description");
            Console.WriteLine("3 - Edit Due Date");
            Console.WriteLine("4 - Toggle Completion Status");
            Console.WriteLine("5 - Delete Task");
            Console.WriteLine("6 - Cancel");
            Console.ResetColor();
            Console.WriteLine("\nWhen making a selection just enter the \x1b[1mnumber\x1b[0m of your selection.\n");
        }

        public static void DisplaySearchSubMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Select an option to search Task(s) by:");
            Console.WriteLine("1 - Task ID");
            Console.WriteLine("2 - Title");
            Console.WriteLine("3 - Description");
            Console.WriteLine("4 - Due Date");
            Console.WriteLine("5 - Completion Status");
            Console.WriteLine("6 - Cancel");
            Console.ResetColor();
            Console.WriteLine("\nWhen making a selection just enter the \x1b[1mnumber\x1b[0m of your selection.\n");
        }

        public static void DisplaySearchSubMenuTitle(string searchOption)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"----You are searching for a task by {searchOption}----\n");
            Console.ResetColor();
        }

        public static void DisplayCompletionStatusSearchSubSubMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Select an option to search Task Completion Status by:");
            Console.WriteLine("1 - Complete");
            Console.WriteLine("2 - Incomplete");
            Console.WriteLine("3 - Cancel");
            Console.ResetColor();
            Console.WriteLine("\nWhen making a selection just enter the \x1b[1mnumber\x1b[0m of your selection.\n");
        }

        public static void DisplayConfirmationToContinueSearchOrExit(string searchOption)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNo tasks found.\nPlease try again.");
            Console.ResetColor();
            Console.Write($"\nPress 'Spacebar' to return to main menu or 'Enter' to continue searching by {searchOption}: ");
        }

        // Not implemented yet:
        public static void DisplayDueDateSearchSubSubMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Select an option to search Task Due Date by:");
            Console.WriteLine("1 - Specific Date");
            Console.WriteLine("2 - Before Date");
            Console.WriteLine("3 - After Date");
            Console.WriteLine("4 - Between 2 Dates");
            Console.WriteLine("5 - Cancel");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nNote: Searched Date(s) will be included in results.\n");
            Console.ResetColor();
            Console.WriteLine("\nWhen making a selection just enter the \x1b[1mnumber\x1b[0m of your selection.\n");
        }
    }
}
