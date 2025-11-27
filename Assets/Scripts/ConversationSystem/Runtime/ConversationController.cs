using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// MonoBehaviour controller for managing conversations.
    /// </summary>
    public class ConversationController : MonoBehaviour {
        [Header("References")]
        [SerializeField]
        private ConversationDatabase conversationDatabase;

        [SerializeField]
        private CharacterDatabase characterDatabase;

        [SerializeField]
        private ConversationViewBase view;

        [Header("Settings")]
        [SerializeField]
        private bool autoInitialize = true;

        [SerializeField]
        private bool persistState = true;

        // Runtime components
        private ConversationContext _context;
        private ConversationRunner _runner;
        private ConversationStateManager _stateManager;
        private CompositeVariableProvider _variableProvider;
        private CharacterProvider _characterProvider;
        private CharacterVariableProvider _characterVariableProvider;

        /// <summary>
        /// Gets whether a conversation is currently running.
        /// </summary>
        public bool IsRunning => _runner?.IsRunning ?? false;

        /// <summary>
        /// Gets the conversation context.
        /// </summary>
        public ConversationContext Context => _context;

        /// <summary>
        /// Gets the state manager.
        /// </summary>
        public ConversationStateManager StateManager => _stateManager;

        /// <summary>
        /// Event raised when a conversation starts.
        /// </summary>
        public event Action OnConversationStarted;

        /// <summary>
        /// Event raised when a conversation ends.
        /// </summary>
        public event Action OnConversationEnded;

        /// <summary>
        /// Event raised when an event is triggered from dialogue.
        /// </summary>
        public event Action<string, SerializableDictionary<string, string>> OnConversationEvent;

        private void Awake() {
            if (autoInitialize) {
                Initialize();
            }
        }

        private void OnDestroy() {
            Cleanup();
        }

        /// <summary>
        /// Initializes the conversation system.
        /// </summary>
        public void Initialize() {
            // Initialize databases
            conversationDatabase?.Initialize();
            characterDatabase?.Initialize();

            // Create state manager
            _stateManager = new ConversationStateManager();

            // Create variable provider
            _variableProvider = new CompositeVariableProvider();
            _variableProvider.AddProvider(_stateManager);
            _variableProvider.AddProvider(new BuiltInVariableProvider());

            // Create character provider
            if (characterDatabase != null) {
                _characterProvider = new CharacterProvider(characterDatabase);
                _characterVariableProvider = new CharacterVariableProvider(_characterProvider);
                _variableProvider.AddProvider(_characterVariableProvider);
            }

            // Create context
            _context = new ConversationContext {
                View = view,
                CharacterProvider = _characterProvider,
                StateManager = _stateManager,
                VariableProvider = _variableProvider
            };

            // Create condition context
            _context.ConditionContext = new ConditionContext(_variableProvider, _context.PredicateRegistry);

            // Create command context
            CommandContext commandContext = new(_stateManager);
            commandContext.OnEventTriggered += (eventName, parameters) => {
                _context.RaiseEventTriggered(eventName, parameters);
                OnConversationEvent?.Invoke(eventName, parameters);
            };
            _context.CommandContext = commandContext;

            // Hook up context events
            _context.OnEventTriggered += (eventName, parameters) => {
                OnConversationEvent?.Invoke(eventName, parameters);
            };

            // Create runner with conversation loader
            var processorRegistry = NodeProcessorRegistry.CreateDefault(LoadConversationAsync);
            _runner = new ConversationRunner(_context, processorRegistry);

            // Wire up runner events
            _runner.OnConversationStarted += () => OnConversationStarted?.Invoke();
            _runner.OnConversationEnded += () => OnConversationEnded?.Invoke();

            Debug.Log("ConversationController: Initialized.");
        }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        private void Cleanup() {
            if (IsRunning) {
                _runner?.Stop();
            }

            if (persistState && _stateManager != null) {
                // Save state
                var persistence = new ConversationStatePersistence();
                persistence.Save(_stateManager.GetStateData());
            }
        }

        /// <summary>
        /// Starts a conversation by ID.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <param name="startNodeId">Optional starting node ID.</param>
        public async Task StartConversationAsync(string conversationId, string startNodeId = null) {
            if (IsRunning) {
                Debug.LogWarning("ConversationController: A conversation is already running.");
                return;
            }

            ConversationData conversation = conversationDatabase?.GetConversation(conversationId);
            if (conversation == null) {
                Debug.LogError($"ConversationController: Conversation '{conversationId}' not found.");
                return;
            }

            await StartConversationAsync(conversation, startNodeId);
        }

        /// <summary>
        /// Starts a conversation with data.
        /// </summary>
        /// <param name="conversation">The conversation data.</param>
        /// <param name="startNodeId">Optional starting node ID.</param>
        public async Task StartConversationAsync(ConversationData conversation, string startNodeId = null) {
            if (IsRunning) {
                Debug.LogWarning("ConversationController: A conversation is already running.");
                return;
            }

            if (conversation == null) {
                Debug.LogError("ConversationController: Conversation is null.");
                return;
            }

            // Update command context with new conversation
            if (_context.CommandContext is CommandContext cmdCtx) {
                // CommandContext needs current conversation reference
            }

            // Register required predicates
            RegisterPredicates(conversation);

            await _runner.RunAsync(conversation, startNodeId);
        }

        /// <summary>
        /// Starts a conversation from JSON.
        /// </summary>
        /// <param name="json">The conversation JSON.</param>
        /// <param name="startNodeId">Optional starting node ID.</param>
        public async Task StartConversationFromJsonAsync(string json, string startNodeId = null) {
            ConversationData conversation = ConversationJsonUtility.FromJson(json);
            if (conversation == null) {
                Debug.LogError("ConversationController: Failed to parse conversation JSON.");
                return;
            }

            await StartConversationAsync(conversation, startNodeId);
        }

        /// <summary>
        /// Loads a conversation asynchronously (for jump nodes).
        /// </summary>
        private Task<ConversationData> LoadConversationAsync(string conversationId) {
            ConversationData conversation = conversationDatabase?.GetConversation(conversationId);
            return Task.FromResult(conversation);
        }

        /// <summary>
        /// Registers predicates required by a conversation.
        /// </summary>
        private void RegisterPredicates(ConversationData conversation) {
            if (conversation.RequiredPredicates == null) {
                return;
            }

            foreach (KeyValuePair<string, string> predicate in conversation.RequiredPredicates) {
                if (!_context.PredicateRegistry.Contains(predicate.Key)) {
                    Debug.LogWarning($"ConversationController: Required predicate '{predicate.Key}' is not registered.");
                }
            }
        }

        /// <summary>
        /// Registers a custom predicate.
        /// </summary>
        /// <param name="varName">The predicate name.</param>
        /// <param name="predicate">The predicate function.</param>
        public void RegisterPredicate(string varName, Func<SerializableDictionary<string, string>, bool> predicate) {
            _context?.PredicateRegistry.Register(varName, predicate);
        }

        /// <summary>
        /// Registers a custom variable provider.
        /// </summary>
        /// <param name="provider">The provider to register.</param>
        public void RegisterVariableProvider(IConversationVariableProvider provider) {
            _variableProvider?.AddProvider(provider);
        }

        /// <summary>
        /// Pauses the current conversation.
        /// </summary>
        public void Pause() {
            _runner?.Pause();
        }

        /// <summary>
        /// Resumes the current conversation.
        /// </summary>
        public void Resume() {
            _runner?.Resume();
        }

        /// <summary>
        /// Stops the current conversation.
        /// </summary>
        public void Stop() {
            _runner?.Stop();
        }

        /// <summary>
        /// Skips the current typewriter effect.
        /// </summary>
        public void SkipTypewriter() {
            _runner?.SkipTypewriter();
        }

        /// <summary>
        /// Sets a variable value.
        /// </summary>
        public void SetVariable(string varName, bool value) => _stateManager?.SetBool(varName, value);
        public void SetVariable(string varName, int value) => _stateManager?.SetInt(varName, value);
        public void SetVariable(string varName, float value) => _stateManager?.SetFloat(varName, value);
        public void SetVariable(string varName, string value) => _stateManager?.SetString(varName, value);

        /// <summary>
        /// Gets a variable value.
        /// </summary>
        public bool GetBool(string varName, bool defaultValue = false) {
            return _stateManager?.TryGetBool(varName, out bool value) == true ? value : defaultValue;
        }

        public int GetInt(string varName, int defaultValue = 0) {
            return _stateManager?.TryGetInt(varName, out int value) == true ? value : defaultValue;
        }

        public float GetFloat(string varName, float defaultValue = 0f) {
            return _stateManager?.TryGetFloat(varName, out float value) == true ? value : defaultValue;
        }

        public string GetString(string varName, string defaultValue = "") {
            return _stateManager?.TryGetString(varName, out string value) == true ? value : defaultValue;
        }

        /// <summary>
        /// Saves the current state.
        /// </summary>
        public void SaveState() {
            if (_stateManager == null) {
                return;
            }

            var persistence = new ConversationStatePersistence();
            persistence.Save(_stateManager.GetStateData());
        }

        /// <summary>
        /// Loads saved state.
        /// </summary>
        public void LoadState() {
            var persistence = new ConversationStatePersistence();
            ConversationStateData data = persistence.Load();
            
            if (data != null) {
                _stateManager?.LoadStateData(data);
            }
        }

        /// <summary>
        /// Resets all state.
        /// </summary>
        public void ResetState() {
            _stateManager?.Reset();
        }
    }
}