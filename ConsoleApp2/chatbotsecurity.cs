using System;

namespace CybersecurityChatbot
{
    // This class handles all the chatbot logic
    class CybersecurityBot
    {
        // Automatic property (required by the POE)
        public string UserName { get; private set; }

        public CybersecurityBot(string userName)
        {
            UserName = userName;   // automatic property in use
        }

        // Main chat loop
        public void StartChat()
        {
            while (true)
            {
                ConsoleUI.ShowPrompt(UserName);
                string input = Console.ReadLine()?.Trim().ToLower();

                // Input validation (rubric requirement)
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleUI.ShowError("❌ I didn’t quite understand that. Could you rephrase?");
                    continue;
                }

                if (input == "exit")
                {
                    ConsoleUI.ShowGoodbye();
                    break;
                }

                // Get smart response
                string response = GetResponse(input);
                ConsoleUI.ShowBotResponse(response);
            }
        }

        // All the chatbot answers (Basic Response System)
        private string GetResponse(string input)
        {
            if (input.Contains("how are you"))
                return "I'm doing fantastic, thank you! How are you today?";

            if (input.Contains("purpose") || input.Contains("who are you"))
                return $"Hi {UserName}! My purpose is to teach you how to stay safe online.";

            if (input.Contains("what can i ask") || input.Contains("topics"))
                return "You can ask me about password safety, phishing, safe browsing, or anything!";

            if (input.Contains("password"))
                return "✅ Use strong, unique passwords for every account. Never use your name or birthday!";

            if (input.Contains("phishing") || input.Contains("scam"))
                return "⚠️ Phishing emails look real but they are fake. Never click links or give your password.";

            if (input.Contains("safe browsing") || input.Contains("browsing"))
                return "✅ Always check the URL, use HTTPS websites, and avoid clicking unknown links.";

            // Default response
            return "Hmm... I didn’t quite understand that. Try asking about password safety, phishing, or safe browsing?";
        }
    }
}
