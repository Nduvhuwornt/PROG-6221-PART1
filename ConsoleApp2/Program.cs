using CybersecurityChatbot.Forms;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Application entry point for the Cybersecurity Awareness Chatbot.
    /// PROG6221 — Programming 2A — POE
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
