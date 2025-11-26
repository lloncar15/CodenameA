using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A dictionary wrapper that can be serialized to JSON.
    /// Used for metadata and extensible properties.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
        
        /// <summary>
        /// Tries to get a value and returns a default if not found.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value if key is not found.</param>
        /// <returns>The value if found, otherwise the default value.</returns>
        public TValue GetOrDefault(TKey key, TValue defaultValue = default) {
            return TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        /// <summary>
        /// Sets a value, adding the key if it doesn't exist.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to set.</param>
        public void Set(TKey key, TValue value) {
            this[key] = value;
        }
    }
}