// Assets/Scripts/ConversationSystem/Core/Data/Nodes/BranchNode.cs
using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents a single branch option with its condition.
    /// </summary>
    [Serializable]
    public class BranchOption {
        /// <summary>
        /// The condition that must be met to take this branch.
        /// </summary>
        public ConditionData Condition { get; set; }

        /// <summary>
        /// The node to go to if the condition is met.
        /// </summary>
        public string NextNodeId { get; set; }

        /// <summary>
        /// Priority for evaluation order (higher = checked first).
        /// </summary>
        public int Priority { get; set; } = 0;

        public BranchOption() { }

        public BranchOption(ConditionData condition, string nextNodeId, int priority = 0) {
            Condition = condition;
            NextNodeId = nextNodeId;
            Priority = priority;
        }
    }

    /// <summary>
    /// A conversation node that automatically branches based on conditions.
    /// Not displayed to the player - used for flow control.
    /// </summary>
    [Serializable]
    public class BranchNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Branch;

        /// <summary>
        /// The branches to evaluate in priority order.
        /// </summary>
        public List<BranchOption> Branches { get; set; }

        /// <summary>
        /// The default node to go to if no conditions are met.
        /// </summary>
        public string DefaultNodeId { get; set; }

        public BranchNode() : base() {
            Branches = new List<BranchOption>();
        }

        public BranchNode(string id) : base(id) {
            Branches = new List<BranchOption>();
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "BranchNode: Id cannot be null or empty.";
                return false;
            }

            if ((Branches == null || Branches.Count == 0) && string.IsNullOrEmpty(DefaultNodeId)) {
                errorMessage = $"BranchNode '{Id}': Must have at least one branch or a default node.";
                return false;
            }

            if (Branches != null) {
                for (int i = 0; i < Branches.Count; i++) {
                    BranchOption branch = Branches[i];
                    if (branch.Condition == null) {
                        errorMessage = $"BranchNode '{Id}': Branch at index {i} has no condition.";
                        return false;
                    }
                    if (string.IsNullOrEmpty(branch.NextNodeId)) {
                        errorMessage = $"BranchNode '{Id}': Branch at index {i} has no NextNodeId.";
                        return false;
                    }
                }
            }

            errorMessage = null;
            return true;
        }
    }
}