using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Tracks the current state of the typewriter effect.
    /// </summary>
    public class TypewriterState {
        /// <summary>
        /// The full text to display.
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// The currently visible text.
        /// </summary>
        public string VisibleText { get; set; }

        /// <summary>
        /// Current character index.
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Whether the typewriter is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether all text has been revealed.
        /// </summary>
        public bool IsComplete => CurrentIndex >= FullText?.Length;

        /// <summary>
        /// Whether the typewriter is paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Current pause remaining time.
        /// </summary>
        public float PauseTimeRemaining { get; set; }

        /// <summary>
        /// Current speed multiplier.
        /// </summary>
        public float SpeedMultiplier { get; set; } = 1f;

        /// <summary>
        /// Time accumulator for character timing.
        /// </summary>
        public float TimeAccumulator { get; set; }

        /// <summary>
        /// Commands to execute at specific positions.
        /// </summary>
        public List<PositionedCommand> PendingCommands { get; set; }

        /// <summary>
        /// Index of the next command to check.
        /// </summary>
        public int NextCommandIndex { get; set; }

        /// <summary>
        /// Whether fast-forward mode is active.
        /// </summary>
        public bool IsFastForward { get; set; }

        /// <summary>
        /// Creates a new typewriter state.
        /// </summary>
        public TypewriterState() {
            PendingCommands = new List<PositionedCommand>();
            Reset();
        }

        /// <summary>
        /// Resets the state for new text.
        /// </summary>
        public void Reset() {
            FullText = string.Empty;
            VisibleText = string.Empty;
            CurrentIndex = 0;
            IsActive = false;
            IsPaused = false;
            PauseTimeRemaining = 0f;
            SpeedMultiplier = 1f;
            TimeAccumulator = 0f;
            PendingCommands.Clear();
            NextCommandIndex = 0;
            IsFastForward = false;
        }

        /// <summary>
        /// Sets up the state for new text.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="commands">Commands associated with the text.</param>
        public void Setup(string text, List<PositionedCommand> commands = null) {
            Reset();
            FullText = text ?? string.Empty;
            IsActive = true;
            
            if (commands != null) {
                PendingCommands.AddRange(commands);
            }
        }

        /// <summary>
        /// Gets the current character.
        /// </summary>
        /// <returns>The current character, or null if at end.</returns>
        public char? GetCurrentCharacter() {
            if (string.IsNullOrEmpty(FullText) || CurrentIndex >= FullText.Length) {
                return null;
            }
            return FullText[CurrentIndex];
        }

        /// <summary>
        /// Gets the next character (lookahead).
        /// </summary>
        /// <returns>The next character, or null if at end.</returns>
        public char? GetNextCharacter() {
            if (string.IsNullOrEmpty(FullText) || CurrentIndex + 1 >= FullText.Length) {
                return null;
            }
            return FullText[CurrentIndex + 1];
        }

        /// <summary>
        /// Advances to the next character.
        /// </summary>
        /// <returns>True if advanced, false if at end.</returns>
        public bool Advance() {
            if (IsComplete) {
                return false;
            }

            CurrentIndex++;
            VisibleText = FullText.Substring(0, CurrentIndex);
            return true;
        }

        /// <summary>
        /// Completes the typewriter, showing all text.
        /// </summary>
        public void Complete() {
            CurrentIndex = FullText?.Length ?? 0;
            VisibleText = FullText ?? string.Empty;
            IsActive = false;
            IsPaused = false;
        }
    }
}