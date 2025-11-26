using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Serializable data structure representing a command to execute.
    /// Used for OnEnter/OnExit commands on nodes.
    /// </summary>
    [Serializable]
    public class CommandData {
        /// <summary>
        /// The type/name of the command to execute.
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// Parameters for the command, stored as key-value pairs.
        /// </summary>
        public SerializableDictionary<string, string> Parameters { get; set; }

        public CommandData() {
            Parameters = new SerializableDictionary<string, string>();
        }

        public CommandData(string commandType) : this() {
            CommandType = commandType;
        }

        /// <summary>
        /// Gets a parameter value as a string.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The parameter value or default.</returns>
        public string GetString(string key, string defaultValue = "") {
            return Parameters.GetOrDefault(key, defaultValue);
        }

        /// <summary>
        /// Gets a parameter value as an integer.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found or parse fails.</param>
        /// <returns>The parameter value as int or default.</returns>
        public int GetInt(string key, int defaultValue = 0) {
            if (Parameters.TryGetValue(key, out string value) && int.TryParse(value, out int result)) {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a parameter value as a float.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found or parse fails.</param>
        /// <returns>The parameter value as float or default.</returns>
        public float GetFloat(string key, float defaultValue = 0f) {
            if (Parameters.TryGetValue(key, out string value) && float.TryParse(value, out float result)) {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a parameter value as a boolean.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found or parse fails.</param>
        /// <returns>The parameter value as bool or default.</returns>
        public bool GetBool(string key, bool defaultValue = false) {
            if (Parameters.TryGetValue(key, out string value) && bool.TryParse(value, out bool result)) {
                return result;
            }
            return defaultValue;
        }
    }
}