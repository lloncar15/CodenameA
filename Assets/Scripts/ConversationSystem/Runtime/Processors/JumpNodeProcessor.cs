using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes JumpNode instances.
    /// </summary>
    public class JumpNodeProcessor : INodeProcessor {
        private readonly System.Func<string, Task<ConversationData>> _conversationLoader;

        public ConversationNodeType NodeType => ConversationNodeType.Jump;

        /// <summary>
        /// Creates a new jump node processor.
        /// </summary>
        /// <param name="conversationLoader">Function to load conversations by ID.</param>
        public JumpNodeProcessor(System.Func<string, Task<ConversationData>> conversationLoader = null) {
            _conversationLoader = conversationLoader;
        }

        public async Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not JumpNode jumpNode) {
                Debug.LogError("JumpNodeProcessor: Invalid node type.");
                return null;
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, jumpNode.Id);

            // Check if jumping to another conversation
            if (!string.IsNullOrEmpty(jumpNode.TargetConversationId)) {
                return await HandleConversationJump(jumpNode, context);
            }

            // Simple node jump within same conversation
            return jumpNode.TargetNodeId;
        }

        /// <summary>
        /// Handles jumping to another conversation.
        /// </summary>
        private async Task<string> HandleConversationJump(JumpNode jumpNode, ConversationContext context) {
            if (_conversationLoader == null) {
                Debug.LogError("JumpNodeProcessor: No conversation loader configured for cross-conversation jumps.");
                return jumpNode.TargetNodeId; // Fall back to local jump
            }

            // Push current state if returning
            if (jumpNode.ReturnAfterTarget) {
                context.PushState(jumpNode.ReturnNodeId);
            }

            // Load target conversation
            ConversationData targetConversation = await _conversationLoader(jumpNode.TargetConversationId);
            if (targetConversation == null) {
                Debug.LogError($"JumpNodeProcessor: Failed to load conversation '{jumpNode.TargetConversationId}'.");
                
                // Pop state if we pushed it
                if (jumpNode.ReturnAfterTarget) {
                    context.PopState();
                }
                
                return null;
            }

            // Switch conversation
            context.Conversation = targetConversation;

            // Return target node ID (or start node if not specified)
            return !string.IsNullOrEmpty(jumpNode.TargetConversationStartNodeId) 
                ? jumpNode.TargetConversationStartNodeId 
                : targetConversation.StartNodeId;
        }
    }
}