using CybersecurityChatbot.Models;

namespace CybersecurityChatbot.Services
{
    /// <summary>
    /// Manages cybersecurity tasks in memory, syncing with the database when available.
    /// </summary>
    public class TaskService
    {
        private readonly List<ChatTask> _tasks = new();
        private readonly DatabaseService _db;
        private bool _dbAvailable;
        public bool DbAvailable => _dbAvailable;
        private int _nextId = 1;

        public List<ChatTask> Tasks => _tasks;

        public TaskService(DatabaseService db)
        {
            _db = db;
            _dbAvailable = _db.InitialiseDatabase();
            if (_dbAvailable)
                LoadFromDatabase();
        }

        /// <summary>
        /// Loads tasks from the database into the in-memory list.
        /// </summary>
        private void LoadFromDatabase()
        {
            var dbTasks = _db.GetAllTasks();
            _tasks.AddRange(dbTasks);
            if (_tasks.Count > 0)
                _nextId = _tasks.Max(t => t.Id) + 1;
        }

        /// <summary>
        /// Adds a new task and persists it to the database.
        /// </summary>
        public ChatTask AddTask(string title, string description, DateTime? reminderDate = null)
        {
            var task = new ChatTask
            {
                Id = _nextId++,
                Title = title,
                Description = description,
                ReminderDate = reminderDate,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            if (_dbAvailable)
            {
                int dbId = _db.AddTask(task);
                if (dbId > 0) task.Id = dbId;
            }

            _tasks.Insert(0, task);
            return task;
        }

        /// <summary>
        /// Marks a task as completed by its list index.
        /// </summary>
        public bool MarkComplete(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;
            task.IsCompleted = true;
            if (_dbAvailable) _db.MarkTaskComplete(taskId);
            return true;
        }

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        public bool DeleteTask(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;
            _tasks.Remove(task);
            if (_dbAvailable) _db.DeleteTask(taskId);
            return true;
        }

        /// <summary>
        /// Returns a formatted string listing all tasks.
        /// </summary>
        public string GetTaskListString()
        {
            if (_tasks.Count == 0)
                return "No tasks found. Add a task by saying 'add task' or using the Task Manager tab!";

            string result = $"📋 Your Cybersecurity Tasks ({_tasks.Count} total):\n\n";
            int count = 1;
            foreach (var t in _tasks)
            {
                string status = t.IsCompleted ? "✅" : "⏳";
                result += $"{count++}. {status} [{t.Id}] {t.Title}\n";
                if (!string.IsNullOrEmpty(t.Description))
                    result += $"   📝 {t.Description}\n";
                if (t.ReminderDate.HasValue)
                    result += $"   🔔 Reminder: {t.ReminderDate.Value:dd MMM yyyy}\n";
                result += "\n";
            }
            return result;
        }
    }
}
