namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for managing conversation-specific state.
    /// Tracks variables set during conversations for persistence.
    /// </summary>
    public interface IConversationStateManager {
        /// <summary>
        /// Sets a boolean variable.
        /// </summary>
        void SetBool(string key, bool value);

        /// <summary>
        /// Gets a boolean variable.
        /// </summary>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>
        /// Sets an integer variable.
        /// </summary>
        void SetInt(string key, int value);

        /// <summary>
        /// Gets an integer variable.
        /// </summary>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// Sets a float variable.
        /// </summary>
        void SetFloat(string key, float value);

        /// <summary>
        /// Gets a float variable.
        /// </summary>
        float GetFloat(string key, float defaultValue = 0f);

        /// <summary>
        /// Sets a string variable.
        /// </summary>
        void SetString(string key, string value);

        /// <summary>
        /// Gets a string variable.
        /// </summary>
        string GetString(string key, string defaultValue = "");

        /// <summary>
        /// Checks if a variable exists.
        /// </summary>
        bool HasVariable(string key);

        /// <summary>
        /// Removes a variable.
        /// </summary>
        bool RemoveVariable(string key);

        /// <summary>
        /// Records that a node was visited.
        /// </summary>
        void RecordNodeVisit(string conversationId, string nodeId);

        /// <summary>
        /// Gets the number of times a node was visited.
        /// </summary>
        int GetNodeVisitCount(string conversationId, string nodeId);

        /// <summary>
        /// Checks if a node was ever visited.
        /// </summary>
        bool WasNodeVisited(string conversationId, string nodeId);

        /// <summary>
        /// Records a choice that was made.
        /// </summary>
        void RecordChoice(string conversationId, string nodeId, string choiceId);

        /// <summary>
        /// Gets the last choice made at a specific node.
        /// </summary>
        string GetLastChoice(string conversationId, string nodeId);

        /// <summary>
        /// Gets all choices made at a specific node.
        /// </summary>
        System.Collections.Generic.IReadOnlyList<string> GetAllChoices(string conversationId, string nodeId);
    }
}