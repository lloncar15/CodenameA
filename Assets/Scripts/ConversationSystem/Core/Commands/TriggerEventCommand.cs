using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Command that triggers a named event for external systems to handle.
    /// </summary>
    public class TriggerEventCommand : ConversationCommandBase {
        public override string CommandType => "TriggerEvent";

        private readonly string _eventName;
        private readonly SerializableDictionary<string, string> _parameters;

        /// <summary>
        /// Creates a new TriggerEventCommand.
        /// </summary>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="parameters">Optional parameters for the event.</param>
        public TriggerEventCommand(string eventName, SerializableDictionary<string, string> parameters = null) {
            _eventName = eventName;
            _parameters = parameters ?? new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// Creates a TriggerEventCommand from CommandData.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>A new TriggerEventCommand instance.</returns>
        public static TriggerEventCommand FromData(CommandData data) {
            string eventName = data.GetString("event");
            
            // Copy parameters excluding the "event" key
            SerializableDictionary<string, string> parameters = new ();
            foreach (KeyValuePair<string, string> kvp in data.Parameters) {
                if (kvp.Key != "event") {
                    parameters[kvp.Key] = kvp.Value;
                }
            }

            return new TriggerEventCommand(eventName, parameters) {
                SourceData = data
            };
        }

        /// <inheritdoc />
        public override void Execute(ICommandContext context) {
            if (string.IsNullOrEmpty(_eventName)) {
                UnityEngine.Debug.LogWarning("TriggerEventCommand: Event name is null or empty.");
                return;
            }

            context.TriggerEvent(_eventName, _parameters);
        }

        /// <inheritdoc />
        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(_eventName)) {
                errorMessage = "TriggerEventCommand: Event name cannot be null or empty.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public override string ToString() {
            return $"TriggerEvent({_eventName})";
        }
    }
}