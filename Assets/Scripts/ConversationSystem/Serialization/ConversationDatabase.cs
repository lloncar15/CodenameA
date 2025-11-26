using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// ScriptableObject database for managing conversation assets.
    /// Provides easy access to conversations in the Unity Editor.
    /// </summary>
    [CreateAssetMenu(fileName = "ConversationDatabase", menuName = "GimGim/Conversation System/Conversation Database")]
    public class ConversationDatabase : ScriptableObject {
        [SerializeField]
        [Tooltip("List of conversation JSON files.")]
        private List<TextAsset> conversationAssets = new List<TextAsset>();

        private Dictionary<string, ConversationData> _loadedConversations;
        private ConversationLoader _loader;

        /// <summary>
        /// Gets the number of conversation assets.
        /// </summary>
        public int Count => conversationAssets.Count;

        /// <summary>
        /// Initializes the database, loading all conversations.
        /// </summary>
        public void Initialize() {
            _loader = new ConversationLoader(useCache: true);
            _loadedConversations = new Dictionary<string, ConversationData>();

            foreach (TextAsset asset in conversationAssets) {
                if (asset == null) {
                    continue;
                }

                ConversationData conversation = _loader.LoadFromTextAsset(asset);
                if (conversation != null) {
                    _loadedConversations[conversation.Id] = conversation;
                }
            }

            Debug.Log($"ConversationDatabase: Loaded {_loadedConversations.Count} conversations.");
        }

        /// <summary>
        /// Gets a conversation by ID.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <returns>The conversation, or null if not found.</returns>
        public ConversationData GetConversation(string conversationId) {
            if (_loadedConversations == null) {
                Initialize();
            }

            return _loadedConversations.GetValueOrDefault(conversationId);
        }

        /// <summary>
        /// Checks if a conversation exists.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasConversation(string conversationId) {
            if (_loadedConversations == null) {
                Initialize();
            }

            return _loadedConversations.ContainsKey(conversationId);
        }

        /// <summary>
        /// Gets all loaded conversation IDs.
        /// </summary>
        /// <returns>Enumerable of conversation IDs.</returns>
        public IEnumerable<string> GetAllConversationIds() {
            if (_loadedConversations == null) {
                Initialize();
            }

            return _loadedConversations.Keys;
        }

        /// <summary>
        /// Reloads all conversations from assets.
        /// </summary>
        public void Reload() {
            _loadedConversations?.Clear();
            _loader?.ClearCache();
            Initialize();
        }

        /// <summary>
        /// Adds a conversation asset to the database.
        /// </summary>
        /// <param name="asset">The TextAsset to add.</param>
        public void AddAsset(TextAsset asset) {
            if (asset != null && !conversationAssets.Contains(asset)) {
                conversationAssets.Add(asset);
            }
        }

        /// <summary>
        /// Removes a conversation asset from the database.
        /// </summary>
        /// <param name="asset">The TextAsset to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool RemoveAsset(TextAsset asset) {
            return conversationAssets.Remove(asset);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor utility to validate all conversation assets.
        /// </summary>
        [ContextMenu("Validate All Conversations")]
        private void ValidateAllConversations() {
            int valid = 0;
            int invalid = 0;

            foreach (TextAsset asset in conversationAssets) {
                if (asset == null) {
                    continue;
                }

                if (ConversationJsonUtility.ValidateJson(asset.text, out string error)) {
                    valid++;
                }
                else {
                    invalid++;
                    Debug.LogWarning($"Conversation '{asset.name}' is invalid: {error}");
                }
            }

            Debug.Log($"Validation complete: {valid} valid, {invalid} invalid.");
        }
#endif
    }
}