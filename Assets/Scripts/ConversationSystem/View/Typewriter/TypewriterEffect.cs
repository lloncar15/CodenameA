using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Handles the typewriter text reveal effect with sound and command support.
    /// </summary>
    public class TypewriterEffect : MonoBehaviour {
        [Header("References")]
        [SerializeField]
        private TMP_Text targetText;

        [SerializeField]
        private AudioSource voiceAudioSource;

        [SerializeField]
        private DefaultVoiceSounds defaultVoiceSounds;

        [Header("Settings")]
        [SerializeField]
        private TypewriterSettings settings;

        [SerializeField]
        private bool useUnscaledTime = true;

        // Components
        private GibberishVoice _voice;
        private TextCommandParser _parser;
        private TextCommandExecutor _commandExecutor;
        private TypewriterState _state;

        // Events
        /// <summary>
        /// Raised when a character is revealed.
        /// </summary>
        public event Action<char, int> OnCharacterRevealed;

        /// <summary>
        /// Raised when typewriter completes.
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Raised when typewriter is skipped.
        /// </summary>
        public event Action OnSkipped;

        /// <summary>
        /// Raised when a command is executed.
        /// </summary>
        public event Action<TextCommandResult> OnCommandExecuted;

        /// <summary>
        /// Raised when expression changes.
        /// </summary>
        public event Action<string> OnExpressionChange;

        /// <summary>
        /// Raised when an event is triggered from text.
        /// </summary>
        public event Action<string, SerializableDictionary<string, string>> OnEventTriggered;

        /// <summary>
        /// Gets whether the typewriter is currently active.
        /// </summary>
        public bool IsActive => _state?.IsActive ?? false;

        /// <summary>
        /// Gets whether the typewriter has completed.
        /// </summary>
        public bool IsComplete => _state?.IsComplete ?? true;

        /// <summary>
        /// Gets whether the typewriter is paused.
        /// </summary>
        public bool IsPaused => _state?.IsPaused ?? false;

        /// <summary>
        /// Gets the current settings.
        /// </summary>
        public TypewriterSettings Settings => settings;

        /// <summary>
        /// Gets the command executor for registering custom handlers.
        /// </summary>
        public TextCommandExecutor CommandExecutor => _commandExecutor;

        private void Awake() {
            Initialize();
        }

        /// <summary>
        /// Initializes the typewriter components.
        /// </summary>
        private void Initialize() {
            if (settings == null) {
                settings = new TypewriterSettings();
            }

            _state = new TypewriterState();
            _parser = new TextCommandParser();
            _commandExecutor = new TextCommandExecutor();

            // Set up voice if audio source is available
            if (voiceAudioSource != null) {
                _voice = new GibberishVoice(voiceAudioSource, defaultVoiceSounds);
            }

            // Wire up command executor events
            _commandExecutor.OnPause += HandlePauseCommand;
            _commandExecutor.OnSpeedChange += HandleSpeedCommand;
            _commandExecutor.OnExpressionChange += HandleExpressionCommand;
            _commandExecutor.OnEventTriggered += HandleEventCommand;
        }

        private void OnDestroy() {
            if (_commandExecutor != null) {
                _commandExecutor.OnPause -= HandlePauseCommand;
                _commandExecutor.OnSpeedChange -= HandleSpeedCommand;
                _commandExecutor.OnExpressionChange -= HandleExpressionCommand;
                _commandExecutor.OnEventTriggered -= HandleEventCommand;
            }
        }

        /// <summary>
        /// Starts displaying text with the typewriter effect.
        /// </summary>
        /// <param name="text">The text to display (may contain commands).</param>
        /// <param name="variableProvider">Optional provider for variable substitution.</param>
        public void ShowText(string text, IConversationVariableProvider variableProvider = null) {
            if (targetText == null) {
                Debug.LogError("TypewriterEffect: No target text component assigned.");
                return;
            }

            StopAllCoroutines();

            // Process text commands
            ProcessedText processed = _parser.ProcessText(text, variableProvider);
            
            _state.Setup(processed.DisplayText, processed.Commands);
            targetText.text = string.Empty;

            StartCoroutine(TypewriterCoroutine());
        }

        /// <summary>
        /// Shows text instantly without typewriter effect.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="variableProvider">Optional provider for variable substitution.</param>
        public void ShowTextInstant(string text, IConversationVariableProvider variableProvider = null) {
            if (targetText == null) {
                return;
            }

            StopAllCoroutines();

            // Process text commands
            ProcessedText processed = _parser.ProcessText(text, variableProvider);
            
            _state.Setup(processed.DisplayText, processed.Commands);
            _state.Complete();
            
            targetText.text = _state.FullText;

            // Execute all commands instantly
            ExecuteAllPendingCommands();

            OnComplete?.Invoke();
        }

        /// <summary>
        /// Skips to the end of the current text.
        /// </summary>
        public void Skip() {
            if (!IsActive || !settings.AllowSkip) {
                return;
            }

            if (settings.SkipShowsAllText) {
                CompleteImmediately();
            }
            else {
                _state.IsFastForward = true;
            }

            OnSkipped?.Invoke();
        }

        /// <summary>
        /// Completes the typewriter immediately.
        /// </summary>
        public void CompleteImmediately() {
            if (_state == null) {
                return;
            }

            StopAllCoroutines();
            
            _state.Complete();
            targetText.text = _state.FullText;
            
            // Execute remaining commands
            ExecuteAllPendingCommands();

            _voice?.Stop();
            OnComplete?.Invoke();
        }

        /// <summary>
        /// Pauses the typewriter.
        /// </summary>
        public void Pause() {
            if (_state != null) {
                _state.IsPaused = true;
            }
        }

        /// <summary>
        /// Resumes the typewriter.
        /// </summary>
        public void Resume() {
            if (_state != null) {
                _state.IsPaused = false;
            }
        }

        /// <summary>
        /// Sets the voice settings for the current speaker.
        /// </summary>
        /// <param name="voiceSettings">The voice settings.</param>
        /// <param name="emotionPitchModifier">Emotion pitch modifier.</param>
        /// <param name="emotionSpeedModifier">Emotion speed modifier.</param>
        public void SetVoice(CharacterVoiceSettings voiceSettings, float emotionPitchModifier = 1f, float emotionSpeedModifier = 1f) {
            _voice?.SetVoiceSettings(voiceSettings, emotionPitchModifier, emotionSpeedModifier);
        }

        /// <summary>
        /// Clears the voice settings.
        /// </summary>
        public void ClearVoice() {
            _voice?.ClearVoiceSettings();
        }

        /// <summary>
        /// Sets the typewriter settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        public void SetSettings(TypewriterSettings newSettings) {
            settings = newSettings ?? new TypewriterSettings();
        }

        /// <summary>
        /// Main typewriter coroutine.
        /// </summary>
        private IEnumerator TypewriterCoroutine() {
            while (!_state.IsComplete) {
                // Check for pause
                if (_state.IsPaused) {
                    yield return null;
                    continue;
                }

                // Handle command-triggered pause
                if (_state.PauseTimeRemaining > 0) {
                    float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    _state.PauseTimeRemaining -= deltaTime;
                    yield return null;
                    continue;
                }

                // Execute any commands at current position
                ExecuteCommandsAtPosition(_state.CurrentIndex);

                // Get current character
                char? currentChar = _state.GetCurrentCharacter();
                if (!currentChar.HasValue) {
                    break;
                }

                char c = currentChar.Value;

                // Handle rich text tags
                if (settings.PreserveRichText && c == '<') {
                    yield return StartCoroutine(HandleRichTextTag());
                    continue;
                }

                // Reveal character
                _state.Advance();
                targetText.text = _state.VisibleText;
                OnCharacterRevealed?.Invoke(c, _state.CurrentIndex - 1);

                // Play sound
                if (settings.ShouldPlaySound(c)) {
                    _voice?.PlaySound(c);
                }

                // Calculate delay
                float speedMultiplier = _state.SpeedMultiplier;
                if (_state.IsFastForward) {
                    speedMultiplier *= settings.FastForwardMultiplier;
                }

                float delay = settings.GetCharacterDelay(speedMultiplier);

                // Add punctuation pause
                float punctuationPause = settings.GetPauseDuration(c, _state.GetNextCharacter());
                if (punctuationPause > 0 && !_state.IsFastForward) {
                    delay += punctuationPause;
                }

                // Wait
                if (useUnscaledTime) {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else {
                    yield return new WaitForSeconds(delay);
                }
            }

            // Ensure final text is displayed
            targetText.text = _state.FullText;
            _state.IsActive = false;
            
            OnComplete?.Invoke();
        }

        /// <summary>
        /// Handles rich text tags by revealing them instantly.
        /// </summary>
        private IEnumerator HandleRichTextTag() {
            // Find the closing >
            int startIndex = _state.CurrentIndex;
            int endIndex = _state.FullText.IndexOf('>', startIndex);

            if (endIndex < 0) {
                // Malformed tag, just advance normally
                _state.Advance();
                targetText.text = _state.VisibleText;
                yield break;
            }

            // Reveal entire tag at once
            while (_state.CurrentIndex <= endIndex && !_state.IsComplete) {
                _state.Advance();
            }

            targetText.text = _state.VisibleText;
        }

        /// <summary>
        /// Executes commands at the specified position.
        /// </summary>
        private void ExecuteCommandsAtPosition(int position) {
            while (_state.NextCommandIndex < _state.PendingCommands.Count) {
                var posCmd = _state.PendingCommands[_state.NextCommandIndex];
                
                if (posCmd.CharacterIndex > position) {
                    break;
                }

                _commandExecutor.Execute(posCmd.Command);
                OnCommandExecuted?.Invoke(posCmd.Command);
                _state.NextCommandIndex++;
            }
        }

        /// <summary>
        /// Executes all remaining commands.
        /// </summary>
        private void ExecuteAllPendingCommands() {
            while (_state.NextCommandIndex < _state.PendingCommands.Count) {
                var posCmd = _state.PendingCommands[_state.NextCommandIndex];
                _commandExecutor.Execute(posCmd.Command);
                OnCommandExecuted?.Invoke(posCmd.Command);
                _state.NextCommandIndex++;
            }
        }

        /// <summary>
        /// Handles pause command from text.
        /// </summary>
        private void HandlePauseCommand(float duration) {
            _state.PauseTimeRemaining = duration;
        }

        /// <summary>
        /// Handles speed command from text.
        /// </summary>
        private void HandleSpeedCommand(float speedMultiplier) {
            _state.SpeedMultiplier = speedMultiplier;
        }

        /// <summary>
        /// Handles expression command from text.
        /// </summary>
        private void HandleExpressionCommand(string expressionKey) {
            OnExpressionChange?.Invoke(expressionKey);
        }

        /// <summary>
        /// Handles event command from text.
        /// </summary>
        private void HandleEventCommand(string eventName, SerializableDictionary<string, string> parameters) {
            OnEventTriggered?.Invoke(eventName, parameters);
        }

        /// <summary>
        /// Clears the displayed text.
        /// </summary>
        public void Clear() {
            StopAllCoroutines();
            _state?.Reset();
            
            if (targetText != null) {
                targetText.text = string.Empty;
            }

            _voice?.Stop();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor helper to test typewriter effect.
        /// </summary>
        [ContextMenu("Test Typewriter")]
        private void TestTypewriter() {
            ShowText("Hello! [pause:0.5]This is a [speed:0.5]test [speed:1]of the typewriter effect...");
        }
#endif
    }
}