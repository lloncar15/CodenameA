using System;
using System.IO;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Handles saving and loading conversation state to/from JSON files.
    /// </summary>
    public class ConversationStatePersistence {
        private readonly string _savePath;
        private readonly string _fileName;

        /// <summary>
        /// Gets the full file path for the save file.
        /// </summary>
        public string FilePath => Path.Combine(_savePath, _fileName);

        /// <summary>
        /// Creates a new persistence service.
        /// </summary>
        /// <param name="fileName">The save file name (default: "conversation_state.json").</param>
        /// <param name="savePath">The save directory (default: Application.persistentDataPath).</param>
        public ConversationStatePersistence(string fileName = "conversation_state.json", string savePath = null) {
            _fileName = fileName;
            _savePath = savePath ?? Application.persistentDataPath;
        }

        /// <summary>
        /// Saves the state data to a JSON file.
        /// </summary>
        /// <param name="data">The state data to save.</param>
        /// <returns>True if saved successfully, false otherwise.</returns>
        public bool Save(ConversationStateData data) {
            if (data == null) {
                Debug.LogWarning("ConversationStatePersistence: Cannot save null data.");
                return false;
            }

            try {
                data.UpdateTimestamp();
                string json = JsonUtility.ToJson(data, true);
                
                // Ensure directory exists
                if (!Directory.Exists(_savePath)) {
                    Directory.CreateDirectory(_savePath);
                }

                File.WriteAllText(FilePath, json);
                Debug.Log($"ConversationStatePersistence: Saved state to {FilePath}");
                return true;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationStatePersistence: Failed to save state: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads state data from a JSON file.
        /// </summary>
        /// <returns>The loaded state data, or a new empty state if file doesn't exist or load fails.</returns>
        public ConversationStateData Load() {
            if (!File.Exists(FilePath)) {
                Debug.Log($"ConversationStatePersistence: No save file found at {FilePath}, returning new state.");
                return new ConversationStateData();
            }

            try {
                string json = File.ReadAllText(FilePath);
                ConversationStateData data = JsonUtility.FromJson<ConversationStateData>(json);
                
                if (data == null) {
                    Debug.LogWarning("ConversationStatePersistence: Failed to parse save file, returning new state.");
                    return new ConversationStateData();
                }

                Debug.Log($"ConversationStatePersistence: Loaded state from {FilePath}");
                return data;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationStatePersistence: Failed to load state: {ex.Message}");
                return new ConversationStateData();
            }
        }

        /// <summary>
        /// Checks if a save file exists.
        /// </summary>
        /// <returns>True if save file exists, false otherwise.</returns>
        public bool SaveExists() {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// Deletes the save file.
        /// </summary>
        /// <returns>True if deleted or didn't exist, false on error.</returns>
        public bool DeleteSave() {
            try {
                if (File.Exists(FilePath)) {
                    File.Delete(FilePath);
                    Debug.Log($"ConversationStatePersistence: Deleted save file at {FilePath}");
                }
                return true;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationStatePersistence: Failed to delete save file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates a backup of the current save file.
        /// </summary>
        /// <param name="backupSuffix">Suffix to add to backup file name.</param>
        /// <returns>True if backup created, false otherwise.</returns>
        public bool CreateBackup(string backupSuffix = ".backup") {
            if (!File.Exists(FilePath)) {
                return false;
            }

            try {
                string backupPath = FilePath + backupSuffix;
                File.Copy(FilePath, backupPath, true);
                Debug.Log($"ConversationStatePersistence: Created backup at {backupPath}");
                return true;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationStatePersistence: Failed to create backup: {ex.Message}");
                return false;
            }
        }
    }
}