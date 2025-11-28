using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// ScriptableObject containing default voice sounds for characters without custom sounds.
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultVoiceSounds", menuName = "GimGim/Conversation System/Default Voice Sounds")]
    public class DefaultVoiceSounds : ScriptableObject {
        [Tooltip("Default voice sounds used when a character has no custom sounds.")]
        [SerializeField]
        private AudioClip[] defaultSounds;

        [Tooltip("Alternate sound sets for variety.")]
        [SerializeField]
        private AudioClip[] alternateSoundsHigh;

        [SerializeField]
        private AudioClip[] alternateSoundsLow;

        /// <summary>
        /// Gets the default sounds array.
        /// </summary>
        public AudioClip[] DefaultSounds => defaultSounds;

        /// <summary>
        /// Gets alternate high-pitched sounds.
        /// </summary>
        public AudioClip[] AlternateSoundsHigh => alternateSoundsHigh;

        /// <summary>
        /// Gets alternate low-pitched sounds.
        /// </summary>
        public AudioClip[] AlternateSoundsLow => alternateSoundsLow;

        /// <summary>
        /// Gets a random sound from the default set.
        /// </summary>
        /// <returns>A random audio clip, or null if empty.</returns>
        public AudioClip GetRandomDefaultSound() {
            if (defaultSounds == null || defaultSounds.Length == 0) {
                return null;
            }

            return defaultSounds[Random.Range(0, defaultSounds.Length)];
        }

        /// <summary>
        /// Gets a random sound appropriate for a pitch level.
        /// </summary>
        /// <param name="pitch">The pitch level (1.0 = normal).</param>
        /// <returns>A random audio clip.</returns>
        public AudioClip GetRandomSoundForPitch(float pitch) {
            AudioClip[] sounds;

            if (pitch > 1.3f && alternateSoundsHigh != null && alternateSoundsHigh.Length > 0) {
                sounds = alternateSoundsHigh;
            }
            else if (pitch < 0.7f && alternateSoundsLow != null && alternateSoundsLow.Length > 0) {
                sounds = alternateSoundsLow;
            }
            else {
                sounds = defaultSounds;
            }

            if (sounds == null || sounds.Length == 0) {
                return GetRandomDefaultSound();
            }

            return sounds[Random.Range(0, sounds.Length)];
        }
    }
}