using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A ConversationStateManager that automatically persists state changes.
    /// </summary>
    public class PersistentConversationStateManager : ConversationStateManager {
        private readonly ConversationStatePersistence _persistence;
        private bool _autoSave;
        private float _lastSaveTime;
        private float _autoSaveInterval = 30f; // seconds
        private bool _isDirty;

        /// <summary>
        /// Gets or sets whether auto-save is enabled.
        /// </summary>
        public bool AutoSave {
            get => _autoSave;
            set => _autoSave = value;
        }

        /// <summary>
        /// Gets or sets the auto-save interval in seconds.
        /// </summary>
        public float AutoSaveInterval {
            get => _autoSaveInterval;
            set => _autoSaveInterval = Mathf.Max(1f, value);
        }

        /// <summary>
        /// Creates a new persistent state manager.
        /// </summary>
        /// <param name="persistence">The persistence service to use.</param>
        /// <param name="autoSave">Whether to enable auto-save.</param>
        public PersistentConversationStateManager(ConversationStatePersistence persistence, bool autoSave = true) {
            _persistence = persistence ?? new ConversationStatePersistence();
            _autoSave = autoSave;
            _lastSaveTime = Time.realtimeSinceStartup;

            // Subscribe to state changes
            OnVariableChanged += (_, _) => MarkDirty();
            OnNodeVisited += (_, _, _) => MarkDirty();
            OnChoiceMade += (_, _, _) => MarkDirty();
        }

        /// <summary>
        /// Loads state from disk.
        /// </summary>
        public void LoadFromDisk() {
            ConversationStateData data = _persistence.Load();
            LoadStateData(data);
            _isDirty = false;
        }

        /// <summary>
        /// Saves state to disk.
        /// </summary>
        /// <returns>True if saved successfully.</returns>
        public bool SaveToDisk() {
            bool success = _persistence.Save(GetStateData());
            if (success) {
                _isDirty = false;
                _lastSaveTime = Time.realtimeSinceStartup;
            }
            return success;
        }

        /// <summary>
        /// Marks the state as dirty (needing save).
        /// </summary>
        private void MarkDirty() {
            _isDirty = true;
        }

        /// <summary>
        /// Call this periodically (e.g., from Update) to check for auto-save.
        /// </summary>
        public void Tick() {
            if (!_autoSave || !_isDirty) {
                return;
            }

            if (Time.realtimeSinceStartup - _lastSaveTime >= _autoSaveInterval) {
                SaveToDisk();
            }
        }

        /// <summary>
        /// Forces an immediate save if dirty.
        /// </summary>
        public void FlushIfDirty() {
            if (_isDirty) {
                SaveToDisk();
            }
        }

        /// <summary>
        /// Creates a backup of the current save.
        /// </summary>
        /// <returns>True if backup created.</returns>
        public bool CreateBackup() {
            return _persistence.CreateBackup();
        }

        /// <summary>
        /// Deletes the save file and resets state.
        /// </summary>
        public void DeleteSaveAndReset() {
            _persistence.DeleteSave();
            Reset();
            _isDirty = false;
        }
    }
}