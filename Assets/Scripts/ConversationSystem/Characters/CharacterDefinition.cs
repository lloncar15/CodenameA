using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// ScriptableObject defining a character for the conversation system.
    /// Contains display information, portraits, and voice settings.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "GimGim/Conversation System/Character Definition")]
    public class CharacterDefinition : ScriptableObject {
        [Header("Identity")]
        [Tooltip("Unique identifier for this character. Used to reference in conversation JSON.")]
        [SerializeField]
        private string characterId;

        [Tooltip("Display name shown in dialogue UI.")]
        [SerializeField]
        private string displayName;

        [Tooltip("Optional short name or nickname.")]
        [SerializeField]
        private string shortName;

        [Header("Appearance")]
        [Tooltip("Default portrait sprite when no emotion is specified.")]
        [SerializeField]
        private Sprite defaultPortrait;

        [Tooltip("Color associated with this character (for UI elements, name display, etc.).")]
        [SerializeField]
        private Color characterColor = Color.white;

        [Header("Emotions")]
        [Tooltip("List of available emotions/expressions for this character.")]
        [SerializeField]
        private List<CharacterEmotion> emotions = new List<CharacterEmotion>();

        [Header("Voice")]
        [Tooltip("Voice settings for this character's dialogue sounds.")]
        [SerializeField]
        private CharacterVoiceSettings voiceSettings;

        [Header("Metadata")]
        [Tooltip("Optional description for editor reference.")]
        [SerializeField]
        [TextArea(2, 4)]
        private string description;

        [Tooltip("Custom metadata for game-specific extensions.")]
        [SerializeField]
        private List<StringKeyValuePair> metadata = new List<StringKeyValuePair>();

        // Runtime cache for emotion lookup
        private Dictionary<string, CharacterEmotion> _emotionCache;

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        public string CharacterId => characterId;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// Gets the short name.
        /// </summary>
        public string ShortName => !string.IsNullOrEmpty(shortName) ? shortName : displayName;

        /// <summary>
        /// Gets the default portrait.
        /// </summary>
        public Sprite DefaultPortrait => defaultPortrait;

        /// <summary>
        /// Gets the character color.
        /// </summary>
        public Color CharacterColor => characterColor;

        /// <summary>
        /// Gets the voice settings.
        /// </summary>
        public CharacterVoiceSettings VoiceSettings => voiceSettings ??= new CharacterVoiceSettings();

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets all emotions.
        /// </summary>
        public IReadOnlyList<CharacterEmotion> Emotions => emotions;

        /// <summary>
        /// Initializes the emotion cache for faster lookup.
        /// </summary>
        private void EnsureEmotionCache() {
            if (_emotionCache != null) {
                return;
            }

            _emotionCache = new Dictionary<string, CharacterEmotion>(System.StringComparer.OrdinalIgnoreCase);
            foreach (CharacterEmotion emotion in emotions) {
                if (!string.IsNullOrEmpty(emotion.EmotionKey)) {
                    _emotionCache[emotion.EmotionKey] = emotion;
                }
            }
        }

        /// <summary>
        /// Gets an emotion by key.
        /// </summary>
        /// <param name="emotionKey">The emotion key to look up.</param>
        /// <returns>The emotion if found, null otherwise.</returns>
        public CharacterEmotion GetEmotion(string emotionKey) {
            if (string.IsNullOrEmpty(emotionKey)) {
                return null;
            }

            EnsureEmotionCache();
            return _emotionCache.GetValueOrDefault(emotionKey);
        }

        /// <summary>
        /// Gets the portrait for a specific emotion.
        /// Falls back to default portrait if emotion not found.
        /// </summary>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The portrait sprite.</returns>
        public Sprite GetPortrait(string emotionKey) {
            CharacterEmotion emotion = GetEmotion(emotionKey);
            if (emotion != null && emotion.Portrait != null) {
                return emotion.Portrait;
            }

            return defaultPortrait;
        }

        /// <summary>
        /// Gets voice pitch for a specific emotion.
        /// </summary>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The combined pitch value.</returns>
        public float GetVoicePitch(string emotionKey) {
            float basePitch = VoiceSettings.BasePitch;
            CharacterEmotion emotion = GetEmotion(emotionKey);
            float modifier = emotion?.VoicePitchModifier ?? 1.0f;
            return basePitch * modifier;
        }

        /// <summary>
        /// Gets voice speed for a specific emotion.
        /// </summary>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The speed modifier.</returns>
        public float GetVoiceSpeed(string emotionKey) {
            CharacterEmotion emotion = GetEmotion(emotionKey);
            return emotion?.VoiceSpeedModifier ?? 1.0f;
        }

        /// <summary>
        /// Checks if an emotion exists for this character.
        /// </summary>
        /// <param name="emotionKey">The emotion key to check.</param>
        /// <returns>True if emotion exists, false otherwise.</returns>
        public bool HasEmotion(string emotionKey) {
            EnsureEmotionCache();
            return _emotionCache.ContainsKey(emotionKey);
        }

        /// <summary>
        /// Gets all available emotion keys.
        /// </summary>
        /// <returns>Enumerable of emotion keys.</returns>
        public IEnumerable<string> GetEmotionKeys() {
            EnsureEmotionCache();
            return _emotionCache.Keys;
        }

        /// <summary>
        /// Gets a metadata value.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The metadata value or default.</returns>
        public string GetMetadata(string key, string defaultValue = "") {
            foreach (StringKeyValuePair kvp in metadata) {
                if (kvp.Key == key) {
                    return kvp.Value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Clears the emotion cache. Call after modifying emotions at runtime.
        /// </summary>
        public void ClearCache() {
            _emotionCache = null;
        }

        private void OnValidate() {
            // Clear cache when values change in editor
            _emotionCache = null;

            // Auto-generate ID from name if empty
            if (string.IsNullOrEmpty(characterId) && !string.IsNullOrEmpty(name)) {
                characterId = name.ToLower().Replace(" ", "_");
            }
        }
    }

    /// <summary>
    /// Serializable key-value pair for metadata.
    /// </summary>
    [System.Serializable]
    public class StringKeyValuePair {
        public string Key;
        public string Value;

        public StringKeyValuePair() { }

        public StringKeyValuePair(string key, string value) {
            Key = key;
            Value = value;
        }
    }
}