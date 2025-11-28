using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for providing character data to the conversation system.
    /// </summary>
    public interface ICharacterProvider {
        /// <summary>
        /// Gets a character definition by ID.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The character definition, or null if not found.</returns>
        CharacterDefinition GetCharacter(string characterId);

        /// <summary>
        /// Tries to get a character by ID.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="character">The character if found.</param>
        /// <returns>True if found, false otherwise.</returns>
        bool TryGetCharacter(string characterId, out CharacterDefinition character);

        /// <summary>
        /// Gets the portrait sprite for a character and emotion.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The portrait sprite.</returns>
        Sprite GetPortrait(string characterId, string emotionKey);

        /// <summary>
        /// Gets the display name for a character.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The display name.</returns>
        string GetDisplayName(string characterId);

        /// <summary>
        /// Gets the character color.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The character color.</returns>
        Color GetCharacterColor(string characterId);

        /// <summary>
        /// Gets the voice settings for a character.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <returns>The voice settings.</returns>
        CharacterVoiceSettings GetVoiceSettings(string characterId);

        /// <summary>
        /// Gets the voice pitch for a character and emotion.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The pitch value.</returns>
        float GetVoicePitch(string characterId, string emotionKey);

        /// <summary>
        /// Gets the voice speed for a character and emotion.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="emotionKey">The emotion key.</param>
        /// <returns>The speed modifier.</returns>
        float GetVoiceSpeed(string characterId, string emotionKey);
    }
}