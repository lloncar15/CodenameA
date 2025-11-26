using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Context interface providing access to variables and predicates for condition evaluation.
    /// </summary>
    public interface IConditionContext {
        /// <summary>
        /// Tries to get a boolean variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetBool(string variableName, out bool value);

        /// <summary>
        /// Tries to get an integer variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetInt(string variableName, out int value);

        /// <summary>
        /// Tries to get a float variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetFloat(string variableName, out float value);

        /// <summary>
        /// Tries to get a string variable value.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetString(string variableName, out string value);

        /// <summary>
        /// Tries to get a predicate function by name.
        /// </summary>
        /// <param name="predicateName">The registered name of the predicate.</param>
        /// <param name="predicate">The predicate function if found.</param>
        /// <returns>True if the predicate exists, false otherwise.</returns>
        bool TryGetPredicate(string predicateName, out Func<SerializableDictionary<string, string>, bool> predicate);
    }
}