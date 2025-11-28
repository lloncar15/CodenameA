using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Data passed to the view for displaying a text node.
    /// </summary>
    public class TextNodeViewData {
        /// <summary>
        /// The text content to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The speaker's display name (null for narrator).
        /// </summary>
        public string SpeakerName { get; set; }

        /// <summary>
        /// The speaker's portrait sprite.
        /// </summary>
        public Sprite Portrait { get; set; }

        /// <summary>
        /// The speaker's color for UI elements.
        /// </summary>
        public Color SpeakerColor { get; set; } = Color.white;

        /// <summary>
        /// The current expression key.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Voice settings for the speaker.
        /// </summary>
        public CharacterVoiceSettings VoiceSettings { get; set; }

        /// <summary>
        /// Emotion-based pitch modifier.
        /// </summary>
        public float EmotionPitchModifier { get; set; } = 1f;

        /// <summary>
        /// Emotion-based speed modifier.
        /// </summary>
        public float EmotionSpeedModifier { get; set; } = 1f;

        /// <summary>
        /// Whether this text requires player input to advance.
        /// </summary>
        public bool RequiresInput { get; set; } = true;

        /// <summary>
        /// Auto-advance delay if RequiresInput is false.
        /// </summary>
        public float AutoAdvanceDelay { get; set; }

        /// <summary>
        /// Whether to use typewriter effect.
        /// </summary>
        public bool UseTypewriter { get; set; } = true;
    }

    /// <summary>
    /// Data for a single choice option.
    /// </summary>
    public class ChoiceOptionViewData {
        /// <summary>
        /// The choice ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The display text for this choice.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Whether this choice is currently selectable.
        /// </summary>
        public bool IsSelectable { get; set; } = true;

        /// <summary>
        /// Whether this choice is visible.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Reason why choice is unavailable (for greyed-out choices).
        /// </summary>
        public string UnavailableReason { get; set; }

        /// <summary>
        /// Preview of consequences for this choice.
        /// </summary>
        public string ConsequencePreview { get; set; }

        /// <summary>
        /// Index in the choice list.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Data passed to the view for displaying choices.
    /// </summary>
    public class ChoiceNodeViewData {
        /// <summary>
        /// Optional prompt text above choices.
        /// </summary>
        public string PromptText { get; set; }

        /// <summary>
        /// The speaker's display name (if any).
        /// </summary>
        public string SpeakerName { get; set; }

        /// <summary>
        /// The speaker's portrait sprite.
        /// </summary>
        public Sprite Portrait { get; set; }

        /// <summary>
        /// The speaker's color.
        /// </summary>
        public Color SpeakerColor { get; set; } = Color.white;

        /// <summary>
        /// The available choices.
        /// </summary>
        public List<ChoiceOptionViewData> Choices { get; set; }

        /// <summary>
        /// Time limit for making a choice (0 = no limit).
        /// </summary>
        public float TimeLimit { get; set; }

        /// <summary>
        /// Whether to use typewriter for prompt text.
        /// </summary>
        public bool UseTypewriter { get; set; } = true;

        public ChoiceNodeViewData() {
            Choices = new List<ChoiceOptionViewData>();
        }
    }

    /// <summary>
    /// Data for updating the speaker's expression mid-dialogue.
    /// </summary>
    public class ExpressionUpdateData {
        /// <summary>
        /// The new expression key.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The new portrait sprite.
        /// </summary>
        public Sprite Portrait { get; set; }

        /// <summary>
        /// New pitch modifier.
        /// </summary>
        public float PitchModifier { get; set; } = 1f;

        /// <summary>
        /// New speed modifier.
        /// </summary>
        public float SpeedModifier { get; set; } = 1f;
    }
}