using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Saves conversation data to various destinations (files, etc.).
    /// </summary>
    public class ConversationSaver {
        /// <summary>
        /// Saves a conversation to a JSON string.
        /// </summary>
        /// <param name="conversation">The conversation to save.</param>
        /// <param name="prettyPrint">Whether to format the JSON.</param>
        /// <returns>The JSON string.</returns>
        public string SaveToJson(ConversationData conversation, bool prettyPrint = true) {
            return ConversationJsonUtility.ToJson(conversation, prettyPrint);
        }

        /// <summary>
        /// Saves a conversation to a file.
        /// </summary>
        /// <param name="conversation">The conversation to save.</param>
        /// <param name="filePath">The file path to save to.</param>
        /// <param name="prettyPrint">Whether to format the JSON.</param>
        /// <returns>True if saved successfully, false otherwise.</returns>
        public bool SaveToFile(ConversationData conversation, string filePath, bool prettyPrint = true) {
            if (conversation == null) {
                Debug.LogError("ConversationSaver: Conversation is null.");
                return false;
            }

            try {
                string json = SaveToJson(conversation, prettyPrint);
                
                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, json);
                Debug.Log($"ConversationSaver: Saved conversation to {filePath}");
                return true;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationSaver: Error saving to file '{filePath}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates a conversation before saving.
        /// </summary>
        /// <param name="conversation">The conversation to validate.</param>
        /// <param name="errors">List of validation errors.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool Validate(ConversationData conversation, out List<string> errors) {
            return conversation.Validate(out errors);
        }
    }
}