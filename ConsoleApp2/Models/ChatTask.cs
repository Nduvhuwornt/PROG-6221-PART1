namespace CybersecurityChatbot.Models
{
    /// <summary>
    /// Represents a cybersecurity task managed by the Task Assistant.
    /// </summary>
    public class ChatTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            string status = IsCompleted ? "[✓]" : "[ ]";
            string reminder = ReminderDate.HasValue ? $" | Reminder: {ReminderDate.Value:dd MMM yyyy}" : "";
            return $"{status} {Title}{reminder}";
        }
    }
}
