using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A conversation node that jumps to another node or conversation.
    /// Can be used for conversation chaining or subroutines.
    /// </summary>
    [Serializable]
    public class JumpNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Jump;

        /// <summary>
        /// The ID of the node to jump to within the current conversation.
        /// Used if TargetConversationId is null.
        /// </summary>
        public string TargetNodeId { get; set; }

        /// <summary>
        /// The ID of another conversation to jump to.
        /// If set, the current conversation ends and the target begins.
        /// </summary>
        public string TargetConversationId { get; set; }

        /// <summary>
        /// The starting node in the target conversation.
        /// If null, uses the target conversation's default start node.
        /// </summary>
        public string TargetConversationStartNodeId { get; set; }

        /// <summary>
        /// If true and jumping to another conversation, returns to this conversation
        /// after the target conversation ends.
        /// </summary>
        public bool ReturnAfterTarget { get; set; }

        /// <summary>
        /// The node to return to after the target conversation (if ReturnAfterTarget is true).
        /// </summary>
        public string ReturnNodeId { get; set; }

        public JumpNode() { }

        public JumpNode(string id, string targetNodeId) : base(id) {
            TargetNodeId = targetNodeId;
        }

        /// <summary>
        /// Creates a jump to another node in the same conversation.
        /// </summary>
        public static JumpNode ToNode(string id, string targetNodeId) {
            return new JumpNode(id, targetNodeId);
        }

        /// <summary>
        /// Creates a jump to another conversation.
        /// </summary>
        public static JumpNode ToConversation(string id, string conversationId, string startNodeId = null, bool returnAfter = false, string returnNodeId = null) {
            return new JumpNode {
                Id = id,
                TargetConversationId = conversationId,
                TargetConversationStartNodeId = startNodeId,
                ReturnAfterTarget = returnAfter,
                ReturnNodeId = returnNodeId
            };
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "JumpNode: Id cannot be null or empty.";
                return false;
            }

            bool hasLocalTarget = !string.IsNullOrEmpty(TargetNodeId);
            bool hasConversationTarget = !string.IsNullOrEmpty(TargetConversationId);

            if (!hasLocalTarget && !hasConversationTarget) {
                errorMessage = $"JumpNode '{Id}': Must specify either TargetNodeId or TargetConversationId.";
                return false;
            }

            if (hasLocalTarget && hasConversationTarget) {
                errorMessage = $"JumpNode '{Id}': Cannot specify both TargetNodeId and TargetConversationId.";
                return false;
            }

            if (ReturnAfterTarget && string.IsNullOrEmpty(ReturnNodeId)) {
                errorMessage = $"JumpNode '{Id}': ReturnNodeId must be set when ReturnAfterTarget is true.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}