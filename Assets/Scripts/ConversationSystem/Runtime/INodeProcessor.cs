using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for processing conversation nodes.
    /// </summary>
    public interface INodeProcessor {
        /// <summary>
        /// Gets the node type this processor handles.
        /// </summary>
        ConversationNodeType NodeType { get; }

        /// <summary>
        /// Processes a node and returns the next node ID.
        /// </summary>
        /// <param name="node">The node to process.</param>
        /// <param name="context">The conversation context.</param>
        /// <returns>The next node ID, or null if conversation should end.</returns>
        Task<string> ProcessAsync(ConversationNode node, ConversationContext context);
    }
}