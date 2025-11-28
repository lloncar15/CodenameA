using System.Text;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Predefined command shortcuts and presets.
    /// </summary>
    public static class TextCommandPresets {
        /// <summary>
        /// Short pause (comma-like).
        /// </summary>
        public const string ShortPause = "[pause:0.15]";

        /// <summary>
        /// Medium pause (period-like).
        /// </summary>
        public const string MediumPause = "[pause:0.3]";

        /// <summary>
        /// Long pause (dramatic).
        /// </summary>
        public const string LongPause = "[pause:0.6]";

        /// <summary>
        /// Very long pause (tension building).
        /// </summary>
        public const string VeryLongPause = "[pause:1.0]";

        /// <summary>
        /// Slow speed for emphasis.
        /// </summary>
        public const string SlowSpeed = "[speed:0.5]";

        /// <summary>
        /// Normal speed.
        /// </summary>
        public const string NormalSpeed = "[speed:1.0]";

        /// <summary>
        /// Fast speed for excitement.
        /// </summary>
        public const string FastSpeed = "[speed:1.5]";

        /// <summary>
        /// Very fast speed for urgency.
        /// </summary>
        public const string VeryFastSpeed = "[speed:2.0]";

        /// <summary>
        /// Creates a pause command.
        /// </summary>
        /// <param name="seconds">Duration in seconds.</param>
        /// <returns>The command string.</returns>
        public static string Pause(float seconds) {
            return $"[pause:{seconds.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}]";
        }

        /// <summary>
        /// Creates a speed command.
        /// </summary>
        /// <param name="multiplier">Speed multiplier.</param>
        /// <returns>The command string.</returns>
        public static string Speed(float multiplier) {
            return $"[speed:{multiplier.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}]";
        }

        /// <summary>
        /// Creates an expression command.
        /// </summary>
        /// <param name="expressionKey">The expression key.</param>
        /// <returns>The command string.</returns>
        public static string Expression(string expressionKey) {
            return $"[expression:{expressionKey}]";
        }

        /// <summary>
        /// Creates an event command.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <returns>The command string.</returns>
        public static string Event(string eventName) {
            return $"[event:{eventName}]";
        }

        /// <summary>
        /// Creates a variable reference.
        /// </summary>
        /// <param name="variableName">The variable name.</param>
        /// <returns>The command string.</returns>
        public static string Variable(string variableName) {
            return $"[var:{variableName}]";
        }

        /// <summary>
        /// Processes text to add automatic pauses after punctuation.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <param name="periodPause">Pause after periods.</param>
        /// <param name="commaPause">Pause after commas.</param>
        /// <returns>Text with pause commands inserted.</returns>
        public static string AddAutoPauses(string text, float periodPause = 0.3f, float commaPause = 0.15f) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }

            StringBuilder builder = new();

            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                builder.Append(c);

                // Don't add pauses inside existing commands
                if (c == '[') {
                    int closeIndex = text.IndexOf(']', i);
                    if (closeIndex > i) {
                        builder.Append(text.Substring(i + 1, closeIndex - i));
                        i = closeIndex;
                        continue;
                    }
                }

                // Add pauses after punctuation
                if (c == '.' || c == '!' || c == '?') {
                    // Check it's not part of "..." or similar
                    if (i + 1 >= text.Length || !char.IsPunctuation(text[i + 1])) {
                        builder.Append(Pause(periodPause));
                    }
                }
                else if (c == ',' || c == ';' || c == ':') {
                    builder.Append(Pause(commaPause));
                }
            }

            return builder.ToString();
        }
    }
}