namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents the result of parsing a text command.
    /// </summary>
    public class TextCommandResult {
        /// <summary>
        /// The type/name of the command.
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// The parameter value (if single parameter).
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// Additional parameters (for commands with multiple params).
        /// </summary>
        public SerializableDictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// The original matched text including brackets.
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        /// The start index in the original string.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// The length of the original text.
        /// </summary>
        public int Length { get; set; }

        public TextCommandResult() {
            Parameters = new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// Gets the parameter as a float.
        /// </summary>
        /// <param name="defaultValue">Default if parse fails.</param>
        /// <returns>The float value.</returns>
        public float GetFloat(float defaultValue = 0f) {
            if (float.TryParse(Parameter, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float result)) {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the parameter as an integer.
        /// </summary>
        /// <param name="defaultValue">Default if parse fails.</param>
        /// <returns>The integer value.</returns>
        public int GetInt(int defaultValue = 0) {
            if (int.TryParse(Parameter, out int result)) {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the parameter as a boolean.
        /// </summary>
        /// <param name="defaultValue">Default if parse fails.</param>
        /// <returns>The boolean value.</returns>
        public bool GetBool(bool defaultValue = false) {
            if (bool.TryParse(Parameter, out bool result)) {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a named parameter value.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default if not found.</param>
        /// <returns>The parameter value.</returns>
        public string GetParameter(string key, string defaultValue = "") {
            return Parameters.GetOrDefault(key, defaultValue);
        }
    }
}