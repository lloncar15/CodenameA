using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Implementation of ICommandContext for command execution.
    /// </summary>
    public class CommandContext : ICommandContext {
        private readonly Dictionary<string, List<Action<SerializableDictionary<string, string>>>> _eventListeners;

        /// <inheritdoc />
        public ConversationData CurrentConversation { get; set; }

        /// <inheritdoc />
        public ConversationNode CurrentNode { get; set; }

        /// <inheritdoc />
        public IConversationStateManager StateManager { get; }

        /// <summary>
        /// Event raised when TriggerEvent is called.
        /// External systems can subscribe to this to handle conversation events.
        /// </summary>
        public event Action<string, SerializableDictionary<string, string>> OnEventTriggered;

        /// <summary>
        /// Creates a new command context.
        /// </summary>
        /// <param name="stateManager">The state manager for variable storage.</param>
        public CommandContext(IConversationStateManager stateManager) {
            StateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
            _eventListeners = new Dictionary<string, List<Action<SerializableDictionary<string, string>>>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public void SetBool(string variableName, bool value) {
            StateManager.SetBool(variableName, value);
        }

        /// <inheritdoc />
        public void SetInt(string variableName, int value) {
            StateManager.SetInt(variableName, value);
        }

        /// <inheritdoc />
        public void SetFloat(string variableName, float value) {
            StateManager.SetFloat(variableName, value);
        }

        /// <inheritdoc />
        public void SetString(string variableName, string value) {
            StateManager.SetString(variableName, value);
        }

        /// <inheritdoc />
        public void TriggerEvent(string eventName, SerializableDictionary<string, string> parameters = null) {
            parameters ??= new SerializableDictionary<string, string>();

            // Notify global listeners
            OnEventTriggered?.Invoke(eventName, parameters);

            // Notify specific event listeners
            if (_eventListeners.TryGetValue(eventName, out var listeners)) {
                foreach (var listener in listeners) {
                    try {
                        listener(parameters);
                    }
                    catch (Exception ex) {
                        UnityEngine.Debug.LogError($"CommandContext: Error in event listener for '{eventName}': {ex.Message}");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void RegisterEventListener(string eventName, Action<SerializableDictionary<string, string>> callback) {
            if (string.IsNullOrEmpty(eventName) || callback == null) {
                return;
            }

            if (!_eventListeners.TryGetValue(eventName, out var listeners)) {
                listeners = new List<Action<SerializableDictionary<string, string>>>();
                _eventListeners[eventName] = listeners;
            }

            if (!listeners.Contains(callback)) {
                listeners.Add(callback);
            }
        }

        /// <inheritdoc />
        public void UnregisterEventListener(string eventName, Action<SerializableDictionary<string, string>> callback) {
            if (string.IsNullOrEmpty(eventName) || callback == null) {
                return;
            }

            if (_eventListeners.TryGetValue(eventName, out var listeners)) {
                listeners.Remove(callback);
            }
        }

        /// <summary>
        /// Clears all event listeners.
        /// </summary>
        public void ClearEventListeners() {
            _eventListeners.Clear();
        }

        /// <summary>
        /// Clears listeners for a specific event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        public void ClearEventListeners(string eventName) {
            _eventListeners.Remove(eventName);
        }
    }
}