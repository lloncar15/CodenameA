using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for handling text commands during typewriter display.
    /// </summary>
    public interface ITextCommandHandler {
        /// <summary>
        /// Handles a pause command.
        /// </summary>
        /// <param name="duration">Pause duration in seconds.</param>
        void HandlePause(float duration);

        /// <summary>
        /// Handles a speed change command.
        /// </summary>
        /// <param name="speedMultiplier">Speed multiplier (1.0 = normal).</param>
        void HandleSpeed(float speedMultiplier);

        /// <summary>
        /// Handles an expression change command.
        /// </summary>
        /// <param name="expressionKey">The new expression key.</param>
        void HandleExpression(string expressionKey);

        /// <summary>
        /// Handles an event trigger command.
        /// </summary>
        /// <param name="eventName">The event name to trigger.</param>
        /// <param name="parameters">Optional event parameters.</param>
        void HandleEvent(string eventName, SerializableDictionary<string, string> parameters = null);

        /// <summary>
        /// Handles a custom command.
        /// </summary>
        /// <param name="commandType">The command type.</param>
        /// <param name="command">The full command result.</param>
        void HandleCustomCommand(string commandType, TextCommandResult command);
    }
}