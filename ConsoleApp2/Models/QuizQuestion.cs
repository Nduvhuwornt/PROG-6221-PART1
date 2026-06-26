namespace CybersecurityChatbot.Models
{
    /// <summary>
    /// Represents a quiz question with multiple-choice or true/false options.
    /// </summary>
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public bool IsTrueFalse { get; set; }
    }
}
