using System.Collections.Generic;
using System.Linq;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Combines multiple variable providers into one.
    /// Queries providers in priority order (highest first).
    /// </summary>
    public class CompositeVariableProvider : IConversationVariableProvider {
        private readonly List<IConversationVariableProvider> _providers;
        private bool _isSorted;

        /// <inheritdoc />
        public int Priority => int.MaxValue; // Composite is always highest when nested

        /// <summary>
        /// Creates a new composite variable provider.
        /// </summary>
        public CompositeVariableProvider() {
            _providers = new List<IConversationVariableProvider>();
            _isSorted = true;
        }

        /// <summary>
        /// Creates a composite provider with initial providers.
        /// </summary>
        /// <param name="providers">Initial providers to add.</param>
        public CompositeVariableProvider(IEnumerable<IConversationVariableProvider> providers) : this() {
            foreach (IConversationVariableProvider provider in providers) {
                AddProvider(provider);
            }
        }

        /// <summary>
        /// Adds a provider to the composite.
        /// </summary>
        /// <param name="provider">The provider to add.</param>
        public void AddProvider(IConversationVariableProvider provider) {
            if (provider == null || provider == this) {
                return;
            }

            _providers.Add(provider);
            _isSorted = false;
        }

        /// <summary>
        /// Removes a provider from the composite.
        /// </summary>
        /// <param name="provider">The provider to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool RemoveProvider(IConversationVariableProvider provider) {
            return _providers.Remove(provider);
        }

        /// <summary>
        /// Clears all providers.
        /// </summary>
        public void Clear() {
            _providers.Clear();
            _isSorted = true;
        }

        /// <summary>
        /// Gets the number of providers.
        /// </summary>
        public int Count => _providers.Count;

        /// <summary>
        /// Ensures providers are sorted by priority (descending).
        /// </summary>
        private void EnsureSorted() {
            if (_isSorted) 
                return;
            
            _providers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            _isSorted = true;
        }

        /// <inheritdoc />
        public bool TryGetBool(string variableName, out bool value) {
            EnsureSorted();
            
            foreach (IConversationVariableProvider provider in _providers) {
                if (provider.TryGetBool(variableName, out value)) {
                    return true;
                }
            }

            value = false;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetInt(string variableName, out int value) {
            EnsureSorted();
            
            foreach (IConversationVariableProvider provider in _providers) {
                if (provider.TryGetInt(variableName, out value)) {
                    return true;
                }
            }

            value = 0;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetFloat(string variableName, out float value) {
            EnsureSorted();
            
            foreach (IConversationVariableProvider provider in _providers) {
                if (provider.TryGetFloat(variableName, out value)) {
                    return true;
                }
            }

            value = 0;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetString(string variableName, out string value) {
            EnsureSorted();
            
            foreach (IConversationVariableProvider provider in _providers) {
                if (provider.TryGetString(variableName, out value)) {
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <inheritdoc />
        public bool HasVariable(string variableName) {
            EnsureSorted();
            return _providers.Any(p => p.HasVariable(variableName));
        }
    }
}