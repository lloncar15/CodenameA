using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Generates Animal Crossing-style gibberish voice sounds.
    /// Creates a "language" effect by playing pitched sound clips synchronized with text.
    /// </summary>
    public class GibberishVoice {
        private readonly AudioSource _audioSource;
        private readonly DefaultVoiceSounds _defaultSounds;
        
        private CharacterVoiceSettings _currentVoiceSettings;
        private float _currentEmotionPitchModifier = 1f;
        private float _currentEmotionSpeedModifier = 1f;
        private float _lastPlayTime;
        private float _minTimeBetweenSounds = 0.05f;

        /// <summary>
        /// Creates a new gibberish voice generator.
        /// </summary>
        /// <param name="audioSource">The AudioSource to play sounds through.</param>
        /// <param name="defaultSounds">Default sounds when character has none.</param>
        public GibberishVoice(AudioSource audioSource, DefaultVoiceSounds defaultSounds = null) {
            _audioSource = audioSource;
            _defaultSounds = defaultSounds;
        }

        /// <summary>
        /// Sets the current voice settings.
        /// </summary>
        /// <param name="settings">The voice settings to use.</param>
        /// <param name="emotionPitchModifier">Emotion-based pitch modifier.</param>
        /// <param name="emotionSpeedModifier">Emotion-based speed modifier.</param>
        public void SetVoiceSettings(CharacterVoiceSettings settings, float emotionPitchModifier = 1f, float emotionSpeedModifier = 1f) {
            _currentVoiceSettings = settings;
            _currentEmotionPitchModifier = emotionPitchModifier;
            _currentEmotionSpeedModifier = emotionSpeedModifier;
        }

        /// <summary>
        /// Clears the current voice settings (uses defaults).
        /// </summary>
        public void ClearVoiceSettings() {
            _currentVoiceSettings = null;
            _currentEmotionPitchModifier = 1f;
            _currentEmotionSpeedModifier = 1f;
        }

        /// <summary>
        /// Plays a voice sound for a character.
        /// </summary>
        /// <param name="character">The character being displayed.</param>
        public void PlaySound(char character) {
            if (!_audioSource) {
                return;
            }

            // Rate limiting to prevent sound spam
            if (Time.time - _lastPlayTime < _minTimeBetweenSounds) {
                return;
            }

            AudioClip clip = GetSoundClip();
            if (!clip) {
                return;
            }

            float pitch = GetPitch(character);
            float volume = GetVolume();

            _audioSource.pitch = pitch;
            _audioSource.volume = volume;
            _audioSource.PlayOneShot(clip);

            _lastPlayTime = Time.time;
        }

        /// <summary>
        /// Gets the appropriate sound clip.
        /// </summary>
        private AudioClip GetSoundClip() {
            // Try character-specific sounds first
            if (_currentVoiceSettings != null) {
                AudioClip clip = _currentVoiceSettings.GetRandomVoiceClip();
                if (clip) {
                    return clip;
                }
            }

            // Fall back to default sounds
            if (_defaultSounds) {
                float pitch = _currentVoiceSettings?.BasePitch ?? 1f;
                return _defaultSounds.GetRandomSoundForPitch(pitch * _currentEmotionPitchModifier);
            }

            return null;
        }

        /// <summary>
        /// Gets the pitch for a character.
        /// </summary>
        private float GetPitch(char character) {
            float basePitch = 1f;
            float variation = 0.1f;

            if (_currentVoiceSettings != null) {
                basePitch = _currentVoiceSettings.BasePitch;
                variation = _currentVoiceSettings.PitchVariation;
            }

            // Apply emotion modifier
            basePitch *= _currentEmotionPitchModifier;

            // Add character-based variation for more natural sound
            float charVariation = GetCharacterPitchVariation(character);
            
            // Add random variation
            float randomVariation = Random.Range(-variation, variation);

            return Mathf.Clamp(basePitch + charVariation + randomVariation, 0.5f, 2f);
        }

        /// <summary>
        /// Gets pitch variation based on the character type.
        /// </summary>
        private float GetCharacterPitchVariation(char character) {
            // Vowels get slight pitch variation based on which vowel
            char lower = char.ToLower(character);
            return lower switch {
                'a' => 0f,
                'e' => 0.05f,
                'i' => 0.1f,
                'o' => -0.05f,
                'u' => -0.1f,
                _ => 0f
            };
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        private float GetVolume() {
            return _currentVoiceSettings?.Volume ?? 0.7f;
        }

        /// <summary>
        /// Gets the interval between sounds.
        /// </summary>
        /// <returns>Interval in seconds.</returns>
        public float GetSoundInterval() {
            float interval = 0.08f;

            if (_currentVoiceSettings != null) {
                interval = _currentVoiceSettings.GetRandomizedInterval(_currentEmotionSpeedModifier);
            }

            return interval;
        }

        /// <summary>
        /// Sets the minimum time between sounds.
        /// </summary>
        /// <param name="minTime">Minimum time in seconds.</param>
        public void SetMinTimeBetweenSounds(float minTime) {
            _minTimeBetweenSounds = Mathf.Max(0.01f, minTime);
        }

        /// <summary>
        /// Stops any currently playing sound.
        /// </summary>
        public void Stop() {
            if (_audioSource != null && _audioSource.isPlaying) {
                _audioSource.Stop();
            }
        }
    }
}