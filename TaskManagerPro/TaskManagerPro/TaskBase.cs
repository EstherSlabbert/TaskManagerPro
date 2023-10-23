using System;

namespace TaskManagerPro
{
    public class TaskBase
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public bool TaskIsComplete { get; set; }

        public TaskBase(int id, string title, string description, DateTime dueDate, bool taskCompleted = false)
        {
            TaskId = id;
            TaskTitle = title;
            TaskDescription = description;
            DueDate = dueDate;
            TaskIsComplete = taskCompleted;
        }
    }
}
