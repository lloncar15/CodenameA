using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A conversation node that displays text to the player.
    /// Supports speaker assignment, expressions, and inline text commands.
    /// </summary>
    [Serializable]
    public class TextNode : ConversationNode {
        public override ConversationNodeType NodeType => ConversationNodeType.Text;

        /// <summary>
        /// The ID of the character speaking this line.
        /// Null or empty for narrator/system text.
        /// </summary>
        public string SpeakerId { get; set; }

        /// <summary>
        /// The emotion/expression key for the speaker.
        /// Used to select the appropriate portrait.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The text content to display.
        /// May contain inline commands like [pause:0.5] or [var:player_name].
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The ID of the next node to proceed to after this text is displayed.
        /// Null or empty indicates end of conversation.
        /// </summary>
        public string NextNodeId { get; set; }

        /// <summary>
        /// Commands to execute when this node is entered.
        /// </summary>
        public List<CommandData> OnEnterCommands { get; set; }

        /// <summary>
        /// Commands to execute when this node is exited.
        /// </summary>
        public List<CommandData> OnExitCommands { get; set; }

        /// <summary>
        /// Whether the player must manually advance past this text.
        /// If false, auto-advances after text is fully displayed (or after a delay).
        /// </summary>
        public bool RequiresInput { get; set; } = true;

        /// <summary>
        /// Auto-advance delay in seconds. Only used if RequiresInput is false.
        /// </summary>
        public float AutoAdvanceDelay { get; set; } = 0f;

        public TextNode() : base() {
            OnEnterCommands = new List<CommandData>();
            OnExitCommands = new List<CommandData>();
        }

        public TextNode(string id, string text, string nextNodeId = null) : base(id) {
            Text = text;
            NextNodeId = nextNodeId;
            OnEnterCommands = new List<CommandData>();
            OnExitCommands = new List<CommandData>();
        }

        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(Id)) {
                errorMessage = "TextNode: Id cannot be null or empty.";
                return false;
            }

            if (string.IsNullOrEmpty(Text)) {
                errorMessage = $"TextNode '{Id}': Text cannot be null or empty.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}