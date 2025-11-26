using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Serializable data structure for conversation state persistence.
    /// Contains all data needed to save/load conversation history.
    /// </summary>
    [Serializable]
    public class ConversationStateData {
        /// <summary>
        /// Version number for migration support.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Timestamp when this state was last saved.
        /// </summary>
        public string LastSavedUtc { get; set; }

        /// <summary>
        /// Boolean variables set during conversations.
        /// </summary>
        public SerializableDictionary<string, bool> BoolVariables { get; set; }

        /// <summary>
        /// Integer variables set during conversations.
        /// </summary>
        public SerializableDictionary<string, int> IntVariables { get; set; }

        /// <summary>
        /// Float variables set during conversations.
        /// </summary>
        public SerializableDictionary<string, float> FloatVariables { get; set; }

        /// <summary>
        /// String variables set during conversations.
        /// </summary>
        public SerializableDictionary<string, string> StringVariables { get; set; }

        /// <summary>
        /// Node visit counts, keyed by "conversationId:nodeId".
        /// </summary>
        public SerializableDictionary<string, int> NodeVisitCounts { get; set; }

        /// <summary>
        /// Choice history, keyed by "conversationId:nodeId", value is list of choice IDs.
        /// </summary>
        public SerializableDictionary<string, List<string>> ChoiceHistory { get; set; }

        /// <summary>
        /// Last random selection per node, keyed by "conversationId:nodeId".
        /// Used for AvoidRepeat functionality.
        /// </summary>
        public SerializableDictionary<string, int> LastRandomSelections { get; set; }

        /// <summary>
        /// Custom metadata for extensibility.
        /// </summary>
        public SerializableDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Creates a new empty state data.
        /// </summary>
        public ConversationStateData() {
            BoolVariables = new SerializableDictionary<string, bool>();
            IntVariables = new SerializableDictionary<string, int>();
            FloatVariables = new SerializableDictionary<string, float>();
            StringVariables = new SerializableDictionary<string, string>();
            NodeVisitCounts = new SerializableDictionary<string, int>();
            ChoiceHistory = new SerializableDictionary<string, List<string>>();
            LastRandomSelections = new SerializableDictionary<string, int>();
            Metadata = new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// Creates a composite key for node-specific data.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <param name="nodeId">The node ID.</param>
        /// <returns>A composite key string.</returns>
        public static string MakeNodeKey(string conversationId, string nodeId) {
            return $"{conversationId}:{nodeId}";
        }

        /// <summary>
        /// Updates the last saved timestamp to now.
        /// </summary>
        public void UpdateTimestamp() {
            LastSavedUtc = DateTime.UtcNow.ToString("O");
        }
    }
}