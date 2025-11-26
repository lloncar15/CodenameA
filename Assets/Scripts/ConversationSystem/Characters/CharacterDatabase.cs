using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// ScriptableObject database for managing character definitions.
    /// Provides centralized access to all characters in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "GimGim/Conversation System/Character Database")]
    public class CharacterDatabase : ScriptableObject {
        [Tooltip("List of all character definitions.")]
        [SerializeField]
        private List<CharacterDefinition> characters = new List<CharacterDefinition>();

        [Tooltip("Default character to use when a character ID is not found.")]
        [SerializeField]
        private CharacterDefinition defaultCharacter;

        // Runtime cache for character lookup
        private Dictionary<string, CharacterDefinition> _characterCache;

        /// <summary>
        /// Gets the number of characters in the database.
        /// </summary>
        public int Count => characters.Count;

        /// <summary>
        /// Gets the default character.
        /// </summary>
        public CharacterDefinition DefaultCharacter => defaultCharacter;

        /// <summary>
        /// Initializes the character cache for faster lookup.
        /// </summary>
        public void Initialize() {
            BuildCache();
        }

        /// <summary>
        /// Builds or rebuilds the character cache.
        /// </summary>
        private void BuildCache() {
            _characterCache = new Dictionary<string, CharacterDefinition>(System.StringComparer.OrdinalIgnoreCase);
            
            foreach (CharacterDefinition character in characters) {
                if (character == null) {
                    continue;
                }

                string id = character.CharacterId;
                if (string.IsNullOrEmpty(id)) {
                    Debug.LogWarning($"CharacterDatabase: Character '{character.name}' has no ID.");
                    continue;
                }

                if (_characterCache.TryAdd(id, character)) continue;
                
                Debug.LogWarning($"CharacterDatabase: Duplicate character ID '{id}'.");
            }

            Debug.Log($"CharacterDatabase: Initialized with {_characterCache.Count} characters.");
        }

        /// <summary>
        /// Ensures the cache is built.
        /// </summary>
        private void EnsureCache() {
            if (_characterCache == null) {
                BuildCache();
            }
        }

        /// <summary>
        /// Gets a character by ID.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The character definition, or default character if not found.</returns>
        public CharacterDefinition GetCharacter(string characterId) {
            if (string.IsNullOrEmpty(characterId)) {
                return defaultCharacter;
            }

            EnsureCache();

            if (_characterCache.TryGetValue(characterId, out CharacterDefinition character)) {
                return character;
            }

            Debug.LogWarning($"CharacterDatabase: Character '{characterId}' not found, using default.");
            return defaultCharacter;
        }

        /// <summary>
        /// Tries to get a character by ID without fallback.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="character">The character if found.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool TryGetCharacter(string characterId, out CharacterDefinition character) {
            EnsureCache();
            return _characterCache.TryGetValue(characterId, out character);
        }

        /// <summary>
        /// Checks if a character exists in the database.
        /// </summary>
        /// <param name="characterId">The character ID to check.</param>
        /// <returns>True if character exists, false otherwise.</returns>
        public bool HasCharacter(string characterId) {
            EnsureCache();
            return _characterCache.ContainsKey(characterId);
        }

        /// <summary>
        /// Gets all character IDs in the database.
        /// </summary>
        /// <returns>Enumerable of character IDs.</returns>
        public IEnumerable<string> GetAllCharacterIds() {
            EnsureCache();
            return _characterCache.Keys;
        }

        /// <summary>
        /// Gets all character definitions.
        /// </summary>
        /// <returns>Enumerable of character definitions.</returns>
        public IEnumerable<CharacterDefinition> GetAllCharacters() {
            return characters;
        }

        /// <summary>
        /// Adds a character to the database.
        /// </summary>
        /// <param name="character">The character to add.</param>
        /// <returns>True if added, false if null or duplicate.</returns>
        public bool AddCharacter(CharacterDefinition character) {
            if (character == null || string.IsNullOrEmpty(character.CharacterId)) {
                return false;
            }

            EnsureCache();

            if (_characterCache.ContainsKey(character.CharacterId)) {
                return false;
            }

            characters.Add(character);
            _characterCache[character.CharacterId] = character;
            return true;
        }

        /// <summary>
        /// Removes a character from the database.
        /// </summary>
        /// <param name="characterId">The character ID to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool RemoveCharacter(string characterId) {
            EnsureCache();

            if (!_characterCache.TryGetValue(characterId, out CharacterDefinition character)) {
                return false;
            }

            characters.Remove(character);
            _characterCache.Remove(characterId);
            return true;
        }

        /// <summary>
        /// Reloads the character cache.
        /// </summary>
        public void Reload() {
            _characterCache = null;
            BuildCache();
        }

        /// <summary>
        /// Gets the portrait for a character and emotion.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The portrait sprite, or null if not found.</returns>
        public Sprite GetPortrait(string characterId, string emotionKey) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.GetPortrait(emotionKey);
        }

        /// <summary>
        /// Gets the display name for a character.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The display name, or the ID if not found.</returns>
        public string GetDisplayName(string characterId) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.DisplayName ?? characterId;
        }

        /// <summary>
        /// Gets the character color.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The character color, or white if not found.</returns>
        public Color GetCharacterColor(string characterId) {
            CharacterDefinition character = GetCharacter(characterId);
            return character?.CharacterColor ?? Color.white;
        }

#if UNITY_EDITOR
        [ContextMenu("Validate Characters")]
        private void ValidateCharacters() {
            int valid = 0;
            int invalid = 0;
            HashSet<string> ids = new HashSet<string>();

            foreach (var character in characters) {
                if (character == null) {
                    Debug.LogWarning("CharacterDatabase: Null character reference found.");
                    invalid++;
                    continue;
                }

                if (string.IsNullOrEmpty(character.CharacterId)) {
                    Debug.LogWarning($"CharacterDatabase: Character '{character.name}' has no ID.");
                    invalid++;
                    continue;
                }

                if (!ids.Add(character.CharacterId)) {
                    Debug.LogWarning($"CharacterDatabase: Duplicate ID '{character.CharacterId}'.");
                    invalid++;
                    continue;
                }

                valid++;
            }

            Debug.Log($"CharacterDatabase validation: {valid} valid, {invalid} invalid.");
        }

        [ContextMenu("Auto-Populate from Folder")]
        private void AutoPopulateFromFolder() {
            // This would scan a folder for CharacterDefinition assets
            // Implementation depends on your project structure
            Debug.Log("CharacterDatabase: Auto-populate not implemented. Add CharacterDefinitions manually.");
        }
#endif

        private void OnValidate() {
            // Clear cache when values change in editor
            _characterCache = null;
        }
    }
}