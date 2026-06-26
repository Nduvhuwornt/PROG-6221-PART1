using CybersecurityChatbot.Models;

namespace CybersecurityChatbot.Services
{
    /// <summary>
    /// Core chatbot logic: keyword recognition, sentiment detection, memory, NLP simulation,
    /// random responses, and conversation flow management.
    /// </summary>
    public class ChatbotService
    {
        // ─── User Memory ───────────────────────────────────────────────────────────
        private string _userName = "User";
        private string _favouriteTopic = string.Empty;
        private string _lastTopic = string.Empty;

        // ─── Activity Log ──────────────────────────────────────────────────────────
        private readonly List<ActivityLogEntry> _activityLog = new();

        // ─── Random instance ───────────────────────────────────────────────────────
        private readonly Random _random = new();

        // ─── Keyword response dictionaries ─────────────────────────────────────────
        private readonly Dictionary<string, List<string>> _keywordResponses = new()
        {
            ["password"] = new List<string>
            {
                "🔑 Use strong, unique passwords for every account — at least 12 characters mixing letters, numbers, and symbols.",
                "🔒 Never reuse passwords. Consider a reputable password manager like Bitwarden or 1Password.",
                "🛡️ Enable two-factor authentication (2FA) alongside a strong password for double protection.",
                "⚠️ Avoid using personal info like your name or birthdate in passwords — they're easy to guess."
            },
            ["phishing"] = new List<string>
            {
                "🎣 Phishing emails often mimic trusted organisations. Always verify the sender's address carefully.",
                "🔗 Never click suspicious links in emails or SMS. Hover over links first to preview the destination.",
                "📧 Legitimate organisations will never ask for your password via email. Report and delete such messages.",
                "🚨 If an email creates urgency ('Act now!'), that's a red flag — take a breath and verify independently."
            },
            ["scam"] = new List<string>
            {
                "⚠️ If an offer sounds too good to be true, it almost certainly is. Trust your instincts.",
                "📵 Never share OTPs, banking PINs, or personal details with unsolicited callers.",
                "🏦 South African banks will never request your full PIN or password — hang up and call your bank directly.",
                "🔍 Verify online sellers on trusted platforms and check reviews before making payments."
            },
            ["privacy"] = new List<string>
            {
                "🔒 Review your social media privacy settings regularly — limit who can see your posts and personal info.",
                "🌐 Use a VPN when connecting to public Wi-Fi to encrypt your internet traffic.",
                "🍪 Clear browser cookies and history regularly, and use privacy-focused browsers like Firefox or Brave.",
                "📱 Audit app permissions — many apps request access they don't actually need."
            },
            ["malware"] = new List<string>
            {
                "🦠 Keep your operating system and software updated — patches fix security vulnerabilities.",
                "🛡️ Install a reputable antivirus solution and run regular scans on your devices.",
                "📥 Only download software from official sources. Pirated software often bundles malware.",
                "💾 Back up your data regularly — ransomware can encrypt your files and demand payment."
            },
            ["2fa"] = new List<string>
            {
                "📲 Two-factor authentication (2FA) adds a vital second layer — even if your password is stolen, your account stays safe.",
                "🔐 Use an authenticator app (Google Authenticator, Microsoft Authenticator) instead of SMS for stronger 2FA.",
                "✅ Enable 2FA on all important accounts: email, banking, social media, and cloud storage."
            },
            ["browsing"] = new List<string>
            {
                "🌐 Look for HTTPS and the padlock icon before entering personal information on any website.",
                "🚫 Avoid clicking on pop-up ads — they can redirect you to malicious sites.",
                "🔍 Use search engines with privacy in mind, such as DuckDuckGo, to reduce tracking."
            },
            ["social engineering"] = new List<string>
            {
                "🎭 Social engineering manipulates people rather than systems. Always verify identities before sharing info.",
                "📞 If someone claims to be from IT support or your bank, hang up and call the official number.",
                "🏢 Educate colleagues — many breaches start with a single employee being tricked."
            }
        };

        // ─── Sentiment keyword maps ─────────────────────────────────────────────────
        private readonly Dictionary<string, string> _sentimentResponses = new()
        {
            ["worried"] = "I completely understand your concern — staying safe online can feel overwhelming. Let me share something reassuring: ",
            ["scared"] = "There's no need to panic. Cybersecurity awareness is the best defence. Here's a helpful tip: ",
            ["frustrated"] = "I hear you — cyber threats are genuinely frustrating. Let's tackle this together: ",
            ["confused"] = "No worries at all! Let me break this down simply for you: ",
            ["curious"] = "I love your curiosity! Let's explore that further: ",
            ["happy"] = "Great attitude! A positive mindset is a good start. Here's something useful to know: ",
            ["angry"] = "I understand your frustration. Cybercriminals are relentless, but we can fight back: ",
            ["nervous"] = "It's perfectly natural to feel nervous about online threats. Knowledge is your best shield: "
        };

        // ─── General conversation responses ────────────────────────────────────────
        private readonly Dictionary<string, List<string>> _generalResponses = new()
        {
            ["how are you"] = new List<string>
            {
                "I'm running at full security protocols — thanks for asking! 😊 How can I help keep you safe today?",
                "Fully operational and ready to protect! How can I assist you?",
                "Great, thanks! I'm always on the lookout for cyber threats. What would you like to know?"
            },
            ["purpose"] = new List<string>
            {
                "My purpose is to educate South African citizens about cybersecurity threats and how to stay safe online. 🛡️",
                "I'm your Cybersecurity Awareness Assistant! I help you recognise and avoid online threats."
            },
            ["what can i ask"] = new List<string>
            {
                "You can ask me about: 🔑 Passwords | 🎣 Phishing | 🔒 Privacy | 🦠 Malware | 📲 2FA | 🌐 Safe Browsing | ⚠️ Scams | 🎭 Social Engineering\n\nYou can also use 'add task', 'start quiz', 'show log', and more!",
                "I cover topics like phishing, password safety, malware, privacy, scams, 2FA, and safe browsing. Just ask!"
            },
            ["hello"] = new List<string>
            {
                "Hello {name}! 👋 How can I assist you with cybersecurity today?",
                "Hi there, {name}! Ready to boost your cybersecurity knowledge?",
                "Hey {name}! Great to see you. What cybersecurity topic interests you today?"
            },
            ["thank"] = new List<string>
            {
                "You're welcome, {name}! Stay safe online! 🛡️",
                "Happy to help! Remember: cybersecurity is everyone's responsibility. 💪",
                "Anytime! Feel free to ask more questions, {name}."
            },
            ["bye"] = new List<string>
            {
                "Stay safe online, {name}! 🛡️ Goodbye!",
                "Goodbye, {name}! Remember to stay vigilant against cyber threats.",
                "Take care, {name}! Keep your digital world secure. 👋"
            }
        };

        // ─── Public Properties ──────────────────────────────────────────────────────
        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        public string FavouriteTopic
        {
            get => _favouriteTopic;
            set => _favouriteTopic = value;
        }

        public List<ActivityLogEntry> ActivityLog => _activityLog;

        // ─── Database reference (null = in-memory only) ────────────────────────────
        private DatabaseService? _db;
        private bool _dbAvailable;

        // ─── Constructor ────────────────────────────────────────────────────────────
        public ChatbotService() { }

        /// <summary>
        /// Attaches a DatabaseService so the activity log is saved to and restored
        /// from MySQL across app restarts. Call once after DB is initialised.
        /// </summary>
        public void AttachDatabase(DatabaseService db, bool dbAvailable)
        {
            _db = db;
            _dbAvailable = dbAvailable;
            if (_dbAvailable)
                LoadLogFromDatabase();
        }

        /// <summary>
        /// Loads the last 100 log entries from the database into the in-memory list
        /// so the user sees their full history when the app first opens.
        /// </summary>
        private void LoadLogFromDatabase()
        {
            var saved = _db!.GetRecentLog(100);
            // saved is newest-first; add to the end so existing order is preserved
            foreach (var entry in saved)
                _activityLog.Add(entry);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // CORE RESPONSE ENGINE
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Processes user input and returns an appropriate chatbot response.
        /// Handles NLP simulation, sentiment detection, keyword recognition, and general chat.
        /// </summary>
        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please type a message so I can assist you! 😊";

            string input = userInput.Trim().ToLower();
            LogActivity("NLP", $"User input processed: \"{userInput}\"");

            // ── Sentiment Detection ────────────────────────────────────────────────
            string sentimentPrefix = DetectSentiment(input);

            // ── NLP: Follow-up / more info requests ───────────────────────────────
            if (IsFollowUp(input))
            {
                return HandleFollowUp();
            }

            // ── NLP: Activity log requests ────────────────────────────────────────
            if (ContainsAny(input, "show log", "activity log", "what have you done", "recent actions", "show activity"))
            {
                return GetActivityLogSummary();
            }

            // ── NLP: Favourite topic memory ───────────────────────────────────────
            if (ContainsAny(input, "interested in", "i love", "i like", "favourite topic", "i care about"))
            {
                return HandleInterestDeclaration(input);
            }

            // ── NLP: Remember-based recall ────────────────────────────────────────
            if (ContainsAny(input, "remember me", "what do you know about me", "my topic", "my favourite"))
            {
                return RecallUserInfo();
            }

            // ── Keyword Recognition ───────────────────────────────────────────────
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    _lastTopic = keyword;
                    LogActivity("Chat", $"Keyword match: {keyword}");
                    string keywordResponse = GetRandomResponse(_keywordResponses[keyword]);
                    return sentimentPrefix + keywordResponse + GetPersonalisedSuffix(keyword);
                }
            }

            // ── General Conversation ──────────────────────────────────────────────
            foreach (var key in _generalResponses.Keys)
            {
                if (input.Contains(key))
                {
                    LogActivity("Chat", $"General response triggered: {key}");
                    return sentimentPrefix + GetRandomResponse(_generalResponses[key]).Replace("{name}", _userName);
                }
            }

            // ── Greetings ─────────────────────────────────────────────────────────
            if (ContainsAny(input, "hi ", "hi!", "hey", "hello", "good morning", "good afternoon", "good evening"))
            {
                return $"Hello, {_userName}! 👋 How can I assist you with cybersecurity today?";
            }

            // ── Default / unrecognised ─────────────────────────────────────────────
            LogActivity("Chat", "Unrecognised input — default response returned");
            return $"I'm not sure I understand that, {_userName}. Could you rephrase? 🤔\n\nTry asking about: passwords, phishing, scams, privacy, malware, 2FA, or browsing safety.";
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // SENTIMENT DETECTION
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Detects emotional sentiment in user input and returns an empathetic prefix.
        /// </summary>
        private string DetectSentiment(string input)
        {
            foreach (var sentiment in _sentimentResponses)
            {
                if (input.Contains(sentiment.Key))
                {
                    LogActivity("Chat", $"Sentiment detected: {sentiment.Key}");
                    return sentiment.Value;
                }
            }
            return string.Empty;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // CONVERSATION FLOW — FOLLOW-UP HANDLING
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Checks whether the user is asking for more information on the current topic.
        /// </summary>
        private bool IsFollowUp(string input)
        {
            return ContainsAny(input, "tell me more", "explain more", "give me another", "more info",
                               "continue", "and then", "what else", "more tips", "another tip");
        }

        /// <summary>
        /// Provides a follow-up response on the last discussed topic.
        /// </summary>
        private string HandleFollowUp()
        {
            if (!string.IsNullOrEmpty(_lastTopic) && _keywordResponses.ContainsKey(_lastTopic))
            {
                string response = GetRandomResponse(_keywordResponses[_lastTopic]);
                LogActivity("Chat", $"Follow-up response on topic: {_lastTopic}");
                return $"Sure! Here's more about {_lastTopic}:\n\n{response}";
            }
            return $"I'd love to tell you more! What topic would you like to explore, {_userName}? Try: password, phishing, privacy, scam, malware, or 2FA.";
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // MEMORY & RECALL
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Stores the user's declared topic of interest.
        /// </summary>
        private string HandleInterestDeclaration(string input)
        {
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    _favouriteTopic = keyword;
                    LogActivity("Chat", $"User interest recorded: {keyword}");
                    return $"Great! I'll remember that you're interested in {keyword}, {_userName}. It's a crucial part of staying safe online. 🎯\n\nHere's a quick tip on {keyword}:\n{GetRandomResponse(_keywordResponses[keyword])}";
                }
            }
            return $"I've noted your interest, {_userName}! Could you mention a specific topic like password safety, phishing, or privacy?";
        }

        /// <summary>
        /// Recalls stored information about the user.
        /// </summary>
        private string RecallUserInfo()
        {
            string info = $"Here's what I know about you, {_userName}:\n";
            info += $"• Name: {_userName}\n";
            if (!string.IsNullOrEmpty(_favouriteTopic))
                info += $"• Favourite topic: {_favouriteTopic}\n\nAs someone interested in {_favouriteTopic}, you might want to review the security settings on your accounts. 🔐";
            else
                info += "• No favourite topic recorded yet. Tell me what you're interested in!";
            return info;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ACTIVITY LOG
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Adds an entry to the activity log with a category and description.
        /// </summary>
        public void LogActivity(string category, string action)
        {
            var entry = new ActivityLogEntry
            {
                Category = category,
                Action = action,
                Timestamp = DateTime.Now
            };

            // Keep the in-memory list capped at 200 entries (newest first)
            _activityLog.Insert(0, entry);
            if (_activityLog.Count > 200)
                _activityLog.RemoveAt(_activityLog.Count - 1);

            // Persist to MySQL so it survives app restarts
            if (_dbAvailable && _db != null)
                _db.SaveLogEntry(entry);
        }

        /// <summary>
        /// Returns a formatted summary of the last 10 activity log entries.
        /// </summary>
        public string GetActivityLogSummary(int count = 10)
        {
            if (_activityLog.Count == 0)
                return "No activity recorded yet. Start chatting, adding tasks, or playing the quiz!";

            var entries = _activityLog.Take(count).ToList();
            string header = _dbAvailable
                ? $"📋 Activity Log — last {entries.Count} of {_activityLog.Count} actions (saved across sessions):\n\n"
                : $"📋 Activity Log — last {entries.Count} actions (session only — no DB):\n\n";

            string summary = header;
            for (int i = 0; i < entries.Count; i++)
                summary += $"{i + 1}. {entries[i]}\n";

            return summary;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // UTILITY HELPERS
        // ═══════════════════════════════════════════════════════════════════════════

        private string GetRandomResponse(List<string> responses)
        {
            return responses[_random.Next(responses.Count)];
        }

        private static bool ContainsAny(string input, params string[] keywords)
        {
            return keywords.Any(k => input.Contains(k));
        }

        private string GetPersonalisedSuffix(string topic)
        {
            if (!string.IsNullOrEmpty(_favouriteTopic) && _favouriteTopic == topic)
                return $"\n\n💡 Since {topic} is your favourite topic, {_userName}, you're already ahead of the game!";
            return string.Empty;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ASCII ART
        // ═══════════════════════════════════════════════════════════════════════════
        s
        public static string GetAsciiArt()
        {
            return @"
  ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗███████╗ ██████╗
 ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝██╔════╝
 ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗█████╗  ██║     
 ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██╔══╝  ██║     
 ╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║███████╗╚██████╗
  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝ ╚═════╝
        ╔═══════════════════════════════════════════════════╗
        ║   🛡️  CYBERSECURITY AWARENESS ASSISTANT  🛡️      ║
        ║        Protecting South African Citizens          ║
        ╚═══════════════════════════════════════════════════╝";
        }
    }
}
