using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A conversation node that waits before continuing.
    /// Can wait for time, condition, or event.
    /// </summary>
    [Serializable]
    public class WaitNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Wait;

        /// <summary>
        /// The type of wait to perform.
        /// </summary>
        public WaitType WaitType { get; set; }

        /// <summary>
        /// Duration to wait in seconds (for WaitType.Time).
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Condition to wait for (for WaitType.Condition).
        /// </summary>
        public ConditionData WaitCondition { get; set; }

        /// <summary>
        /// Event name to wait for (for WaitType.Event).
        /// </summary>
        public string WaitEventName { get; set; }

        /// <summary>
        /// Maximum time to wait before timing out (optional, 0 = no timeout).
        /// </summary>
        public float Timeout { get; set; } = 0f;

        /// <summary>
        /// Node to go to if timeout occurs.
        /// </summary>
        public string TimeoutNodeId { get; set; }

        /// <summary>
        /// The ID of the next node to proceed to after waiting.
        /// </summary>
        public string NextNodeId { get; set; }

        public WaitNode() : base() { }

        public WaitNode(string id, WaitType waitType, string nextNodeId) : base(id) {
            WaitType = waitType;
            NextNodeId = nextNodeId;
        }

        /// <summary>
        /// Creates a time-based wait node.
        /// </summary>
        public static WaitNode ForTime(string id, float duration, string nextNodeId) {
            return new WaitNode(id, WaitType.Time, nextNodeId) {
                Duration = duration
            };
        }

        /// <summary>
        /// Creates a condition-based wait node.
        /// </summary>
        public static WaitNode ForCondition(string id, ConditionData condition, string nextNodeId, float timeout = 0f, string timeoutNodeId = null) {
            return new WaitNode(id, WaitType.Condition, nextNodeId) {
                WaitCondition = condition,
                Timeout = timeout,
                TimeoutNodeId = timeoutNodeId
            };
        }

        /// <summary>
        /// Creates an event-based wait node.
        /// </summary>
        public static WaitNode ForEvent(string id, string eventName, string nextNodeId, float timeout = 0f, string timeoutNodeId = null) {
            return new WaitNode(id, WaitType.Event, nextNodeId) {
                WaitEventName = eventName,
                Timeout = timeout,
                TimeoutNodeId = timeoutNodeId
            };
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "WaitNode: Id cannot be null or empty.";
                return false;
            }

            switch (WaitType) {
                case WaitType.Time:
                    if (Duration <= 0) {
                        errorMessage = $"WaitNode '{Id}': Duration must be greater than 0 for time-based wait.";
                        return false;
                    }
                    break;
                case WaitType.Condition:
                    if (WaitCondition == null) {
                        errorMessage = $"WaitNode '{Id}': WaitCondition must be set for condition-based wait.";
                        return false;
                    }
                    break;
                case WaitType.Event:
                    if (string.IsNullOrEmpty(WaitEventName)) {
                        errorMessage = $"WaitNode '{Id}': WaitEventName must be set for event-based wait.";
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Timeout > 0 && string.IsNullOrEmpty(TimeoutNodeId)) {
                errorMessage = $"WaitNode '{Id}': TimeoutNodeId must be set when Timeout is specified.";
                return false;
            }

            if (string.IsNullOrEmpty(NextNodeId) && (Timeout <= 0 || string.IsNullOrEmpty(TimeoutNodeId))) {
                errorMessage = $"WaitNode '{Id}': Must have NextNodeId or a valid timeout configuration.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}