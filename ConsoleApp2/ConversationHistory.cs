using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Represents a single exchange in the conversation
    /// </summary>
    public class ConversationExchange
    {
        public string UserInput { get; set; }
        public string BotResponse { get; set; }
        public DateTime Timestamp { get; set; }

        public ConversationExchange(string userInput, string botResponse)
        {
            UserInput = userInput;
            BotResponse = botResponse;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Maintains conversation history for memory and recall functionality
    /// </summary>
    public class ConversationHistory
    {
        private List<ConversationExchange> exchanges;
        private const int MaxHistoryLength = 50; // Keep last 50 exchanges

        public ConversationHistory()
        {
            exchanges = new List<ConversationExchange>();
        }

        /// <summary>
        /// Add a conversation exchange
        /// </summary>
        public void AddExchange(string userInput, string botResponse)
        {
            exchanges.Add(new ConversationExchange(userInput, botResponse));

            // Remove oldest exchange if max length exceeded
            if (exchanges.Count > MaxHistoryLength)
            {
                exchanges.RemoveAt(0);
            }
        }

        /// <summary>
        /// Get all conversation exchanges
        /// </summary>
        public List<ConversationExchange> GetAllExchanges()
        {
            return new List<ConversationExchange>(exchanges);
        }

        /// <summary>
        /// Get last N exchanges
        /// </summary>
        public List<ConversationExchange> GetLastExchanges(int count)
        {
            int startIndex = Math.Max(0, exchanges.Count - count);
            return exchanges.Skip(startIndex).ToList();
        }

        /// <summary>
        /// Get the most recent exchange
        /// </summary>
        public ConversationExchange GetLastExchange()
        {
            return exchanges.Count > 0 ? exchanges[exchanges.Count - 1] : null;
        }

        /// <summary>
        /// Get all exchanges related to a specific topic
        /// </summary>
        public List<ConversationExchange> GetExchangesByTopic(string topic)
        {
            string lowerTopic = topic.ToLower();
            return exchanges
                .Where(e => e.UserInput.ToLower().Contains(lowerTopic) ||
                           e.BotResponse.ToLower().Contains(lowerTopic))
                .ToList();
        }

        /// <summary>
        /// Check if a topic has been discussed
        /// </summary>
        public bool HasDiscussedTopic(string topic)
        {
            return GetExchangesByTopic(topic).Count > 0;
        }

        /// <summary>
        /// Get conversation summary
        /// </summary>
        public string GetConversationSummary()
        {
            if (exchanges.Count == 0)
            {
                return "No conversation history yet.";
            }

            string summary = $"Conversation Summary ({exchanges.Count} exchanges):\n";
            summary += "-----------------------------------\n";

            foreach (var exchange in GetLastExchanges(10))
            {
                summary += $"[{exchange.Timestamp:HH:mm:ss}] User: {exchange.UserInput}\n";
                summary += $"Bot: {exchange.BotResponse}\n";
                summary += "-----------------------------------\n";
            }

            return summary;
        }

        /// <summary>
        /// Clear conversation history
        /// </summary>
        public void ClearHistory()
        {
            exchanges.Clear();
        }

        /// <summary>
        /// Get total number of exchanges
        /// </summary>
        public int GetExchangeCount()
        {
            return exchanges.Count;
        }

        /// <summary>
        /// Get topics discussed in order
        /// </summary>
        public List<string> GetDiscussedTopics()
        {
            var topics = new List<string>();
            var topicKeywords = new[] { "password", "phishing", "privacy", "malware", "browsing", "scam", "virus" };

            foreach (var exchange in exchanges)
            {
                foreach (var keyword in topicKeywords)
                {
                    if ((exchange.UserInput.ToLower().Contains(keyword) ||
                         exchange.BotResponse.ToLower().Contains(keyword)) &&
                        !topics.Contains(keyword))
                    {
                        topics.Add(keyword);
                    }
                }
            }

            return topics;
        }
    }
}
