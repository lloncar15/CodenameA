using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes EventNode instances.
    /// </summary>
    public class EventNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Event;

        public async Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not EventNode eventNode) {
                Debug.LogError("EventNodeProcessor: Invalid node type.");
                return null;
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, eventNode.Id);

            // Execute commands
            if (eventNode.Commands != null && eventNode.Commands.Count > 0) {
                CommandExecutor executor = new(context.CommandFactory);
                await executor.ExecuteAllAsync(eventNode.Commands, context.CommandContext);
            }

            return eventNode.NextNodeId;
        }
    }
}