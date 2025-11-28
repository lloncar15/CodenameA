using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents a weighted random option.
    /// </summary>
    [Serializable]
    public class RandomOption {
        /// <summary>
        /// The node to go to if this option is selected.
        /// </summary>
        public string NextNodeId { get; set; }

        /// <summary>
        /// The weight of this option (higher = more likely).
        /// </summary>
        public float Weight { get; set; } = 1f;

        /// <summary>
        /// Optional condition that must be met for this option to be considered.
        /// </summary>
        public ConditionData Condition { get; set; }

        public RandomOption() { }

        public RandomOption(string nextNodeId, float weight = 1f) {
            NextNodeId = nextNodeId;
            Weight = weight;
        }
    }

    /// <summary>
    /// A conversation node that randomly selects from multiple paths.
    /// Useful for adding variety to NPC dialogue.
    /// </summary>
    [Serializable]
    public class RandomNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Random;

        /// <summary>
        /// The weighted options to randomly select from.
        /// </summary>
        public List<RandomOption> Options { get; set; }

        /// <summary>
        /// If true, avoids repeating the same option consecutively.
        /// Tracks last selection in conversation state.
        /// </summary>
        public bool AvoidRepeat { get; set; } = false;

        public RandomNode() : base() {
            Options = new List<RandomOption>();
        }

        public RandomNode(string id) : base(id) {
            Options = new List<RandomOption>();
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "RandomNode: Id cannot be null or empty.";
                return false;
            }

            if (Options == null || Options.Count == 0) {
                errorMessage = $"RandomNode '{Id}': Must have at least one option.";
                return false;
            }

            for (int i = 0; i < Options.Count; i++) {
                var option = Options[i];
                if (string.IsNullOrEmpty(option.NextNodeId)) {
                    errorMessage = $"RandomNode '{Id}': Option at index {i} has no NextNodeId.";
                    return false;
                }
                if (option.Weight < 0) {
                    errorMessage = $"RandomNode '{Id}': Option at index {i} has negative weight.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}