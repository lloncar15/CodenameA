using System.Text;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Helper class for building rich text strings compatible with TextMeshPro.
    /// </summary>
    public class RichTextBuilder {
        private readonly StringBuilder _builder;

        /// <summary>
        /// Creates a new rich text builder.
        /// </summary>
        public RichTextBuilder() {
            _builder = new StringBuilder();
        }

        /// <summary>
        /// Creates a new rich text builder with initial text.
        /// </summary>
        /// <param name="initialText">Initial text.</param>
        public RichTextBuilder(string initialText) {
            _builder = new StringBuilder(initialText);
        }

        /// <summary>
        /// Appends plain text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder Append(string text) {
            _builder.Append(text);
            return this;
        }

        /// <summary>
        /// Appends a character.
        /// </summary>
        /// <param name="c">Character to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder Append(char c) {
            _builder.Append(c);
            return this;
        }

        /// <summary>
        /// Appends colored text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <param name="color">Color for the text.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendColored(string text, Color color) {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            _builder.Append($"<color=#{hex}>{text}</color>");
            return this;
        }

        /// <summary>
        /// Appends bold text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendBold(string text) {
            _builder.Append($"<b>{text}</b>");
            return this;
        }

        /// <summary>
        /// Appends italic text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendItalic(string text) {
            _builder.Append($"<i>{text}</i>");
            return this;
        }

        /// <summary>
        /// Appends underlined text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendUnderline(string text) {
            _builder.Append($"<u>{text}</u>");
            return this;
        }

        /// <summary>
        /// Appends strikethrough text.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendStrikethrough(string text) {
            _builder.Append($"<s>{text}</s>");
            return this;
        }

        /// <summary>
        /// Appends text with a specific size.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <param name="sizePercent">Size as percentage (e.g., 150 for 150%).</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendSized(string text, int sizePercent) {
            _builder.Append($"<size={sizePercent}%>{text}</size>");
            return this;
        }

        /// <summary>
        /// Appends a line break.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendLine() {
            _builder.Append('\n');
            return this;
        }

        /// <summary>
        /// Appends text followed by a line break.
        /// </summary>
        /// <param name="text">Text to append.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder AppendLine(string text) {
            _builder.Append(text);
            _builder.Append('\n');
            return this;
        }

        /// <summary>
        /// Opens a color tag.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder BeginColor(Color color) {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            _builder.Append($"<color=#{hex}>");
            return this;
        }

        /// <summary>
        /// Closes a color tag.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder EndColor() {
            _builder.Append("</color>");
            return this;
        }

        /// <summary>
        /// Opens a bold tag.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder BeginBold() {
            _builder.Append("<b>");
            return this;
        }

        /// <summary>
        /// Closes a bold tag.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder EndBold() {
            _builder.Append("</b>");
            return this;
        }

        /// <summary>
        /// Clears the builder.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public RichTextBuilder Clear() {
            _builder.Clear();
            return this;
        }

        /// <summary>
        /// Gets the current length.
        /// </summary>
        public int Length => _builder.Length;

        /// <summary>
        /// Returns the built string.
        /// </summary>
        /// <returns>The rich text string.</returns>
        public override string ToString() {
            return _builder.ToString();
        }

        /// <summary>
        /// Implicit conversion to string.
        /// </summary>
        public static implicit operator string(RichTextBuilder builder) {
            return builder.ToString();
        }
    }
}