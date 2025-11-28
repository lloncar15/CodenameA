using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Component that listens to conversation events and exposes them as UnityEvents.
    /// </summary>
    public class ConversationEventListener : MonoBehaviour {
        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [SerializeField]
        private bool findControllerAutomatically = true;

        [Header("Conversation Events")]
        [SerializeField]
        private ConversationEvent onConversationStarted;

        [SerializeField]
        private ConversationEvent onConversationEnded;

        [SerializeField]
        private ConversationIdEvent onConversationStartedWithId;

        [Header("Node Events")]
        [SerializeField]
        private ConversationNodeEvent onNodeEntered;

        [SerializeField]
        private ConversationNodeEvent onNodeExited;

        [Header("Choice Events")]
        [SerializeField]
        private ChoiceSelectedEvent onChoiceSelected;

        [Header("Dialogue Events")]
        [SerializeField]
        private DialogueEventTriggered onDialogueEvent;

        [SerializeField]
        private DialogueEventWithParamsTriggered onDialogueEventWithParams;

        [Header("Variable Events")]
        [SerializeField]
        private VariableChangedEvent onVariableChanged;

        [Header("Speaker Events")]
        [SerializeField]
        private SpeakerChangedEvent onSpeakerChanged;

        [SerializeField]
        private ExpressionChangedEvent onExpressionChanged;

        // Current tracking
        private string _currentConversationId;
        private string _currentSpeakerId;

        private void Awake() {
            if (controller == null && findControllerAutomatically) {
                controller = FindAnyObjectByType<ConversationController>();
            }
        }

        private void OnEnable() {
            SubscribeToEvents();
        }

        private void OnDisable() {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Subscribes to controller events.
        /// </summary>
        private void SubscribeToEvents() {
            if (controller == null) {
                return;
            }

            controller.OnConversationStarted += HandleConversationStarted;
            controller.OnConversationEnded += HandleConversationEnded;
            controller.OnConversationEvent += HandleConversationEvent;

            // Subscribe to runner events if accessible
            if (controller.Context != null) {
                controller.Context.OnVariableChanged += HandleVariableChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from controller events.
        /// </summary>
        private void UnsubscribeFromEvents() {
            if (controller == null) {
                return;
            }

            controller.OnConversationStarted -= HandleConversationStarted;
            controller.OnConversationEnded -= HandleConversationEnded;
            controller.OnConversationEvent -= HandleConversationEvent;

            if (controller.Context != null) {
                controller.Context.OnVariableChanged -= HandleVariableChanged;
            }
        }

        /// <summary>
        /// Handles conversation started.
        /// </summary>
        private void HandleConversationStarted() {
            _currentConversationId = controller.Context?.CurrentConversationId;
            
            onConversationStarted?.Invoke();
            
            if (!string.IsNullOrEmpty(_currentConversationId)) {
                onConversationStartedWithId?.Invoke(_currentConversationId);
            }
        }

        /// <summary>
        /// Handles conversation ended.
        /// </summary>
        private void HandleConversationEnded() {
            onConversationEnded?.Invoke();
            _currentConversationId = null;
            _currentSpeakerId = null;
        }

        /// <summary>
        /// Handles dialogue events from commands.
        /// </summary>
        private void HandleConversationEvent(string eventName, SerializableDictionary<string, string> parameters) {
            onDialogueEvent?.Invoke(eventName);

            if (parameters != null && parameters.Count > 0) {
                string paramsJson = JsonUtility.ToJson(parameters);
                onDialogueEventWithParams?.Invoke(eventName, paramsJson);
            }
        }

        /// <summary>
        /// Handles variable changes.
        /// </summary>
        private void HandleVariableChanged(string varName, object value) {
            onVariableChanged?.Invoke(varName, value?.ToString() ?? "null");
        }

        /// <summary>
        /// Manually triggers a speaker change event.
        /// Called by the view or processor when speaker changes.
        /// </summary>
        public void NotifySpeakerChanged(string speakerId, string speakerName) {
            if (_currentSpeakerId != speakerId) {
                _currentSpeakerId = speakerId;
                onSpeakerChanged?.Invoke(speakerId, speakerName);
            }
        }

        /// <summary>
        /// Manually triggers an expression change event.
        /// </summary>
        public void NotifyExpressionChanged(string speakerId, string expressionKey) {
            onExpressionChanged?.Invoke(speakerId, expressionKey);
        }

        /// <summary>
        /// Manually triggers a choice selected event.
        /// </summary>
        public void NotifyChoiceSelected(string nodeId, string choiceId) {
            onChoiceSelected?.Invoke(nodeId, choiceId);
        }

        /// <summary>
        /// Manually triggers a node entered event.
        /// </summary>
        public void NotifyNodeEntered(ConversationNode node) {
            onNodeEntered?.Invoke(node);

            // Check for speaker/expression changes on text nodes
            if (node is TextNode textNode) {
                if (!string.IsNullOrEmpty(textNode.SpeakerId)) {
                    string speakerName = controller.Context?.CharacterProvider?.GetDisplayName(textNode.SpeakerId);
                    NotifySpeakerChanged(textNode.SpeakerId, speakerName);
                }

                if (!string.IsNullOrEmpty(textNode.Expression)) {
                    NotifyExpressionChanged(textNode.SpeakerId, textNode.Expression);
                }
            }
        }

        /// <summary>
        /// Manually triggers a node exited event.
        /// </summary>
        public void NotifyNodeExited(ConversationNode node) {
            onNodeExited?.Invoke(node);
        }
    }
}