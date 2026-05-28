using System;
using System.Windows;

namespace CybersecurityChatbot
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();
            var window = new ChatbotWindow();
            app.Run(window);
        }
    }
}