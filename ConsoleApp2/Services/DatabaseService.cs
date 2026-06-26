using CybersecurityChatbot.Models;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbot.Services
{
    /// <summary>
    /// Handles all MySQL database operations for tasks AND the activity log.
    /// Both are persisted so they survive app restarts.
    /// </summary>
    public class DatabaseService
    {
        private const string ConnectionString =
            "Server=localhost;Database=cybersecurity_chatbot;Uid=root;Pwd=Ramadirnt@1;";

        // ═══════════════════════════════════════════════════════════════════════════
        // INIT — creates both tables if they don't exist
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Opens a connection and ensures both the tasks and activity_log tables exist.
        /// Returns true if the database is reachable, false if offline (app runs in-memory).
        /// </summary>
        public bool InitialiseDatabase()
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                // Create the database itself in case it doesn't exist yet
                new MySqlCommand(
                    "CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot;", conn)
                    .ExecuteNonQuery();

                // Tasks table
                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS cybersecurity_chatbot.tasks (
                        Id          INT AUTO_INCREMENT PRIMARY KEY,
                        Title       VARCHAR(255) NOT NULL,
                        Description TEXT,
                        IsCompleted BOOLEAN      DEFAULT FALSE,
                        ReminderDate DATETIME    NULL,
                        CreatedAt   DATETIME     DEFAULT CURRENT_TIMESTAMP
                    );", conn).ExecuteNonQuery();

                // Activity log table — stores every chatbot action with a timestamp
                new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS cybersecurity_chatbot.activity_log (
                        Id        INT AUTO_INCREMENT PRIMARY KEY,
                        Category  VARCHAR(50)  NOT NULL,
                        Action    TEXT         NOT NULL,
                        Timestamp DATETIME     DEFAULT CURRENT_TIMESTAMP
                    );", conn).ExecuteNonQuery();

                return true;
            }
            catch
            {
                // MySQL not available — app falls back to in-memory only
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // TASKS
        // ═══════════════════════════════════════════════════════════════════════════

        public int AddTask(ChatTask task)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                const string sql = @"
                    INSERT INTO cybersecurity_chatbot.tasks
                        (Title, Description, IsCompleted, ReminderDate, CreatedAt)
                    VALUES
                        (@title, @desc, @completed, @reminder, @created);
                    SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", task.Title);
                cmd.Parameters.AddWithValue("@desc", task.Description);
                cmd.Parameters.AddWithValue("@completed", task.IsCompleted);
                cmd.Parameters.AddWithValue("@reminder", task.ReminderDate.HasValue
                                                            ? (object)task.ReminderDate.Value
                                                            : DBNull.Value);
                cmd.Parameters.AddWithValue("@created", task.CreatedAt);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch { return -1; }
        }

        public List<ChatTask> GetAllTasks()
        {
            var list = new List<ChatTask>();
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var reader = new MySqlCommand(
                    "SELECT * FROM cybersecurity_chatbot.tasks ORDER BY CreatedAt DESC;", conn)
                    .ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ChatTask
                    {
                        Id = reader.GetInt32("Id"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                         ? "" : reader.GetString("Description"),
                        IsCompleted = reader.GetBoolean("IsCompleted"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate"))
                                         ? null : reader.GetDateTime("ReminderDate"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    });
                }
            }
            catch { }
            return list;
        }

        public bool MarkTaskComplete(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE cybersecurity_chatbot.tasks SET IsCompleted = TRUE WHERE Id = @id;", conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "DELETE FROM cybersecurity_chatbot.tasks WHERE Id = @id;", conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ACTIVITY LOG
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Saves a single activity log entry to the database immediately.
        /// </summary>
        public void SaveLogEntry(ActivityLogEntry entry)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                const string sql = @"
                    INSERT INTO cybersecurity_chatbot.activity_log (Category, Action, Timestamp)
                    VALUES (@cat, @action, @ts);";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cat", entry.Category);
                cmd.Parameters.AddWithValue("@action", entry.Action);
                cmd.Parameters.AddWithValue("@ts", entry.Timestamp);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        /// <summary>
        /// Loads the most recent log entries from the database (newest first).
        /// </summary>
        public List<ActivityLogEntry> GetRecentLog(int limit = 50)
        {
            var list = new List<ActivityLogEntry>();
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = $@"
                    SELECT Category, Action, Timestamp
                    FROM cybersecurity_chatbot.activity_log
                    ORDER BY Timestamp DESC
                    LIMIT {limit};";
                using var reader = new MySqlCommand(sql, conn).ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ActivityLogEntry
                    {
                        Category = reader.GetString("Category"),
                        Action = reader.GetString("Action"),
                        Timestamp = reader.GetDateTime("Timestamp")
                    });
                }
            }
            catch { }
            return list;
        }

        /// <summary>
        /// Deletes log entries older than the given number of days to keep the table tidy.
        /// </summary>
        public void PruneOldLog(int keepDays = 30)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand(@"
                    DELETE FROM cybersecurity_chatbot.activity_log
                    WHERE Timestamp < DATE_SUB(NOW(), INTERVAL @days DAY);", conn);
                cmd.Parameters.AddWithValue("@days", keepDays);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
    }
}
