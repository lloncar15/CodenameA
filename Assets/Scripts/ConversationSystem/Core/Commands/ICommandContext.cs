using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Context interface providing access to conversation state for command execution.
    /// </summary>
    public interface ICommandContext {
        /// <summary>
        /// Gets the current conversation data.
        /// </summary>
        ConversationData CurrentConversation { get; }

        /// <summary>
        /// Gets the current node being processed.
        /// </summary>
        ConversationNode CurrentNode { get; }

        /// <summary>
        /// Gets the conversation state manager for tracking choices and visited nodes.
        /// </summary>
        IConversationStateManager StateManager { get; }

        /// <summary>
        /// Sets a boolean variable in the conversation state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value to set.</param>
        void SetBool(string variableName, bool value);

        /// <summary>
        /// Sets an integer variable in the conversation state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value to set.</param>
        void SetInt(string variableName, int value);

        /// <summary>
        /// Sets a float variable in the conversation state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value to set.</param>
        void SetFloat(string variableName, float value);

        /// <summary>
        /// Sets a string variable in the conversation state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value to set.</param>
        void SetString(string variableName, string value);

        /// <summary>
        /// Triggers a named event that external systems can listen to.
        /// </summary>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="parameters">Optional parameters for the event.</param>
        void TriggerEvent(string eventName, SerializableDictionary<string, string> parameters = null);

        /// <summary>
        /// Registers a callback to be invoked when a specific event is triggered.
        /// </summary>
        /// <param name="eventName">The event name to listen for.</param>
        /// <param name="callback">The callback to invoke.</param>
        void RegisterEventListener(string eventName, Action<SerializableDictionary<string, string>> callback);

        /// <summary>
        /// Unregisters an event listener.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="callback">The callback to remove.</param>
        void UnregisterEventListener(string eventName, Action<SerializableDictionary<string, string>> callback);
    }
}