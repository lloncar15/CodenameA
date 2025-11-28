namespace GimGim.ConversationSystem {
    /// <summary>
    /// Variable provider that exposes character data for use in conditions.
    /// Allows conditions like "speaker.emotion == happy".
    /// </summary>
    public class CharacterVariableProvider : IConversationVariableProvider {
        private readonly ICharacterProvider _characterProvider;
        private string _currentSpeakerId;
        private string _currentEmotion;

        /// <inheritdoc />
        public int Priority => 50; // Medium priority

        /// <summary>
        /// Creates a new character variable provider.
        /// </summary>
        /// <param name="characterProvider">The character provider.</param>
        public CharacterVariableProvider(ICharacterProvider characterProvider) {
            _characterProvider = characterProvider;
        }

        /// <summary>
        /// Updates the current speaker context.
        /// </summary>
        /// <param name="speakerId">The current speaker's ID.</param>
        /// <param name="emotion">The current emotion.</param>
        public void SetCurrentSpeaker(string speakerId, string emotion) {
            _currentSpeakerId = speakerId;
            _currentEmotion = emotion;
        }

        /// <summary>
        /// Clears the current speaker context.
        /// </summary>
        public void ClearCurrentSpeaker() {
            _currentSpeakerId = null;
            _currentEmotion = null;
        }

        /// <inheritdoc />
        public bool TryGetBool(string variableName, out bool value) {
            value = false;

            if (string.IsNullOrEmpty(_currentSpeakerId)) {
                return false;
            }

            // Support: "speaker.hasEmotion.{emotionKey}"
            if (variableName.StartsWith("speaker.hasEmotion.")) {
                string emotionKey = variableName["speaker.hasEmotion.".Length..];
                CharacterDefinition character = _characterProvider.GetCharacter(_currentSpeakerId);
                value = character?.HasEmotion(emotionKey) ?? false;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool TryGetInt(string variableName, out int value) {
            value = default;
            return false; // No integer properties currently
        }

        /// <inheritdoc />
        public bool TryGetFloat(string variableName, out float value) {
            value = 0;

            if (string.IsNullOrEmpty(_currentSpeakerId)) {
                return false;
            }

            // Support: "speaker.voicePitch", "speaker.voiceSpeed"
            switch (variableName.ToLower()) {
                case "speaker.voicepitch":
                    value = _characterProvider.GetVoicePitch(_currentSpeakerId, _currentEmotion);
                    return true;
                case "speaker.voicespeed":
                    value = _characterProvider.GetVoiceSpeed(_currentSpeakerId, _currentEmotion);
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool TryGetString(string variableName, out string value) {
            value = null;

            if (string.IsNullOrEmpty(_currentSpeakerId)) {
                return false;
            }

            switch (variableName.ToLower()) {
                case "speaker.id":
                    value = _currentSpeakerId;
                    return true;
                case "speaker.name":
                    value = _characterProvider.GetDisplayName(_currentSpeakerId);
                    return true;
                case "speaker.emotion":
                    value = _currentEmotion ?? "";
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool HasVariable(string variableName) {
            string lower = variableName.ToLower();
            return lower.StartsWith("speaker.") && !string.IsNullOrEmpty(_currentSpeakerId);
        }
    }
}