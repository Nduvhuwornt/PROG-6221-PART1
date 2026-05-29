using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;

namespace CybersecurityChatbot
{
    public partial class ChatbotWindow : Window
    {
        private string userName = "You";
        private string favoriteTopic = "";
        private string currentTopic = "";
        private SoundPlayer greetingPlayer;

        public ChatbotWindow()
        {
            InitializeComponent();
            Loaded += ChatbotWindow_Loaded;
        }

        private void ChatbotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayGreetingSound();
            AddBotMessage("Hello! I'm your Cybersecurity Assistant.\n\nHow can I help you stay safe online today?");
        }

        private void PlayGreetingSound()
        {
            try
            {
                greetingPlayer = new SoundPlayer("greeting.wav");
                greetingPlayer.Play();
            }
            catch { }
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

            Dispatcher.BeginInvoke(() => ProcessUserInput(message));
        }

        private void ProcessUserInput(string input)
        {
            string lower = input.ToLower().Trim();

            // Memory: Name
            if (lower.Contains("my name is") || lower.Contains("call me"))
            {
                userName = input.Split(' ').Last().TrimEnd('.', '!');
                AddBotMessage($"Nice to meet you, {userName}! I'll remember your name.");
                return;
            }

            // Memory: Favorite Topic
            if (lower.Contains("interested in") || lower.Contains("favourite") || lower.Contains("like"))
            {
                if (lower.Contains("privacy")) favoriteTopic = "privacy";
                else if (lower.Contains("password")) favoriteTopic = "passwords";
                else if (lower.Contains("phish") || lower.Contains("scam")) favoriteTopic = "phishing";
                else if (lower.Contains("malware")) favoriteTopic = "malware";

                if (!string.IsNullOrEmpty(favoriteTopic))
                {
                    currentTopic = favoriteTopic;
                    AddBotMessage($"Great! I'll remember that you're interested in **{favoriteTopic}**.");
                    return;
                }
            }

            // Main Response Logic with Multiple Responses
            string response = lower switch
            {
                var s when s.Contains("password") || s.Contains("pass") => GetRandomPasswordTip(),
                var s when s.Contains("phish") || s.Contains("scam") => GetRandomPhishingTip(),
                var s when s.Contains("privacy") => GetRandomPrivacyTip(),
                var s when s.Contains("malware") || s.Contains("virus") => GetRandomMalwareTip(),
                _ => "I'm not sure I understand. Can you try rephrasing? Try asking about passwords, phishing, privacy, or malware."
            };

            AddBotMessage(response);
        }

        // ====================== MULTIPLE RESPONSES ======================
        private string GetRandomPasswordTip()
        {
            string[] tips = {
                "✅ Use strong, unique passwords (16+ characters) with numbers and symbols.",
                "Never reuse the same password across multiple websites.",
                "Consider using a password manager like Bitwarden or LastPass.",
                "Enable 2FA/MFA on all your important accounts."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomPhishingTip()
        {
            string[] tips = {
                "⚠️ Never click suspicious links. Always verify the sender.",
                "Phishing emails often create urgency. Pause and think before acting.",
                "Hover over any link before clicking to see the real destination.",
                "Report phishing attempts to your security team immediately."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomPrivacyTip()
        {
            string[] tips = {
                "🛡️ Review app permissions regularly and revoke unnecessary access.",
                "Use a VPN when connecting to public Wi-Fi.",
                "Be careful about what personal information you share online.",
                "Enable privacy settings on all your social media accounts."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomMalwareTip()
        {
            string[] tips = {
                "🦠 Always keep your operating system and software updated.",
                "Install reputable antivirus software and run regular scans.",
                "Avoid downloading software from untrusted or pirated sources.",
                "Be cautious with USB drives from unknown people."
            };
            return tips[new Random().Next(tips.Length)];
        }

        // ====================== UI METHODS ======================
        private void AddUserMessage(string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(new Run($"{userName}: ") { Foreground = Brushes.LightGreen });
            p.Inlines.Add(new Run(message));
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }

        private void AddBotMessage(string message)
        {
            Paragraph p = new Paragraph(new Run($"🤖 Bot: {message}"));
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }

        private void SetNameButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UserNameInput.Text?.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                userName = name;
                NameStatusBlock.Text = $"✅ Name set to: {userName}";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatDisplay.Document.Blocks.Clear();
            AddBotMessage("Chat has been cleared. How can I help you today?");
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                UserInput.Text = btn.Content.ToString() + " tips";
                SendMessage();
            }
        }
    }
}