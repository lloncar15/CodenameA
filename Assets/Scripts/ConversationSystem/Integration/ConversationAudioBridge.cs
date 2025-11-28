using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Bridges conversation events to the audio system.
    /// Handles music changes, ambient sounds, and sound effects triggered by dialogue.
    /// </summary>
    public class ConversationAudioBridge : MonoBehaviour {
        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private AudioSource ambientSource;

        [SerializeField]
        private AudioSource sfxSource;

        [Header("Settings")]
        [SerializeField]
        private float musicFadeDuration = 1f;

        [SerializeField]
        private float defaultMusicVolume = 0.5f;

        [SerializeField]
        private bool lowerMusicDuringDialogue = true;

        [SerializeField]
        [Range(0f, 1f)]
        private float dialogueMusicVolume = 0.3f;

        // State
        private float _originalMusicVolume;
        private bool _isInDialogue;

        private void Awake() {
            if (controller == null) {
                controller = FindAnyObjectByType<ConversationController>();
            }
        }

        private void OnEnable() {
            if (controller != null) {
                controller.OnConversationStarted += HandleConversationStarted;
                controller.OnConversationEnded += HandleConversationEnded;
                controller.OnConversationEvent += HandleConversationEvent;
            }
        }

        private void OnDisable() {
            if (controller != null) {
                controller.OnConversationStarted -= HandleConversationStarted;
                controller.OnConversationEnded -= HandleConversationEnded;
                controller.OnConversationEvent -= HandleConversationEvent;
            }
        }

        /// <summary>
        /// Handles conversation started.
        /// </summary>
        private void HandleConversationStarted() {
            _isInDialogue = true;

            if (lowerMusicDuringDialogue && musicSource) {
                _originalMusicVolume = musicSource.volume;
                StartCoroutine(FadeVolume(musicSource, dialogueMusicVolume, musicFadeDuration));
            }
        }

        /// <summary>
        /// Handles conversation ended.
        /// </summary>
        private void HandleConversationEnded() {
            _isInDialogue = false;

            if (lowerMusicDuringDialogue && musicSource) {
                StartCoroutine(FadeVolume(musicSource, _originalMusicVolume, musicFadeDuration));
            }
        }

        /// <summary>
        /// Handles dialogue events for audio commands.
        /// </summary>
        private void HandleConversationEvent(string eventName, SerializableDictionary<string, string> parameters) {
            switch (eventName.ToLower()) {
                case "playmusic":
                    HandlePlayMusic(parameters);
                    break;
                case "stopmusic":
                    HandleStopMusic(parameters);
                    break;
                case "playsfx":
                    HandlePlaySfx(parameters);
                    break;
                case "playambient":
                    HandlePlayAmbient(parameters);
                    break;
                case "stopambient":
                    HandleStopAmbient(parameters);
                    break;
                case "setmusicvolume":
                    HandleSetMusicVolume(parameters);
                    break;
            }
        }

        /// <summary>
        /// Handles play music event.
        /// </summary>
        private void HandlePlayMusic(SerializableDictionary<string, string> parameters) {
            string clipName = parameters.GetOrDefault("clip", "");
            bool loop = parameters.GetOrDefault("loop", "true").ToLower() == "true";
            float fadeIn = float.TryParse(parameters.GetOrDefault("fadeIn", "1"), out float f) ? f : 1f;

            AudioClip clip = LoadAudioClip(clipName);
            if (clip == null) {
                Debug.LogWarning($"ConversationAudioBridge: Music clip '{clipName}' not found.");
                return;
            }

            if (musicSource != null) {
                StartCoroutine(CrossfadeMusic(clip, loop, fadeIn));
            }
        }

        /// <summary>
        /// Handles stop music event.
        /// </summary>
        private void HandleStopMusic(SerializableDictionary<string, string> parameters) {
            float fadeOut = float.TryParse(parameters.GetOrDefault("fadeOut", "1"), out float f) ? f : 1f;

            if (musicSource != null) {
                StartCoroutine(FadeOutAndStop(musicSource, fadeOut));
            }
        }

        /// <summary>
        /// Handles play SFX event.
        /// </summary>
        private void HandlePlaySfx(SerializableDictionary<string, string> parameters) {
            string clipName = parameters.GetOrDefault("clip", "");
            float volume = float.TryParse(parameters.GetOrDefault("volume", "1"), out float v) ? v : 1f;

            AudioClip clip = LoadAudioClip(clipName);
            if (clip == null) {
                Debug.LogWarning($"ConversationAudioBridge: SFX clip '{clipName}' not found.");
                return;
            }

            if (sfxSource != null) {
                sfxSource.PlayOneShot(clip, volume);
            }
            else {
                AudioSource.PlayClipAtPoint(clip, Camera.main?.transform.position ?? Vector3.zero, volume);
            }
        }

        /// <summary>
        /// Handles play ambient event.
        /// </summary>
        private void HandlePlayAmbient(SerializableDictionary<string, string> parameters) {
            string clipName = parameters.GetOrDefault("clip", "");
            float volume = float.TryParse(parameters.GetOrDefault("volume", "0.5"), out float v) ? v : 0.5f;

            AudioClip clip = LoadAudioClip(clipName);
            if (clip == null || ambientSource == null) {
                return;
            }

            ambientSource.clip = clip;
            ambientSource.volume = volume;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        /// <summary>
        /// Handles stop ambient event.
        /// </summary>
        private void HandleStopAmbient(SerializableDictionary<string, string> parameters) {
            float fadeOut = float.TryParse(parameters.GetOrDefault("fadeOut", "1"), out float f) ? f : 1f;

            if (ambientSource != null) {
                StartCoroutine(FadeOutAndStop(ambientSource, fadeOut));
            }
        }

        /// <summary>
        /// Handles set music volume event.
        /// </summary>
        private void HandleSetMusicVolume(SerializableDictionary<string, string> parameters) {
            float volume = float.TryParse(parameters.GetOrDefault("volume", "0.5"), out float v) ? v : 0.5f;
            float fadeDuration = float.TryParse(parameters.GetOrDefault("fade", "0.5"), out float f) ? f : 0.5f;

            if (musicSource != null) {
                StartCoroutine(FadeVolume(musicSource, volume, fadeDuration));
            }
        }

        /// <summary>
        /// Loads an audio clip from Resources.
        /// Override for custom loading.
        /// </summary>
        protected virtual AudioClip LoadAudioClip(string clipName) {
            if (string.IsNullOrEmpty(clipName)) {
                return null;
            }

            return Resources.Load<AudioClip>($"Audio/{clipName}");
        }

        /// <summary>
        /// Fades audio source volume.
        /// </summary>
        private System.Collections.IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration) {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }

        /// <summary>
        /// Fades out and stops an audio source.
        /// </summary>
        private System.Collections.IEnumerator FadeOutAndStop(AudioSource source, float duration) {
            yield return FadeVolume(source, 0f, duration);
            source.Stop();
        }

        /// <summary>
        /// Crossfades to a new music track.
        /// </summary>
        private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip, bool loop, float duration) {
            float targetVolume = _isInDialogue ? dialogueMusicVolume : defaultMusicVolume;

            // Fade out current
            if (musicSource.isPlaying) {
                yield return FadeVolume(musicSource, 0f, duration / 2);
            }

            // Switch clip
            musicSource.clip = newClip;
            musicSource.loop = loop;
            musicSource.volume = 0f;
            musicSource.Play();

            // Fade in new
            yield return FadeVolume(musicSource, targetVolume, duration / 2);
        }
    }
}