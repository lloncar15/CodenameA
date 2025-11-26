using System;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Configuration settings for the typewriter effect.
    /// </summary>
    [Serializable]
    public class TypewriterSettings {
        [Header("Speed")]
        [Tooltip("Base characters per second.")]
        [SerializeField]
        [Range(10f, 200f)]
        private float charactersPerSecond = 50f;

        [Tooltip("Minimum characters per second (when slowed).")]
        [SerializeField]
        [Range(5f, 50f)]
        private float minCharactersPerSecond = 10f;

        [Tooltip("Maximum characters per second (when sped up).")]
        [SerializeField]
        [Range(50f, 500f)]
        private float maxCharactersPerSecond = 150f;

        [Header("Punctuation Pauses")]
        [Tooltip("Enable automatic pauses after punctuation.")]
        [SerializeField]
        private bool autoPauseOnPunctuation = true;

        [Tooltip("Pause duration after periods, exclamation, question marks (seconds).")]
        [SerializeField]
        [Range(0f, 1f)]
        private float periodPauseDuration = 0.25f;

        [Tooltip("Pause duration after commas, colons, semicolons (seconds).")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float commaPauseDuration = 0.1f;

        [Tooltip("Pause duration after ellipsis '...' (seconds).")]
        [SerializeField]
        [Range(0f, 2f)]
        private float ellipsisPauseDuration = 0.5f;

        [Header("Behavior")]
        [Tooltip("Allow skipping to show all text instantly.")]
        [SerializeField]
        private bool allowSkip = true;

        [Tooltip("If true, skipping shows all text. If false, skipping speeds up.")]
        [SerializeField]
        private bool skipShowsAllText = true;

        [Tooltip("Speed multiplier when fast-forwarding (if skipShowsAllText is false).")]
        [SerializeField]
        [Range(2f, 20f)]
        private float fastForwardMultiplier = 5f;

        [Header("Rich Text")]
        [Tooltip("Preserve rich text tags (color, bold, etc.) during typewriter.")]
        [SerializeField]
        private bool preserveRichText = true;

        [Header("Sound")]
        [Tooltip("Enable typewriter sound effects.")]
        [SerializeField]
        private bool enableSound = true;

        [Tooltip("Play sound for every character, or only on syllables.")]
        [SerializeField]
        private SoundMode soundMode = SoundMode.EverySyllable;

        [Tooltip("Characters considered as syllable boundaries.")]
        [SerializeField]
        private string syllableCharacters = "aeiouAEIOU";

        /// <summary>
        /// Gets the base characters per second.
        /// </summary>
        public float CharactersPerSecond => charactersPerSecond;

        /// <summary>
        /// Gets the minimum characters per second.
        /// </summary>
        public float MinCharactersPerSecond => minCharactersPerSecond;

        /// <summary>
        /// Gets the maximum characters per second.
        /// </summary>
        public float MaxCharactersPerSecond => maxCharactersPerSecond;

        /// <summary>
        /// Gets whether auto-pause on punctuation is enabled.
        /// </summary>
        public bool AutoPauseOnPunctuation => autoPauseOnPunctuation;

        /// <summary>
        /// Gets the period pause duration.
        /// </summary>
        public float PeriodPauseDuration => periodPauseDuration;

        /// <summary>
        /// Gets the comma pause duration.
        /// </summary>
        public float CommaPauseDuration => commaPauseDuration;

        /// <summary>
        /// Gets the ellipsis pause duration.
        /// </summary>
        public float EllipsisPauseDuration => ellipsisPauseDuration;

        /// <summary>
        /// Gets whether skipping is allowed.
        /// </summary>
        public bool AllowSkip => allowSkip;

        /// <summary>
        /// Gets whether skip shows all text.
        /// </summary>
        public bool SkipShowsAllText => skipShowsAllText;

        /// <summary>
        /// Gets the fast forward multiplier.
        /// </summary>
        public float FastForwardMultiplier => fastForwardMultiplier;

        /// <summary>
        /// Gets whether rich text is preserved.
        /// </summary>
        public bool PreserveRichText => preserveRichText;

        /// <summary>
        /// Gets whether sound is enabled.
        /// </summary>
        public bool EnableSound => enableSound;

        /// <summary>
        /// Gets the sound mode.
        /// </summary>
        public SoundMode SoundMode => soundMode;

        /// <summary>
        /// Gets the syllable characters.
        /// </summary>
        public string SyllableCharacters => syllableCharacters;

        /// <summary>
        /// Gets the delay between characters in seconds.
        /// </summary>
        /// <param name="speedMultiplier">Speed multiplier (1.0 = normal).</param>
        /// <returns>Delay in seconds.</returns>
        public float GetCharacterDelay(float speedMultiplier = 1f) {
            float effectiveCps = Mathf.Clamp(
                charactersPerSecond * speedMultiplier,
                minCharactersPerSecond,
                maxCharactersPerSecond
            );
            return 1f / effectiveCps;
        }

        /// <summary>
        /// Gets the pause duration for a character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="nextCharacter">The next character (for ellipsis detection).</param>
        /// <returns>Pause duration in seconds, 0 if no pause.</returns>
        public float GetPauseDuration(char character, char? nextCharacter = null) {
            if (!autoPauseOnPunctuation) {
                return 0f;
            }

            // Check for ellipsis
            if (character == '.' && nextCharacter == '.') {
                return 0f; // Don't pause on individual dots in ellipsis
            }

            switch (character) {
                case '.':
                case '!':
                case '?':
                    return periodPauseDuration;
                case ',':
                case ';':
                case ':':
                    return commaPauseDuration;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Checks if a character should trigger a sound.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>True if sound should play.</returns>
        public bool ShouldPlaySound(char character) {
            if (!enableSound) {
                return false;
            }

            if (char.IsWhiteSpace(character)) {
                return false;
            }

            if (soundMode == SoundMode.EveryCharacter) {
                return char.IsLetterOrDigit(character);
            }

            // Syllable mode - play on vowels
            return syllableCharacters.IndexOf(character) >= 0;
        }
    }

    /// <summary>
    /// Sound playback modes for typewriter.
    /// </summary>
    public enum SoundMode {
        /// <summary>
        /// Play sound for every visible character.
        /// </summary>
        EveryCharacter,

        /// <summary>
        /// Play sound only on syllable characters (vowels).
        /// </summary>
        EverySyllable,

        /// <summary>
        /// Play sound on word boundaries.
        /// </summary>
        EveryWord
    }
}