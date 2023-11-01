using System;

namespace TaskManagerPro
{
    public interface IUserInterface
    {
        string ReadLine();
        void WriteLine(string value);
        ConsoleKeyInfo ReadKey();
    }
}
