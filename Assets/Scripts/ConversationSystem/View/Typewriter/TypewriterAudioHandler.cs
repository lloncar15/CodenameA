// Assets/Scripts/ConversationSystem/View/Typewriter/TypewriterAudioHandler.cs
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Handles audio playback for the typewriter effect.
    /// Manages both character voice and UI sounds.
    /// </summary>
    public class TypewriterAudioHandler : MonoBehaviour {
        [Header("Audio Sources")]
        [SerializeField]
        [Tooltip("Audio source for character voice sounds.")]
        private AudioSource voiceSource;

        [SerializeField]
        [Tooltip("Audio source for UI sounds (advance, complete, etc.).")]
        private AudioSource uiSource;

        [Header("UI Sounds")]
        [SerializeField]
        private AudioClip advanceSound;

        [SerializeField]
        private AudioClip completeSound;

        [SerializeField]
        private AudioClip skipSound;

        [Header("Fallback Voice")]
        [SerializeField]
        [Tooltip("Use procedural sounds if no voice clips available.")]
        private bool useProceduralFallback = true;

        [SerializeField]
        private float proceduralBaseFrequency = 440f;

        private GibberishVoice _voice;
        private AudioClip[] _proceduralClips;

        /// <summary>
        /// Gets the gibberish voice component.
        /// </summary>
        public GibberishVoice Voice => _voice;

        /// <summary>
        /// Gets the voice audio source.
        /// </summary>
        public AudioSource VoiceSource => voiceSource;

        private void Awake() {
            InitializeVoice();
        }

        /// <summary>
        /// Initializes the voice system.
        /// </summary>
        private void InitializeVoice() {
            if (voiceSource == null) {
                // Create audio source if not assigned
                voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
                voiceSource.spatialBlend = 0f; // 2D sound
            }

            // Generate procedural clips as fallback
            if (useProceduralFallback) {
                _proceduralClips = ProceduralVoiceGenerator.GenerateBlipSet(5, proceduralBaseFrequency);
            }

            // Create default sounds asset with procedural clips
            DefaultVoiceSounds defaultSounds = null;
            //TODO: assign this via inspector or create dynamically

            _voice = new GibberishVoice(voiceSource, defaultSounds);
        }

        /// <summary>
        /// Plays a voice sound for a character.
        /// </summary>
        /// <param name="character">The character being displayed.</param>
        public void PlayVoiceSound(char character) {
            // If voice has no clips, use procedural
            if (_voice != null) {
                _voice.PlaySound(character);
            }
            else if (useProceduralFallback && _proceduralClips != null && _proceduralClips.Length > 0) {
                PlayProceduralSound(character);
            }
        }

        /// <summary>
        /// Plays a procedural sound.
        /// </summary>
        private void PlayProceduralSound(char character) {
            if (voiceSource == null || _proceduralClips == null) {
                return;
            }

            // Select clip based on character
            int index = Mathf.Abs(character.GetHashCode()) % _proceduralClips.Length;
            AudioClip clip = _proceduralClips[index];

            if (clip != null) {
                float pitch = 0.9f + (character % 10) * 0.02f; // Slight variation
                voiceSource.pitch = pitch;
                voiceSource.PlayOneShot(clip, 0.5f);
            }
        }

        /// <summary>
        /// Sets the voice for a character.
        /// </summary>
        /// <param name="settings">Voice settings.</param>
        /// <param name="emotionPitchMod">Emotion pitch modifier.</param>
        /// <param name="emotionSpeedMod">Emotion speed modifier.</param>
        public void SetVoice(CharacterVoiceSettings settings, float emotionPitchMod = 1f, float emotionSpeedMod = 1f) {
            _voice?.SetVoiceSettings(settings, emotionPitchMod, emotionSpeedMod);
        }

        /// <summary>
        /// Clears the current voice.
        /// </summary>
        public void ClearVoice() {
            _voice?.ClearVoiceSettings();
        }

        /// <summary>
        /// Plays the advance sound.
        /// </summary>
        public void PlayAdvanceSound() {
            PlayUISound(advanceSound);
        }

        /// <summary>
        /// Plays the complete sound.
        /// </summary>
        public void PlayCompleteSound() {
            PlayUISound(completeSound);
        }

        /// <summary>
        /// Plays the skip sound.
        /// </summary>
        public void PlaySkipSound() {
            PlayUISound(skipSound);
        }

        /// <summary>
        /// Plays a UI sound.
        /// </summary>
        private void PlayUISound(AudioClip clip) {
            if (clip == null) {
                return;
            }

            if (uiSource != null) {
                uiSource.PlayOneShot(clip);
            }
            else if (voiceSource != null) {
                // Fallback to voice source
                float originalPitch = voiceSource.pitch;
                voiceSource.pitch = 1f;
                voiceSource.PlayOneShot(clip);
                voiceSource.pitch = originalPitch;
            }
            // TODO: Call audio manager here when integrated
            // AudioManager.Instance.PlaySound(clip);
        }

        /// <summary>
        /// Stops all audio.
        /// </summary>
        public void StopAll() {
            _voice?.Stop();
            
            if (voiceSource != null) {
                voiceSource.Stop();
            }
            
            if (uiSource != null) {
                uiSource.Stop();
            }
        }

        private void OnDestroy() {
            // Clean up procedural clips
            if (_proceduralClips != null) {
                foreach (var clip in _proceduralClips) {
                    if (clip != null) {
                        Destroy(clip);
                    }
                }
            }
        }
    }
}