using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Command that logs a message to the console.
    /// Useful for debugging conversations.
    /// </summary>
    public class LogCommand : ConversationCommandBase {
        public override string CommandType => "Log";

        private readonly string _message;
        private readonly LogType _logType;

        /// <summary>
        /// Creates a new LogCommand.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logType">The type of log (Info, Warning, Error).</param>
        public LogCommand(string message, LogType logType = LogType.Log) {
            _message = message;
            _logType = logType;
        }

        /// <summary>
        /// Creates a LogCommand from CommandData.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>A new LogCommand instance.</returns>
        public static LogCommand FromData(CommandData data) {
            string message = data.GetString("message");
            string typeStr = data.GetString("type", "Log");

            LogType logType = LogType.Log;
            if (System.Enum.TryParse(typeStr, true, out LogType parsed)) {
                logType = parsed;
            }

            return new LogCommand(message, logType) {
                SourceData = data
            };
        }

        /// <inheritdoc />
        public override void Execute(ICommandContext context) {
            string formattedMessage = $"[Conversation:{context.CurrentConversation?.Id}] {_message}";

            switch (_logType) {
                case LogType.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogType.Error:
                    Debug.LogError(formattedMessage);
                    break;
                default:
                    Debug.Log(formattedMessage);
                    break;
            }
        }

        public override string ToString() {
            return $"Log({_logType}: {_message})";
        }
    }
}