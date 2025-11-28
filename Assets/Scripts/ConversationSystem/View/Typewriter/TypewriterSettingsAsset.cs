using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// ScriptableObject wrapper for TypewriterSettings.
    /// Allows sharing settings across multiple dialogues.
    /// </summary>
    [CreateAssetMenu(fileName = "TypewriterSettings", menuName = "GimGim/Conversation System/Typewriter Settings")]
    public class TypewriterSettingsAsset : ScriptableObject {
        [SerializeField]
        private TypewriterSettings settings;

        /// <summary>
        /// Gets the typewriter settings.
        /// </summary>
        public TypewriterSettings Settings => settings ??= new TypewriterSettings();

        /// <summary>
        /// Creates a copy of the settings.
        /// </summary>
        /// <returns>A new TypewriterSettings instance with the same values.</returns>
        public TypewriterSettings CreateCopy() {
            // Use JSON serialization for deep copy
            string json = JsonUtility.ToJson(settings);
            return JsonUtility.FromJson<TypewriterSettings>(json);
        }
    }
}