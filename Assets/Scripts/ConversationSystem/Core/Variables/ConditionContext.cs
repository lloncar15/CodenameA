using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Implementation of IConditionContext that combines variable providers and predicate registry.
    /// </summary>
    public class ConditionContext : IConditionContext {
        private readonly IConversationVariableProvider _variableProvider;
        private readonly PredicateRegistry _predicateRegistry;

        /// <summary>
        /// Creates a new condition context.
        /// </summary>
        /// <param name="variableProvider">The variable provider to use.</param>
        /// <param name="predicateRegistry">The predicate registry to use.</param>
        public ConditionContext(IConversationVariableProvider variableProvider, PredicateRegistry predicateRegistry = null) {
            _variableProvider = variableProvider ?? throw new ArgumentNullException(nameof(variableProvider));
            _predicateRegistry = predicateRegistry ?? new PredicateRegistry();
        }

        /// <inheritdoc />
        public bool TryGetBool(string variableName, out bool value) {
            return _variableProvider.TryGetBool(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetInt(string variableName, out int value) {
            return _variableProvider.TryGetInt(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetFloat(string variableName, out float value) {
            return _variableProvider.TryGetFloat(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetString(string variableName, out string value) {
            return _variableProvider.TryGetString(variableName, out value);
        }

        /// <inheritdoc />
        public bool TryGetPredicate(string predicateName, out Func<SerializableDictionary<string, string>, bool> predicate) {
            return _predicateRegistry.TryGet(predicateName, out predicate);
        }
    }
}