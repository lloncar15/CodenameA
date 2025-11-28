using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A conversation node that presents choices to the player.
    /// Branches to different nodes based on player selection.
    /// </summary>
    [Serializable]
    public class ChoiceNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Choice;

        /// <summary>
        /// Optional prompt text displayed above the choices.
        /// </summary>
        public string PromptText { get; set; }

        /// <summary>
        /// The ID of the character speaking the prompt (if any).
        /// </summary>
        public string SpeakerId { get; set; }

        /// <summary>
        /// The expression for the speaker (if any).
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The list of choices available to the player.
        /// </summary>
        public List<ChoiceData> Choices { get; set; }

        /// <summary>
        /// If true, shuffle the order of choices when displayed.
        /// </summary>
        public bool ShuffleChoices { get; set; } = false;

        /// <summary>
        /// Optional time limit for making a choice (in seconds).
        /// 0 or negative means no time limit.
        /// </summary>
        public float TimeLimit { get; set; } = 0f;

        /// <summary>
        /// The node to go to if time runs out.
        /// Only used if TimeLimit > 0.
        /// </summary>
        public string TimeoutNodeId { get; set; }

        public ChoiceNode() : base() {
            Choices = new List<ChoiceData>();
        }

        public ChoiceNode(string id) : base(id) {
            Choices = new List<ChoiceData>();
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "ChoiceNode: Id cannot be null or empty.";
                return false;
            }

            if (Choices == null || Choices.Count == 0) {
                errorMessage = $"ChoiceNode '{Id}': Must have at least one choice.";
                return false;
            }

            for (int i = 0; i < Choices.Count; i++) {
                var choice = Choices[i];
                if (string.IsNullOrEmpty(choice.Id)) {
                    errorMessage = $"ChoiceNode '{Id}': Choice at index {i} has no Id.";
                    return false;
                }
                if (string.IsNullOrEmpty(choice.Text)) {
                    errorMessage = $"ChoiceNode '{Id}': Choice '{choice.Id}' has no text.";
                    return false;
                }
                if (string.IsNullOrEmpty(choice.NextNodeId)) {
                    errorMessage = $"ChoiceNode '{Id}': Choice '{choice.Id}' has no NextNodeId.";
                    return false;
                }
            }

            if (TimeLimit > 0 && string.IsNullOrEmpty(TimeoutNodeId)) {
                errorMessage = $"ChoiceNode '{Id}': TimeLimit is set but TimeoutNodeId is not specified.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}