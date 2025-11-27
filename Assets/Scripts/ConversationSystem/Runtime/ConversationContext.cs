using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Contains all dependencies and state for a conversation session.
    /// Acts as a service locator for conversation components.
    /// </summary>
    public class ConversationContext {
        /// <summary>
        /// The current conversation data.
        /// </summary>
        public ConversationData Conversation { get; set; }

        /// <summary>
        /// The current node being processed.
        /// </summary>
        public ConversationNode CurrentNode { get; set; }

        /// <summary>
        /// The conversation view for UI display.
        /// </summary>
        public IConversationView View { get; set; }

        /// <summary>
        /// The character provider for looking up characters.
        /// </summary>
        public ICharacterProvider CharacterProvider { get; set; }

        /// <summary>
        /// The state manager for variables and history.
        /// </summary>
        public IConversationStateManager StateManager { get; set; }

        /// <summary>
        /// The variable provider for condition evaluation.
        /// </summary>
        public IConversationVariableProvider VariableProvider { get; set; }

        /// <summary>
        /// The condition context for evaluating conditions.
        /// </summary>
        public IConditionContext ConditionContext { get; set; }

        /// <summary>
        /// The command context for executing commands.
        /// </summary>
        public ICommandContext CommandContext { get; set; }

        /// <summary>
        /// The predicate registry for custom conditions.
        /// </summary>
        public PredicateRegistry PredicateRegistry { get; set; }

        /// <summary>
        /// The condition factory for creating conditions.
        /// </summary>
        public IConditionFactory ConditionFactory { get; set; }

        /// <summary>
        /// The command factory for creating commands.
        /// </summary>
        public CommandFactory CommandFactory { get; set; }

        /// <summary>
        /// Stack of conversation states for nested conversations.
        /// </summary>
        public Stack<ConversationStackFrame> ConversationStack { get; }

        /// <summary>
        /// Custom data storage for game-specific extensions.
        /// </summary>
        public Dictionary<string, object> CustomData { get; }

        /// <summary>
        /// Event raised when a variable changes.
        /// </summary>
        public event Action<string, object> OnVariableChanged;

        /// <summary>
        /// Event raised when an event is triggered from commands.
        /// </summary>
        public event Action<string, SerializableDictionary<string, string>> OnEventTriggered;

        public ConversationContext() {
            ConversationStack = new Stack<ConversationStackFrame>();
            CustomData = new Dictionary<string, object>();
            PredicateRegistry = new PredicateRegistry();
            ConditionFactory = new ConditionFactory();
            CommandFactory = new CommandFactory();
        }

        /// <summary>
        /// Gets the current conversation ID.
        /// </summary>
        public string CurrentConversationId => Conversation?.Id;

        /// <summary>
        /// Gets the current node ID.
        /// </summary>
        public string CurrentNodeId => CurrentNode?.Id;

        /// <summary>
        /// Gets a node from the current conversation.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
        /// <returns>The node, or null if not found.</returns>
        public ConversationNode GetNode(string nodeId) {
            return Conversation?.GetNode(nodeId);
        }

        /// <summary>
        /// Sets a custom data value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetCustomData<T>(string key, T value) {
            CustomData[key] = value;
        }

        /// <summary>
        /// Gets a custom data value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The value or default.</returns>
        public T GetCustomData<T>(string key, T defaultValue = default) {
            if (CustomData.TryGetValue(key, out object value) && value is T typed) {
                return typed;
            }
            return defaultValue;
        }

        /// <summary>
        /// Pushes the current state onto the stack for nested conversation.
        /// </summary>
        /// <param name="returnNodeId">The node to return to.</param>
        public void PushState(string returnNodeId) {
            ConversationStack.Push(new ConversationStackFrame {
                ConversationId = Conversation?.Id,
                NodeId = returnNodeId,
                PredicateRegistry = PredicateRegistry
            });

            // Create new predicate registry for nested conversation
            PredicateRegistry = new PredicateRegistry();
        }

        /// <summary>
        /// Pops state from the stack after nested conversation.
        /// </summary>
        /// <returns>The stack frame, or null if empty.</returns>
        public ConversationStackFrame PopState() {
            if (ConversationStack.Count == 0) {
                return null;
            }

            ConversationStackFrame frame = ConversationStack.Pop();
            PredicateRegistry = frame.PredicateRegistry ?? new PredicateRegistry();
            return frame;
        }

        /// <summary>
        /// Raises the variable changed event.
        /// </summary>
        internal void RaiseVariableChanged(string name, object value) {
            OnVariableChanged?.Invoke(name, value);
        }

        /// <summary>
        /// Raises the event triggered event.
        /// </summary>
        internal void RaiseEventTriggered(string eventName, SerializableDictionary<string, string> parameters) {
            OnEventTriggered?.Invoke(eventName, parameters);
        }
    }

    /// <summary>
    /// Represents a saved state for nested conversations.
    /// </summary>
    public class ConversationStackFrame {
        /// <summary>
        /// The conversation ID to return to.
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// The node ID to return to.
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// The predicate registry at the time of push.
        /// </summary>
        public PredicateRegistry PredicateRegistry { get; set; }
    }
}