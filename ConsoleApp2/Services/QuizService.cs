using CybersecurityChatbot.Models;

namespace CybersecurityChatbot.Services
{
    /// <summary>
    /// Manages the cybersecurity quiz game, including questions, scoring, and feedback.
    /// </summary>
    public class QuizService
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;
        private bool _isActive = false;

        public bool IsActive => _isActive;
        public int CurrentIndex => _currentIndex;
        public int TotalQuestions => _questions.Count;
        public int Score => _score;

        public QuizService()
        {
            _questions = BuildQuestions();
        }

        /// <summary>
        /// Starts a new quiz session, shuffling the question order.
        /// </summary>
        public void StartQuiz()
        {
            // Shuffle questions for variety
            var rng = new Random();
            for (int i = _questions.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_questions[i], _questions[j]) = (_questions[j], _questions[i]);
            }
            _currentIndex = 0;
            _score = 0;
            _isActive = true;
        }

        /// <summary>
        /// Returns the current question, or null if quiz has ended.
        /// </summary>
        public QuizQuestion? GetCurrentQuestion()
        {
            if (_currentIndex < _questions.Count)
                return _questions[_currentIndex];
            return null;
        }

        /// <summary>
        /// Submits an answer and returns feedback. Advances to the next question.
        /// </summary>
        public (bool IsCorrect, string Feedback) SubmitAnswer(int selectedIndex)
        {
            var question = _questions[_currentIndex];
            bool correct = selectedIndex == question.CorrectOptionIndex;
            if (correct) _score++;
            _currentIndex++;
            if (_currentIndex >= _questions.Count) _isActive = false;

            string feedback = correct
                ? $"✅ Correct! {question.Explanation}"
                : $"❌ Incorrect. The correct answer was: \"{question.Options[question.CorrectOptionIndex]}\"\n💡 {question.Explanation}";

            return (correct, feedback);
        }

        /// <summary>
        /// Returns a final score message based on the user's performance.
        /// </summary>
        public string GetFinalMessage()
        {
            int percent = (_score * 100) / TotalQuestions;
            if (percent >= 90) return $"🏆 Outstanding! {_score}/{TotalQuestions} — You're a Cybersecurity Pro!";
            if (percent >= 70) return $"🌟 Great job! {_score}/{TotalQuestions} — Keep up the good work!";
            if (percent >= 50) return $"📚 Not bad! {_score}/{TotalQuestions} — Keep learning to strengthen your defences.";
            return $"🔄 {_score}/{TotalQuestions} — Don't worry! Every lesson makes you safer. Try again!";
        }

        /// <summary>
        /// Builds the bank of 12 cybersecurity questions.
        /// </summary>
        private static List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new() { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectOptionIndex = 2,
                    Explanation = "Reporting phishing emails helps protect others and alerts your email provider."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for all accounts is acceptable if it's very strong.",
                    Options = new() { "True", "False" },
                    CorrectOptionIndex = 1,
                    IsTrueFalse = true,
                    Explanation = "Reusing passwords means one breach exposes all your accounts. Always use unique passwords."
                },
                new QuizQuestion
                {
                    Question = "What does HTTPS in a website URL indicate?",
                    Options = new() { "The website is popular", "The connection is encrypted and more secure", "The website is fast", "The website is free" },
                    CorrectOptionIndex = 1,
                    Explanation = "HTTPS uses SSL/TLS encryption to protect data between your browser and the website."
                },
                new QuizQuestion
                {
                    Question = "Which of the following is the strongest password?",
                    Options = new() { "password123", "John1990!", "Tr!8kQ#mX2@pL", "Admin@2024" },
                    CorrectOptionIndex = 2,
                    Explanation = "Strong passwords are long, random, and mix uppercase, lowercase, numbers, and symbols."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi networks are generally safe for online banking.",
                    Options = new() { "True", "False" },
                    CorrectOptionIndex = 1,
                    IsTrueFalse = true,
                    Explanation = "Public Wi-Fi is unencrypted and can be intercepted. Use a VPN or mobile data for banking."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new() {
                        "Logging in with two different passwords",
                        "A second verification step beyond your password",
                        "Having two separate accounts",
                        "Encryption of your files"
                    },
                    CorrectOptionIndex = 1,
                    Explanation = "2FA requires a second proof of identity (e.g., an OTP or authenticator app) even if your password is stolen."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a sign of a phishing email?",
                    Options = new() {
                        "It comes from your bank's official domain",
                        "It has your full name and personalised content",
                        "It creates urgency and asks you to click a link immediately",
                        "It has no attachments"
                    },
                    CorrectOptionIndex = 2,
                    Explanation = "Phishing emails often create false urgency to pressure you into acting without thinking."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new() {
                        "Software that speeds up your computer",
                        "Malware that encrypts your files and demands payment",
                        "A type of antivirus programme",
                        "A browser extension for privacy"
                    },
                    CorrectOptionIndex = 1,
                    Explanation = "Ransomware locks your data until you pay. Regular backups are your best defence."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software alone is sufficient protection against all cyber threats.",
                    Options = new() { "True", "False" },
                    CorrectOptionIndex = 1,
                    IsTrueFalse = true,
                    Explanation = "Antivirus is one layer. You also need strong passwords, 2FA, software updates, and awareness."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new() {
                        "Building social media platforms",
                        "Manipulating people into revealing confidential information",
                        "Engineering secure social networks",
                        "A type of firewall configuration"
                    },
                    CorrectOptionIndex = 1,
                    Explanation = "Social engineering exploits human psychology rather than technical vulnerabilities."
                },
                new QuizQuestion
                {
                    Question = "Which action best protects you on social media?",
                    Options = new() {
                        "Sharing your location in every post",
                        "Setting your profile to public for more followers",
                        "Regularly reviewing and tightening your privacy settings",
                        "Using your real name and ID number as your username"
                    },
                    CorrectOptionIndex = 2,
                    Explanation = "Privacy settings control who sees your data. Review them regularly as platforms update their policies."
                },
                new QuizQuestion
                {
                    Question = "True or False: Software updates should be delayed as long as possible to avoid bugs.",
                    Options = new() { "True", "False" },
                    CorrectOptionIndex = 1,
                    IsTrueFalse = true,
                    Explanation = "Updates patch security vulnerabilities. Delaying them leaves your device exposed to known exploits."
                }
            };
        }
    }
}
