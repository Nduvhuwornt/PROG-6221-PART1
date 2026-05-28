using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Enum to represent different sentiment types
    /// </summary>
    public enum SentimentType
    {
        Neutral,
        Worried,
        Frustrated,
        Curious,
        Happy
    }

    /// <summary>
    /// Analyzes user input to detect sentiment and emotion
    /// </summary>
    public class SentimentAnalyzer
    {
        private Dictionary<string, SentimentType> sentimentKeywords;

        public SentimentAnalyzer()
        {
            InitializeSentimentKeywords();
        }

        /// <summary>
        /// Initialize sentiment keywords and their associated sentiments
        /// </summary>
        private void InitializeSentimentKeywords()
        {
            sentimentKeywords = new Dictionary<string, SentimentType>
            {
                // Worried/Concerned sentiments
                { "worried", SentimentType.Worried },
                { "concerned", SentimentType.Worried },
                { "anxious", SentimentType.Worried },
                { "scared", SentimentType.Worried },
                { "nervous", SentimentType.Worried },
                { "afraid", SentimentType.Worried },
                { "vulnerable", SentimentType.Worried },
                { "threatened", SentimentType.Worried },
                { "danger", SentimentType.Worried },
                { "risk", SentimentType.Worried },

                // Frustrated sentiments
                { "frustrated", SentimentType.Frustrated },
                { "annoyed", SentimentType.Frustrated },
                { "angry", SentimentType.Frustrated },
                { "upset", SentimentType.Frustrated },
                { "tired", SentimentType.Frustrated },
                { "confused", SentimentType.Frustrated },
                { "lost", SentimentType.Frustrated },
                { "struggling", SentimentType.Frustrated },

                // Curious sentiments
                { "curious", SentimentType.Curious },
                { "interested", SentimentType.Curious },
                { "learn", SentimentType.Curious },
                { "know more", SentimentType.Curious },
                { "explain", SentimentType.Curious },
                { "how", SentimentType.Curious },
                { "why", SentimentType.Curious },
                { "tell me", SentimentType.Curious },

                // Happy/Positive sentiments
                { "great", SentimentType.Happy },
                { "excellent", SentimentType.Happy },
                { "good", SentimentType.Happy },
                { "happy", SentimentType.Happy },
                { "love", SentimentType.Happy },
                { "appreciate", SentimentType.Happy },
                { "thank", SentimentType.Happy },
                { "wonderful", SentimentType.Happy }
            };
        }

        /// <summary>
        /// Detect sentiment from user input
        /// </summary>
        public SentimentType DetectSentiment(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return SentimentType.Neutral;
            }

            string lowerInput = userInput.ToLower();
            SentimentType dominantSentiment = SentimentType.Neutral;
            int sentimentCount = 0;

            // Count sentiment keywords and determine dominant sentiment
            foreach (var kvp in sentimentKeywords)
            {
                if (lowerInput.Contains(kvp.Key))
                {
                    if (dominantSentiment == SentimentType.Neutral)
                    {
                        dominantSentiment = kvp.Value;
                    }
                    sentimentCount++;
                }
            }

            // If multiple sentiments detected, return the one found most frequently
            if (sentimentCount > 1)
            {
                return FindMostFrequentSentiment(lowerInput);
            }

            return dominantSentiment;
        }

        /// <summary>
        /// Find the most frequently occurring sentiment in the input
        /// </summary>
        private SentimentType FindMostFrequentSentiment(string input)
        {
            Dictionary<SentimentType, int> sentimentFrequency = new Dictionary<SentimentType, int>();

            foreach (var kvp in sentimentKeywords)
            {
                if (input.Contains(kvp.Key))
                {
                    if (!sentimentFrequency.ContainsKey(kvp.Value))
                    {
                        sentimentFrequency[kvp.Value] = 0;
                    }
                    sentimentFrequency[kvp.Value]++;
                }
            }

            // Return sentiment with highest frequency
            if (sentimentFrequency.Count > 0)
            {
                return sentimentFrequency.OrderByDescending(x => x.Value).First().Key;
            }

            return SentimentType.Neutral;
        }

        /// <summary>
        /// Get a sentiment description
        /// </summary>
        public string GetSentimentDescription(SentimentType sentiment)
        {
            switch (sentiment)
            {
                case SentimentType.Worried:
                    return "Concerned/Worried";
                case SentimentType.Frustrated:
                    return "Frustrated";
                case SentimentType.Curious:
                    return "Curious/Interested";
                case SentimentType.Happy:
                    return "Happy/Positive";
                default:
                    return "Neutral";
            }
        }

        /// <summary>
        /// Add a new sentiment keyword
        /// </summary>
        public void AddSentimentKeyword(string keyword, SentimentType sentiment)
        {
            if (!sentimentKeywords.ContainsKey(keyword.ToLower()))
            {
                sentimentKeywords[keyword.ToLower()] = sentiment;
            }
        }
    }
}
