using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Loads conversation data from various sources (files, TextAssets, Resources).
    /// </summary>
    public class ConversationLoader {
        private readonly Dictionary<string, ConversationData> _cache;
        private readonly bool _useCache;

        /// <summary>
        /// Creates a new conversation loader.
        /// </summary>
        /// <param name="useCache">Whether to cache loaded conversations.</param>
        public ConversationLoader(bool useCache = true) {
            _useCache = useCache;
            _cache = new Dictionary<string, ConversationData>();
        }

        /// <summary>
        /// Loads a conversation from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="cacheKey">Optional cache key. If null, conversation ID is used.</param>
        /// <returns>The loaded conversation, or null if loading fails.</returns>
        public ConversationData LoadFromJson(string json, string cacheKey = null) {
            ConversationData conversation = ConversationJsonUtility.FromJson(json);
            
            if (conversation != null && _useCache) {
                string key = cacheKey ?? conversation.Id;
                _cache[key] = conversation;
            }

            return conversation;
        }

        /// <summary>
        /// Loads a conversation from a TextAsset.
        /// </summary>
        /// <param name="textAsset">The TextAsset containing JSON.</param>
        /// <returns>The loaded conversation, or null if loading fails.</returns>
        public ConversationData LoadFromTextAsset(TextAsset textAsset) {
            if (!textAsset) {
                Debug.LogError("ConversationLoader: TextAsset is null.");
                return null;
            }

            return LoadFromJson(textAsset.text, textAsset.name);
        }

        /// <summary>
        /// Loads a conversation from a file path.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>The loaded conversation, or null if loading fails.</returns>
        public ConversationData LoadFromFile(string filePath) {
            if (!File.Exists(filePath)) {
                Debug.LogError($"ConversationLoader: File not found: {filePath}");
                return null;
            }

            try {
                string json = File.ReadAllText(filePath);
                return LoadFromJson(json, Path.GetFileNameWithoutExtension(filePath));
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationLoader: Error reading file '{filePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads a conversation from Resources.
        /// </summary>
        /// <param name="resourcePath">The path within Resources (without extension).</param>
        /// <returns>The loaded conversation, or null if loading fails.</returns>
        public ConversationData LoadFromResources(string resourcePath) {
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null) {
                Debug.LogError($"ConversationLoader: Resource not found: {resourcePath}");
                return null;
            }

            return LoadFromTextAsset(textAsset);
        }

        /// <summary>
        /// Gets a conversation from cache.
        /// </summary>
        /// <param name="key">The cache key (conversation ID or custom key).</param>
        /// <returns>The cached conversation, or null if not found.</returns>
        public ConversationData GetFromCache(string key) {
            return _cache.GetValueOrDefault(key);
        }

        /// <summary>
        /// Checks if a conversation is cached.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if cached, false otherwise.</returns>
        public bool IsCached(string key) {
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// Removes a conversation from cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool RemoveFromCache(string key) {
            return _cache.Remove(key);
        }

        /// <summary>
        /// Clears the conversation cache.
        /// </summary>
        public void ClearCache() {
            _cache.Clear();
        }

        /// <summary>
        /// Gets all cached conversation IDs.
        /// </summary>
        /// <returns>Enumerable of cache keys.</returns>
        public IEnumerable<string> GetCachedKeys() {
            return _cache.Keys;
        }

        /// <summary>
        /// Gets the number of cached conversations.
        /// </summary>
        public int CacheCount => _cache.Count;
    }
}