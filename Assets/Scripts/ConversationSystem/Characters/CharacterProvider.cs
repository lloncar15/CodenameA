using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Runtime service for accessing character data.
    /// Can be configured to use a CharacterDatabase or custom lookup.
    /// </summary>
    public class CharacterProvider : ICharacterProvider {
        private readonly CharacterDatabase _database;
        private readonly System.Func<string, CharacterDefinition> _customLookup;

        /// <summary>
        /// Creates a character provider using a CharacterDatabase.
        /// </summary>
        /// <param name="database">The character database.</param>
        public CharacterProvider(CharacterDatabase database) {
            _database = database;
            _database?.Initialize();
        }

        /// <summary>
        /// Creates a character provider using a custom lookup function.
        /// </summary>
        /// <param name="customLookup">Function that returns CharacterDefinition for an ID.</param>
        public CharacterProvider(System.Func<string, CharacterDefinition> customLookup) {
            _customLookup = customLookup;
        }

        /// <inheritdoc />
        public CharacterDefinition GetCharacter(string characterId) {
            if (_customLookup != null) {
                return _customLookup(characterId);
            }

            return _database?.GetCharacter(characterId);
        }

        /// <inheritdoc />
        public bool TryGetCharacter(string characterId, out CharacterDefinition character) {
            character = GetCharacter(characterId);
            return character != null;
        }

        /// <inheritdoc />
        public Sprite GetPortrait(string characterId, string emotionKey) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.GetPortrait(emotionKey);
        }

        /// <inheritdoc />
        public string GetDisplayName(string characterId) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.DisplayName ?? characterId ?? "???";
        }

        /// <inheritdoc />
        public Color GetCharacterColor(string characterId) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.CharacterColor ?? Color.white;
        }

        /// <inheritdoc />
        public CharacterVoiceSettings GetVoiceSettings(string characterId) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.VoiceSettings;
        }

        /// <inheritdoc />
        public float GetVoicePitch(string characterId, string emotionKey) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.GetVoicePitch(emotionKey) ?? 1.0f;
        }

        /// <inheritdoc />
        public float GetVoiceSpeed(string characterId, string emotionKey) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.GetVoiceSpeed(emotionKey) ?? 1.0f;
        }
    }
}