using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents a single choice option presented to the player.
    /// </summary>
    [Serializable]
    public class ChoiceData {
        /// <summary>
        /// Unique identifier for this choice within the node.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The text displayed for this choice.
        /// May contain variable substitutions like [var:player_name].
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The ID of the node to go to when this choice is selected.
        /// </summary>
        public string NextNodeId { get; set; }

        /// <summary>
        /// Condition that must be met for this choice to be visible.
        /// If null, choice is always visible.
        /// </summary>
        public ConditionData VisibilityCondition { get; set; }

        /// <summary>
        /// Condition that must be met for this choice to be selectable.
        /// If null but visible, choice is always selectable.
        /// If condition fails, choice appears greyed out.
        /// </summary>
        public ConditionData SelectableCondition { get; set; }

        /// <summary>
        /// Optional preview text shown when hovering over greyed-out choices.
        /// Example: "[Requires 50 gold]"
        /// </summary>
        public string UnavailableReason { get; set; }

        /// <summary>
        /// Optional preview of consequences for selecting this choice.
        /// Example: "[This will anger the merchant]"
        /// </summary>
        public string ConsequencePreview { get; set; }

        /// <summary>
        /// Commands to execute when this choice is selected.
        /// </summary>
        public List<CommandData> OnSelectCommands { get; set; }

        public ChoiceData() {
            OnSelectCommands = new List<CommandData>();
        }

        public ChoiceData(string id, string text, string nextNodeId) : this() {
            Id = id;
            Text = text;
            NextNodeId = nextNodeId;
        }
    }
}