namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for providing variable values to the conversation system.
    /// Implement this interface to expose game state to conversations.
    /// </summary>
    public interface IConversationVariableProvider {
        /// <summary>
        /// Gets the priority of this provider. Higher priority providers are queried first.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Tries to get a boolean variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if this provider has the variable, false otherwise.</returns>
        bool TryGetBool(string variableName, out bool value);

        /// <summary>
        /// Tries to get an integer variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if this provider has the variable, false otherwise.</returns>
        bool TryGetInt(string variableName, out int value);

        /// <summary>
        /// Tries to get a float variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if this provider has the variable, false otherwise.</returns>
        bool TryGetFloat(string variableName, out float value);

        /// <summary>
        /// Tries to get a string variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if this provider has the variable, false otherwise.</returns>
        bool TryGetString(string variableName, out string value);

        /// <summary>
        /// Checks if this provider has a variable with the given name.
        /// </summary>
        /// <param name="variableName">The name to check.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool HasVariable(string variableName);
    }
}