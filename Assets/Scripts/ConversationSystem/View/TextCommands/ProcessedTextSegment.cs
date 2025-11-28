namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents a segment of processed text - either plain text or a command.
    /// </summary>
    public class ProcessedTextSegment {
        /// <summary>
        /// The type of this segment.
        /// </summary>
        public SegmentType Type { get; set; }

        /// <summary>
        /// The text content (for Text segments) or display text (for substituted variables).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The command result (for Command segments).
        /// </summary>
        public TextCommandResult Command { get; set; }

        /// <summary>
        /// Creates a text segment.
        /// </summary>
        /// <param name="text">The text content.</param>
        /// <returns>A new text segment.</returns>
        public static ProcessedTextSegment CreateText(string text) {
            return new ProcessedTextSegment {
                Type = SegmentType.Text,
                Text = text
            };
        }

        /// <summary>
        /// Creates a command segment.
        /// </summary>
        /// <param name="command">The command result.</param>
        /// <returns>A new command segment.</returns>
        public static ProcessedTextSegment CreateCommand(TextCommandResult command) {
            return new ProcessedTextSegment {
                Type = SegmentType.Command,
                Command = command
            };
        }
    }

    /// <summary>
    /// Types of processed text segments.
    /// </summary>
    public enum SegmentType {
        /// <summary>
        /// Plain text to display.
        /// </summary>
        Text,

        /// <summary>
        /// A command to execute.
        /// </summary>
        Command
    }
}