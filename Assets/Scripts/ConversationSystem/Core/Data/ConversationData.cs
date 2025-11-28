using System;
using System.Collections.Generic;
using System.Linq;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Root data structure representing an entire conversation.
    /// Contains all nodes and metadata for a single dialogue sequence.
    /// </summary>
    [Serializable]
    public class ConversationData {
        /// <summary>
        /// Unique identifier for this conversation.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Human-readable name for this conversation (for editor/debugging).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description of this conversation's purpose.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID of the first node to start the conversation.
        /// </summary>
        public string StartNodeId { get; set; }

        /// <summary>
        /// All nodes in this conversation, keyed by their ID.
        /// </summary>
        public Dictionary<string, ConversationNode> Nodes { get; set; }

        /// <summary>
        /// IDs of characters that participate in this conversation.
        /// Used for preloading character data.
        /// </summary>
        public List<string> ParticipantIds { get; set; }

        /// <summary>
        /// Custom predicates required by this conversation.
        /// Maps predicate names to their descriptions for documentation.
        /// </summary>
        public SerializableDictionary<string, string> RequiredPredicates { get; set; }

        /// <summary>
        /// Optional metadata for extending conversation functionality.
        /// </summary>
        public SerializableDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Version number for tracking conversation updates.
        /// </summary>
        public int Version { get; set; } = 1;

        public ConversationData() {
            Nodes = new Dictionary<string, ConversationNode>();
            ParticipantIds = new List<string>();
            RequiredPredicates = new SerializableDictionary<string, string>();
            Metadata = new SerializableDictionary<string, string>();
        }

        public ConversationData(string id, string name) : this() {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets a node by its ID.
        /// </summary>
        /// <param name="nodeId">The ID of the node to retrieve.</param>
        /// <returns>The node if found, null otherwise.</returns>
        public ConversationNode GetNode(string nodeId) {
            return string.IsNullOrEmpty(nodeId) ? null : Nodes.GetValueOrDefault(nodeId);
        }

        /// <summary>
        /// Gets a node by its ID, cast to a specific type.
        /// </summary>
        /// <typeparam name="T">The expected node type.</typeparam>
        /// <param name="nodeId">The ID of the node to retrieve.</param>
        /// <returns>The node if found and of correct type, null otherwise.</returns>
        public T GetNode<T>(string nodeId) where T : ConversationNode {
            return GetNode(nodeId) as T;
        }

        /// <summary>
        /// Gets the start node for this conversation.
        /// </summary>
        /// <returns>The start node if found, null otherwise.</returns>
        public ConversationNode GetStartNode() {
            return GetNode(StartNodeId);
        }

        /// <summary>
        /// Adds a node to the conversation.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <returns>True if added successfully, false if ID already exists.</returns>
        public bool AddNode(ConversationNode node) {
            if (node == null || string.IsNullOrEmpty(node.Id)) return false;
            return Nodes.TryAdd(node.Id, node);
        }

        /// <summary>
        /// Removes a node from the conversation.
        /// </summary>
        /// <param name="nodeId">The ID of the node to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool RemoveNode(string nodeId) {
            return Nodes.Remove(nodeId);
        }

        /// <summary>
        /// Gets all nodes of a specific type.
        /// </summary>
        /// <typeparam name="T">The node type to filter by.</typeparam>
        /// <returns>Enumerable of nodes of the specified type.</returns>
        public IEnumerable<T> GetNodesOfType<T>() where T : ConversationNode {
            return Nodes.Values.OfType<T>();
        }

        /// <summary>
        /// Validates the entire conversation structure.
        /// </summary>
        /// <param name="errors">List of validation errors found.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool Validate(out List<string> errors) {
            errors = new List<string>();

            if (string.IsNullOrEmpty(Id)) {
                errors.Add("Conversation: Id cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(StartNodeId)) {
                errors.Add($"Conversation '{Id}': StartNodeId cannot be null or empty.");
            }
            else if (!Nodes.ContainsKey(StartNodeId)) {
                errors.Add($"Conversation '{Id}': StartNodeId '{StartNodeId}' does not exist in nodes.");
            }

            foreach (KeyValuePair<string, ConversationNode> kvp in Nodes) {
                if (kvp.Key != kvp.Value.Id) {
                    errors.Add($"Conversation '{Id}': Node key '{kvp.Key}' does not match node Id '{kvp.Value.Id}'.");
                }

                if (!kvp.Value.Validate(out string nodeError)) {
                    errors.Add(nodeError);
                }

                // Validate node references
                ValidateNodeReferences(kvp.Value, errors);
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Validates that all node references point to existing nodes.
        /// </summary>
        private void ValidateNodeReferences(ConversationNode node, List<string> errors) {
            switch (node) {
                case TextNode textNode:
                    ValidateNodeReference(textNode.NextNodeId, node.Id, "NextNodeId", errors);
                    break;
                    
                case ChoiceNode choiceNode:
                    foreach (var choice in choiceNode.Choices) {
                        ValidateNodeReference(choice.NextNodeId, node.Id, $"Choice '{choice.Id}' NextNodeId", errors);
                    }
                    ValidateNodeReference(choiceNode.TimeoutNodeId, node.Id, "TimeoutNodeId", errors);
                    break;
                    
                case BranchNode branchNode:
                    foreach (var branch in branchNode.Branches) {
                        ValidateNodeReference(branch.NextNodeId, node.Id, "Branch NextNodeId", errors);
                    }
                    ValidateNodeReference(branchNode.DefaultNodeId, node.Id, "DefaultNodeId", errors);
                    break;
                    
                case EventNode eventNode:
                    ValidateNodeReference(eventNode.NextNodeId, node.Id, "NextNodeId", errors);
                    break;
                    
                case RandomNode randomNode:
                    foreach (var option in randomNode.Options) {
                        ValidateNodeReference(option.NextNodeId, node.Id, "RandomOption NextNodeId", errors);
                    }
                    break;
                    
                case WaitNode waitNode:
                    ValidateNodeReference(waitNode.NextNodeId, node.Id, "NextNodeId", errors);
                    ValidateNodeReference(waitNode.TimeoutNodeId, node.Id, "TimeoutNodeId", errors);
                    break;
                    
                case JumpNode jumpNode:
                    // Only validate local jumps; cross-conversation jumps can't be validated here
                    if (string.IsNullOrEmpty(jumpNode.TargetConversationId)) {
                        ValidateNodeReference(jumpNode.TargetNodeId, node.Id, "TargetNodeId", errors);
                    }
                    ValidateNodeReference(jumpNode.ReturnNodeId, node.Id, "ReturnNodeId", errors);
                    break;
            }
        }

        /// <summary>
        /// Validates a single node reference.
        /// </summary>
        private void ValidateNodeReference(string targetNodeId, string sourceNodeId, string fieldName, List<string> errors) {
            if (!string.IsNullOrEmpty(targetNodeId) && !Nodes.ContainsKey(targetNodeId)) {
                errors.Add($"Conversation '{Id}': Node '{sourceNodeId}' references non-existent node '{targetNodeId}' in {fieldName}.");
            }
        }
    }
}