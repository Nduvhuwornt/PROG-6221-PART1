using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class ChatbotWindow : Window
    {
        private string userName = "You";

        public ChatbotWindow()
        {
            InitializeComponent();
            Loaded += ChatbotWindow_Loaded;
        }

        private void ChatbotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddBotMessage("Hello! I'm your Cybersecurity Assistant.\n\nHow can I help you stay safe online today?");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        private void SendMessage()
        {
            string message = UserInput.Text?.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddUserMessage(message);
            UserInput.Clear();

            Dispatcher.BeginInvoke(new Action(() => ProcessUserInput(message)));
        }

        private void AddUserMessage(string message)
        {
            var p = new Paragraph();
            p.Inlines.Add(new Run($"{userName}: ") { Foreground = Brushes.LightGreen });
            p.Inlines.Add(new Run(message));
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }

        private void AddBotMessage(string message)
        {
            var p = new Paragraph(new Run($"🤖 Bot: {message}"));
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }

        private void ProcessUserInput(string input)
        {
            string lower = input.ToLower();

            string response = lower switch
            {
                var s when s.Contains("password") || s.Contains("pass") => "✅ Use strong unique passwords (16+ chars), enable 2FA, and use a password manager.",
                var s when s.Contains("phish") || s.Contains("scam") => "⚠️ Never click suspicious links. Always verify the sender.",
                var s when s.Contains("privacy") => "🛡️ Review app permissions and use VPN on public Wi-Fi.",
                var s when s.Contains("malware") || s.Contains("virus") => "🦠 Keep your system updated and use good antivirus software.",
                _ => "Try asking about **passwords**, **phishing**, **privacy**, or **malware**!"
            };

            AddBotMessage(response);
        }

        private void SetNameButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UserNameInput.Text?.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                userName = name;
                NameStatusBlock.Text = $"✅ Name set to: {userName}";
            }
            else
            {
                NameStatusBlock.Text = "❌ Please enter a name";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatDisplay.Document.Blocks.Clear();
            AddBotMessage("Chat cleared. How can I help you today?");
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                UserInput.Text = btn.Content.ToString() + " tips please";
                SendMessage();
            }
        }
    }
}