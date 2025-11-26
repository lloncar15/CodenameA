using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Registry for custom predicate functions.
    /// Predicates are registered per-conversation to avoid global state pollution.
    /// </summary>
    public class PredicateRegistry {
        private readonly Dictionary<string, Func<SerializableDictionary<string, string>, bool>> _predicates;

        /// <summary>
        /// Creates a new predicate registry.
        /// </summary>
        public PredicateRegistry() {
            _predicates = new Dictionary<string, Func<SerializableDictionary<string, string>, bool>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a copy of another registry.
        /// </summary>
        /// <param name="source">The registry to copy from.</param>
        public PredicateRegistry(PredicateRegistry source) {
            _predicates = new Dictionary<string, Func<SerializableDictionary<string, string>, bool>>(
                source._predicates, 
                StringComparer.OrdinalIgnoreCase
            );
        }

        /// <summary>
        /// Registers a predicate with the given name.
        /// </summary>
        /// <param name="name">The name to register the predicate under.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <returns>True if registered successfully, false if name already exists.</returns>
        public bool Register(string name, Func<SerializableDictionary<string, string>, bool> predicate) {
            if (string.IsNullOrEmpty(name)) {
                Debug.LogWarning("PredicateRegistry: Cannot register predicate with null or empty name.");
                return false;
            }

            if (predicate == null) {
                Debug.LogWarning($"PredicateRegistry: Cannot register null predicate for '{name}'.");
                return false;
            }

            if (_predicates.ContainsKey(name)) {
                Debug.LogWarning($"PredicateRegistry: Predicate '{name}' already registered. Use Replace() to override.");
                return false;
            }

            _predicates[name] = predicate;
            return true;
        }

        /// <summary>
        /// Registers a simple predicate without parameters.
        /// </summary>
        /// <param name="name">The name to register the predicate under.</param>
        /// <param name="predicate">The predicate function (no parameters).</param>
        /// <returns>True if registered successfully, false if name already exists.</returns>
        public bool Register(string name, Func<bool> predicate) {
            if (predicate == null) {
                Debug.LogWarning($"PredicateRegistry: Cannot register null predicate for '{name}'.");
                return false;
            }

            return Register(name, _ => predicate());
        }

        /// <summary>
        /// Replaces an existing predicate or registers a new one.
        /// </summary>
        /// <param name="name">The name of the predicate.</param>
        /// <param name="predicate">The predicate function.</param>
        public void Replace(string name, Func<SerializableDictionary<string, string>, bool> predicate) {
            if (string.IsNullOrEmpty(name) || predicate == null) {
                return;
            }

            _predicates[name] = predicate;
        }

        /// <summary>
        /// Unregisters a predicate by name.
        /// </summary>
        /// <param name="name">The name of the predicate to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool Unregister(string name) {
            return _predicates.Remove(name);
        }

        /// <summary>
        /// Tries to get a predicate by name.
        /// </summary>
        /// <param name="name">The name of the predicate.</param>
        /// <param name="predicate">The predicate if found.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool TryGet(string name, out Func<SerializableDictionary<string, string>, bool> predicate) {
            return _predicates.TryGetValue(name, out predicate);
        }

        /// <summary>
        /// Checks if a predicate is registered.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if registered, false otherwise.</returns>
        public bool Contains(string name) {
            return _predicates.ContainsKey(name);
        }

        /// <summary>
        /// Gets all registered predicate names.
        /// </summary>
        /// <returns>Enumerable of predicate names.</returns>
        public IEnumerable<string> GetRegisteredNames() {
            return _predicates.Keys;
        }

        /// <summary>
        /// Clears all registered predicates.
        /// </summary>
        public void Clear() {
            _predicates.Clear();
        }

        /// <summary>
        /// Gets the number of registered predicates.
        /// </summary>
        public int Count => _predicates.Count;
    }
}