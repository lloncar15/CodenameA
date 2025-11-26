using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A simple variable provider backed by dictionaries.
    /// Useful for testing or simple game state.
    /// </summary>
    public class DictionaryVariableProvider : IConversationVariableProvider {
        private readonly Dictionary<string, bool> _bools;
        private readonly Dictionary<string, int> _ints;
        private readonly Dictionary<string, float> _floats;
        private readonly Dictionary<string, string> _strings;

        /// <inheritdoc />
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Creates a new dictionary variable provider.
        /// </summary>
        /// <param name="priority">The priority of this provider.</param>
        public DictionaryVariableProvider(int priority = 0) {
            Priority = priority;
            _bools = new Dictionary<string, bool>();
            _ints = new Dictionary<string, int>();
            _floats = new Dictionary<string, float>();
            _strings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets a boolean variable.
        /// </summary>
        public void SetBool(string name, bool value) {
            _bools[name] = value;
        }

        /// <summary>
        /// Sets an integer variable.
        /// </summary>
        public void SetInt(string name, int value) {
            _ints[name] = value;
        }

        /// <summary>
        /// Sets a float variable.
        /// </summary>
        public void SetFloat(string name, float value) {
            _floats[name] = value;
        }

        /// <summary>
        /// Sets a string variable.
        /// </summary>
        public void SetString(string name, string value) {
            _strings[name] = value;
        }

        /// <summary>
        /// Removes a variable of any type.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns>True if removed from any dictionary.</returns>
        public bool Remove(string name) {
            bool removed = false;
            removed |= _bools.Remove(name);
            removed |= _ints.Remove(name);
            removed |= _floats.Remove(name);
            removed |= _strings.Remove(name);
            return removed;
        }

        /// <summary>
        /// Clears all variables.
        /// </summary>
        public void Clear() {
            _bools.Clear();
            _ints.Clear();
            _floats.Clear();
            _strings.Clear();
        }

        /// <inheritdoc />
        public bool TryGetBool(string variableName, out bool value) {
            return _bools.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetInt(string variableName, out int value) {
            return _ints.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetFloat(string variableName, out float value) {
            return _floats.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetString(string variableName, out string value) {
            return _strings.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        public bool HasVariable(string variableName) {
            return _bools.ContainsKey(variableName) ||
                   _ints.ContainsKey(variableName) ||
                   _floats.ContainsKey(variableName) ||
                   _strings.ContainsKey(variableName);
        }
    }
}