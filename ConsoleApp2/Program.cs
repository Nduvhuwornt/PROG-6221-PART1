using System;

namespace CybersecurityChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Play voice greeting
            ConsoleUI.PlayVoiceGreeting();

            // Show beautiful ASCII art
            ConsoleUI.DisplayAsciiArt();

            // Get user's name and start the bot
            string userName = ConsoleUI.GetUserName();
            CybersecurityBot bot = new CybersecurityBot(userName);

            ConsoleUI.ShowWelcomeMessage(userName);
            ConsoleUI.ShowInstructions();

            // Start chatting
            bot.StartChat();
        }
    }
}
