using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Executes text commands, routing them to appropriate handlers.
    /// </summary>
    public class TextCommandExecutor : ITextCommandHandler {
        private readonly Dictionary<string, Action<TextCommandResult>> _customHandlers;
        
        /// <summary>
        /// Event raised when a pause command is executed.
        /// </summary>
        public event Action<float> OnPause;

        /// <summary>
        /// Event raised when a speed command is executed.
        /// </summary>
        public event Action<float> OnSpeedChange;

        /// <summary>
        /// Event raised when an expression command is executed.
        /// </summary>
        public event Action<string> OnExpressionChange;

        /// <summary>
        /// Event raised when an event command is executed.
        /// </summary>
        public event Action<string, SerializableDictionary<string, string>> OnEventTriggered;

        /// <summary>
        /// Event raised for any command (for logging/debugging).
        /// </summary>
        public event Action<TextCommandResult> OnAnyCommand;

        /// <summary>
        /// Creates a new text command executor.
        /// </summary>
        public TextCommandExecutor() {
            _customHandlers = new Dictionary<string, Action<TextCommandResult>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registers a custom command handler.
        /// </summary>
        /// <param name="commandType">The command type to handle.</param>
        /// <param name="handler">The handler action.</param>
        public void RegisterHandler(string commandType, Action<TextCommandResult> handler) {
            if (string.IsNullOrEmpty(commandType) || handler == null) {
                return;
            }

            _customHandlers[commandType] = handler;
        }

        /// <summary>
        /// Unregisters a custom command handler.
        /// </summary>
        /// <param name="commandType">The command type to unregister.</param>
        public void UnregisterHandler(string commandType) {
            _customHandlers.Remove(commandType);
        }

        /// <summary>
        /// Executes a text command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void Execute(TextCommandResult command) {
            if (command == null) {
                return;
            }

            OnAnyCommand?.Invoke(command);

            switch (command.CommandType.ToLower()) {
                case "pause":
                case "wait":
                case "p":
                    HandlePause(command.GetFloat(0.5f));
                    break;

                case "speed":
                case "s":
                    HandleSpeed(command.GetFloat(1.0f));
                    break;

                case "expression":
                case "expr":
                case "e":
                    HandleExpression(command.Parameter);
                    break;

                case "event":
                case "evt":
                    HandleEvent(command.Parameter, command.Parameters);
                    break;

                default:
                    HandleCustomCommand(command.CommandType, command);
                    break;
            }
        }

        /// <inheritdoc />
        public void HandlePause(float duration) {
            OnPause?.Invoke(duration);
        }

        /// <inheritdoc />
        public void HandleSpeed(float speedMultiplier) {
            OnSpeedChange?.Invoke(Mathf.Max(0.1f, speedMultiplier));
        }

        /// <inheritdoc />
        public void HandleExpression(string expressionKey) {
            if (!string.IsNullOrEmpty(expressionKey)) {
                OnExpressionChange?.Invoke(expressionKey);
            }
        }

        /// <inheritdoc />
        public void HandleEvent(string eventName, SerializableDictionary<string, string> parameters = null) {
            if (!string.IsNullOrEmpty(eventName)) {
                OnEventTriggered?.Invoke(eventName, parameters ?? new SerializableDictionary<string, string>());
            }
        }

        /// <inheritdoc />
        public void HandleCustomCommand(string commandType, TextCommandResult command) {
            if (_customHandlers.TryGetValue(commandType, out Action<TextCommandResult> handler)) {
                try {
                    handler(command);
                }
                catch (Exception ex) {
                    Debug.LogError($"TextCommandExecutor: Error executing custom handler for '{commandType}': {ex.Message}");
                }
            }
            else {
                Debug.LogWarning($"TextCommandExecutor: Unknown command type '{commandType}'.");
            }
        }

        /// <summary>
        /// Clears all custom handlers.
        /// </summary>
        public void ClearHandlers() {
            _customHandlers.Clear();
        }
    }
}