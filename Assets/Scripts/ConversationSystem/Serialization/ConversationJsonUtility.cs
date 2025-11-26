using System;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Utility class for JSON serialization/deserialization of conversation data.
    /// Wraps Unity's JsonUtility with custom handling for complex types.
    /// </summary>
    public static class ConversationJsonUtility {
        /// <summary>
        /// Serializes a ConversationData object to JSON string.
        /// </summary>
        /// <param name="conversation">The conversation to serialize.</param>
        /// <param name="prettyPrint">Whether to format the JSON for readability.</param>
        /// <returns>The JSON string.</returns>
        public static string ToJson(ConversationData conversation, bool prettyPrint = true) {
            if (conversation == null) {
                return null;
            }

            ConversationJsonSerializer serializer = new();
            RawConversationData raw = serializer.Serialize(conversation);
            
            return JsonUtility.ToJson(raw, prettyPrint);
        }

        /// <summary>
        /// Deserializes a JSON string to a ConversationData object.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized ConversationData, or null if parsing fails.</returns>
        public static ConversationData FromJson(string json) {
            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            try {
                RawConversationData raw = JsonUtility.FromJson<RawConversationData>(json);
                if (raw == null) {
                    Debug.LogError("ConversationJsonUtility: Failed to parse JSON to raw data.");
                    return null;
                }

                var parser = new ConversationJsonParser();
                return parser.Parse(raw);
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationJsonUtility: Error parsing JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates a JSON string without fully parsing it.
        /// </summary>
        /// <param name="json">The JSON string to validate.</param>
        /// <param name="errorMessage">Error message if validation fails.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public static bool ValidateJson(string json, out string errorMessage) {
            if (string.IsNullOrEmpty(json)) {
                errorMessage = "JSON string is null or empty.";
                return false;
            }

            try {
                RawConversationData raw = JsonUtility.FromJson<RawConversationData>(json);
                if (raw == null) {
                    errorMessage = "Failed to parse JSON structure.";
                    return false;
                }

                if (string.IsNullOrEmpty(raw.id)) {
                    errorMessage = "Conversation ID is missing.";
                    return false;
                }

                if (string.IsNullOrEmpty(raw.startNodeId)) {
                    errorMessage = "Start node ID is missing.";
                    return false;
                }

                if (raw.nodes == null || raw.nodes.Count == 0) {
                    errorMessage = "No nodes defined in conversation.";
                    return false;
                }

                errorMessage = null;
                return true;
            }
            catch (Exception ex) {
                errorMessage = $"JSON parsing error: {ex.Message}";
                return false;
            }
        }
    }
}