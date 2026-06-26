namespace CybersecurityChatbot.Models
{
    /// <summary>
    /// Represents a single entry in the chatbot activity log.
    /// </summary>
    public class ActivityLogEntry
    {
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Category { get; set; } = string.Empty; // Task, Quiz, NLP, Chat, Reminder

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [{Category}] {Action}";
        }
    }
}
