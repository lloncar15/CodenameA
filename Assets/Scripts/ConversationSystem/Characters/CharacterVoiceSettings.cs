using System;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Voice settings for a character's "Animal Crossing" style gibberish voice.
    /// </summary>
    [Serializable]
    public class CharacterVoiceSettings {
        [Header("Base Voice")]
        [Tooltip("Base pitch of the character's voice (1.0 = normal).")]
        [SerializeField]
        [Range(0.5f, 2.0f)]
        private float basePitch = 1.0f;

        [Tooltip("Random pitch variation range (+/-).")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float pitchVariation = 0.1f;

        [Header("Timing")]
        [Tooltip("Base time between syllables in seconds.")]
        [SerializeField]
        [Range(0.02f, 0.2f)]
        private float syllableInterval = 0.08f;

        [Tooltip("Random variation in syllable timing (+/-).")]
        [SerializeField]
        [Range(0f, 0.1f)]
        private float intervalVariation = 0.02f;

        [Header("Sound")]
        [Tooltip("Volume of the voice (0-1).")]
        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 0.7f;

        [Tooltip("Custom audio clips for this character's voice. If empty, uses default sounds.")]
        [SerializeField]
        private AudioClip[] customVoiceSounds;

        [Header("Advanced")]
        [Tooltip("Whether to play sounds for punctuation pauses.")]
        [SerializeField]
        private bool soundOnPunctuation = false;

        [Tooltip("Multiplier for pause duration at punctuation.")]
        [SerializeField]
        [Range(1f, 5f)]
        private float punctuationPauseMultiplier = 2.0f;

        /// <summary>
        /// Gets the base pitch.
        /// </summary>
        public float BasePitch => basePitch;

        /// <summary>
        /// Gets the pitch variation range.
        /// </summary>
        public float PitchVariation => pitchVariation;

        /// <summary>
        /// Gets the syllable interval.
        /// </summary>
        public float SyllableInterval => syllableInterval;

        /// <summary>
        /// Gets the interval variation.
        /// </summary>
        public float IntervalVariation => intervalVariation;

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public float Volume => volume;

        /// <summary>
        /// Gets the custom voice sounds.
        /// </summary>
        public AudioClip[] CustomVoiceSounds => customVoiceSounds;

        /// <summary>
        /// Gets whether to play sounds on punctuation.
        /// </summary>
        public bool SoundOnPunctuation => soundOnPunctuation;

        /// <summary>
        /// Gets the punctuation pause multiplier.
        /// </summary>
        public float PunctuationPauseMultiplier => punctuationPauseMultiplier;

        /// <summary>
        /// Creates default voice settings.
        /// </summary>
        public CharacterVoiceSettings() {
            basePitch = 1.0f;
            pitchVariation = 0.1f;
            syllableInterval = 0.08f;
            intervalVariation = 0.02f;
            volume = 0.7f;
            soundOnPunctuation = false;
            punctuationPauseMultiplier = 2.0f;
        }

        /// <summary>
        /// Gets a randomized pitch value.
        /// </summary>
        /// <param name="emotionModifier">Optional emotion-based pitch modifier.</param>
        /// <returns>A randomized pitch value.</returns>
        public float GetRandomizedPitch(float emotionModifier = 1.0f) {
            float variation = UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            return (basePitch + variation) * emotionModifier;
        }

        /// <summary>
        /// Gets a randomized syllable interval.
        /// </summary>
        /// <param name="emotionModifier">Optional emotion-based speed modifier.</param>
        /// <returns>A randomized interval value.</returns>
        public float GetRandomizedInterval(float emotionModifier = 1.0f) {
            float variation = UnityEngine.Random.Range(-intervalVariation, intervalVariation);
            return (syllableInterval + variation) / emotionModifier;
        }

        /// <summary>
        /// Gets a random voice sound clip.
        /// </summary>
        /// <returns>A random audio clip, or null if none available.</returns>
        public AudioClip GetRandomVoiceClip() {
            if (customVoiceSounds == null || customVoiceSounds.Length == 0) {
                return null;
            }

            int index = UnityEngine.Random.Range(0, customVoiceSounds.Length);
            return customVoiceSounds[index];
        }
    }
}