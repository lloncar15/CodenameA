using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Registry for node processors.
    /// </summary>
    public class NodeProcessorRegistry {
        private readonly Dictionary<ConversationNodeType, INodeProcessor> _processors;

        public NodeProcessorRegistry() {
            _processors = new Dictionary<ConversationNodeType, INodeProcessor>();
        }

        /// <summary>
        /// Registers a node processor.
        /// </summary>
        /// <param name="processor">The processor to register.</param>
        public void Register(INodeProcessor processor) {
            if (processor == null) {
                return;
            }

            _processors[processor.NodeType] = processor;
        }

        /// <summary>
        /// Gets the processor for a node type.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <returns>The processor, or null if not found.</returns>
        public INodeProcessor GetProcessor(ConversationNodeType nodeType) {
            return _processors.TryGetValue(nodeType, out var processor) ? processor : null;
        }

        /// <summary>
        /// Gets the processor for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The processor, or null if not found.</returns>
        public INodeProcessor GetProcessor(ConversationNode node) {
            return node != null ? GetProcessor(node.NodeType) : null;
        }

        /// <summary>
        /// Checks if a processor is registered for a node type.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <returns>True if registered.</returns>
        public bool HasProcessor(ConversationNodeType nodeType) {
            return _processors.ContainsKey(nodeType);
        }

        /// <summary>
        /// Creates a registry with all default processors.
        /// </summary>
        /// <param name="conversationLoader">Optional conversation loader for jump nodes.</param>
        /// <returns>A new registry with default processors.</returns>
        public static NodeProcessorRegistry CreateDefault(Func<string, System.Threading.Tasks.Task<ConversationData>> conversationLoader = null) {
            NodeProcessorRegistry registry = new();
            
            registry.Register(new TextNodeProcessor());
            registry.Register(new ChoiceNodeProcessor());
            registry.Register(new BranchNodeProcessor());
            registry.Register(new EventNodeProcessor());
            registry.Register(new RandomNodeProcessor());
            registry.Register(new WaitNodeProcessor());
            registry.Register(new JumpNodeProcessor(conversationLoader));

            return registry;
        }
    }
}