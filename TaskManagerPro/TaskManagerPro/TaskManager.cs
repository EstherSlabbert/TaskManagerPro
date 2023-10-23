using System.Collections.Generic;

namespace TaskManagerPro
{
    public class TaskManager
    {
        public List<TaskBase> Tasks = new List<TaskBase>();
        public int CurrentTaskId;

        public void AddTask(TaskBase newTask)
        {
            Tasks.Add(newTask);
        }
    }
}
