using System;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Represents a single emotion/expression for a character.
    /// Contains the portrait sprite and voice settings for this emotion.
    /// </summary>
    [Serializable]
    public class CharacterEmotion {
        [Tooltip("The unique key for this emotion (e.g., 'happy', 'sad', 'angry').")]
        [SerializeField]
        private string emotionKey;

        [Tooltip("The portrait sprite for this emotion.")]
        [SerializeField]
        private Sprite portrait;

        [Tooltip("Optional voice pitch modifier for this emotion (1.0 = normal).")]
        [SerializeField]
        [Range(0.5f, 2.0f)]
        private float voicePitchModifier;

        [Tooltip("Optional voice speed modifier for this emotion (1.0 = normal).")]
        [SerializeField]
        [Range(0.5f, 2.0f)]
        private float voiceSpeedModifier;

        /// <summary>
        /// Gets the emotion key.
        /// </summary>
        public string EmotionKey => emotionKey;

        /// <summary>
        /// Gets the portrait sprite.
        /// </summary>
        public Sprite Portrait => portrait;

        /// <summary>
        /// Gets the voice pitch modifier.
        /// </summary>
        public float VoicePitchModifier => voicePitchModifier;

        /// <summary>
        /// Gets the voice speed modifier.
        /// </summary>
        public float VoiceSpeedModifier => voiceSpeedModifier;

        /// <summary>
        /// Creates a new character emotion.
        /// </summary>
        public CharacterEmotion() {
            voicePitchModifier = 1.0f;
            voiceSpeedModifier = 1.0f;
        }

        /// <summary>
        /// Creates a new character emotion with specified values.
        /// </summary>
        /// <param name="key">The emotion key.</param>
        /// <param name="portrait">The portrait sprite.</param>
        /// <param name="pitchModifier">Voice pitch modifier.</param>
        /// <param name="speedModifier">Voice speed modifier.</param>
        public CharacterEmotion(string key, Sprite portrait, float pitchModifier = 1.0f, float speedModifier = 1.0f) {
            emotionKey = key;
            this.portrait = portrait;
            voicePitchModifier = pitchModifier;
            voiceSpeedModifier = speedModifier;
        }
    }
}