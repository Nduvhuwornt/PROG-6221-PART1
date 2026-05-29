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
        private string currentMood = "";           // NEW: Mood Memory
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

            // === MEMORY: Name ===
            if (lower.Contains("my name is") || lower.Contains("call me"))
            {
                userName = input.Split(' ').Last().TrimEnd('.', '!');
                AddBotMessage($"Nice to meet you, {userName}! I'll remember your name.");
                return;
            }

            // === MEMORY: Set / Change Favourite Topic ===
            if (lower.Contains("my favourite topic") || lower.Contains("my favorite topic"))
            {
                if (lower.Contains("privacy")) favoriteTopic = "privacy";
                else if (lower.Contains("password")) favoriteTopic = "passwords";
                else if (lower.Contains("phish") || lower.Contains("scam")) favoriteTopic = "phishing";
                else if (lower.Contains("malware")) favoriteTopic = "malware";

                if (!string.IsNullOrEmpty(favoriteTopic))
                {
                    AddBotMessage($"✅ Updated! Your favourite topic is now **{favoriteTopic}**.");
                    return;
                }
            }

            // === MEMORY: Set / Change Mood ===
            if (lower.Contains("my mood is") || lower.Contains("i feel") || lower.Contains("i'm feeling"))
            {
                if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("anxious"))
                    currentMood = "worried";
                else if (lower.Contains("happy") || lower.Contains("good") || lower.Contains("great"))
                    currentMood = "happy";
                else if (lower.Contains("frustrated") || lower.Contains("angry"))
                    currentMood = "frustrated";
                else if (lower.Contains("confused"))
                    currentMood = "confused";
                else
                    currentMood = "neutral";

                AddBotMessage($"Thanks for sharing! I'll remember you're feeling **{currentMood}** right now.");
                return;
            }

            // === RECALL Mood ===
            if (lower.Contains("how am i feeling") || lower.Contains("my mood"))
            {
                if (!string.IsNullOrEmpty(currentMood))
                    AddBotMessage($"You're currently feeling **{currentMood}**. Is there anything I can help you with?");
                else
                    AddBotMessage("You haven't told me how you're feeling yet. You can say: 'I'm feeling worried'");
                return;
            }

            // Main Response with Mood Awareness
            string response = lower switch
            {
                var s when s.Contains("password") || s.Contains("pass") => GetRandomPasswordTip(),
                var s when s.Contains("phish") || s.Contains("scam") => GetRandomPhishingTip(),
                var s when s.Contains("privacy") => GetRandomPrivacyTip(),
                var s when s.Contains("malware") || s.Contains("virus") => GetRandomMalwareTip(),
                _ => "I'm not sure I understand. Can you try rephrasing?"
            };

            // Empathetic response based on mood
            if (currentMood == "worried")
                response = "I understand you're feeling worried. " + response;
            else if (currentMood == "frustrated")
                response = "I know this can be frustrating. " + response;

            AddBotMessage(response);
        }

        // Random Response Methods
        private string GetRandomPasswordTip()
        {
            string[] tips = {
                "✅ Use strong, unique passwords (16+ characters) with numbers and symbols.",
                "Never reuse the same password across multiple websites.",
                "Consider using a password manager like Bitwarden."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomPhishingTip()
        {
            string[] tips = {
                "⚠️ Never click suspicious links. Always verify the sender.",
                "Phishing emails often create urgency. Pause and think before acting.",
                "Hover over any link before clicking to see the real destination."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomPrivacyTip()
        {
            string[] tips = {
                "🛡️ Review app permissions regularly.",
                "Use a VPN when connecting to public Wi-Fi.",
                "Be careful about sharing personal information online."
            };
            return tips[new Random().Next(tips.Length)];
        }

        private string GetRandomMalwareTip()
        {
            string[] tips = {
                "🦠 Always keep your operating system and software updated.",
                "Install reputable antivirus software.",
                "Avoid downloading from untrusted sources."
            };
            return tips[new Random().Next(tips.Length)];
        }

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