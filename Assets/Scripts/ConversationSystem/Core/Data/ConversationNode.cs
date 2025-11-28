using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Abstract base class for all conversation nodes.
    /// Represents the smallest unit of a conversation.
    /// </summary>
    [Serializable]
    public abstract class ConversationNode {
        /// <summary>
        /// Unique identifier for this node within the conversation.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The type of this node, used for deserialization and processing.
        /// </summary>
        public abstract ConversationNodeType NodeType { get; }
        
        /// <summary>
        /// Optional metadata for extending node functionality.
        /// Can store custom key-value pairs for game-specific features.
        /// </summary>
        public SerializableDictionary<string, string> Metadata { get; set; }

        protected ConversationNode() {
            Metadata = new SerializableDictionary<string, string>();
        }

        protected ConversationNode(string id) : this() {
            Id = id;
        }

        /// <summary>
        /// Validates the node configuration.
        /// </summary>
        /// <returns>True if the node is valid, false otherwise.</returns>
        public abstract bool Validate(out string errorMessage);
    }
}