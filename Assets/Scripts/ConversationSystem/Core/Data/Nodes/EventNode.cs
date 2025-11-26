using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A conversation node that executes commands without displaying anything.
    /// Used to trigger game events, set variables, etc.
    /// </summary>
    [Serializable]
    public class EventNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Event;

        /// <summary>
        /// The commands to execute when this node is reached.
        /// </summary>
        public List<CommandData> Commands { get; set; }

        /// <summary>
        /// The ID of the next node to proceed to.
        /// Null or empty indicates end of conversation.
        /// </summary>
        public string NextNodeId { get; set; }

        public EventNode() : base() {
            Commands = new List<CommandData>();
        }

        public EventNode(string id, string nextNodeId = null) : base(id) {
            NextNodeId = nextNodeId;
            Commands = new List<CommandData>();
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "EventNode: Id cannot be null or empty.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}