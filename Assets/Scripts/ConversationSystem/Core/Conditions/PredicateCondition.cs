using System;
using System.Linq;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A condition that evaluates a custom predicate function.
    /// Predicates are registered per-conversation and can access game-specific logic.
    /// </summary>
    public class PredicateCondition : ICondition {
        private readonly string _predicateName;
        private readonly SerializableDictionary<string, string> _parameters;
        private readonly bool _negate;

        /// <summary>
        /// Creates a new predicate condition.
        /// </summary>
        /// <param name="predicateName">The registered name of the predicate.</param>
        /// <param name="parameters">Optional parameters to pass to the predicate.</param>
        /// <param name="negate">If true, inverts the result.</param>
        public PredicateCondition(string predicateName, SerializableDictionary<string, string> parameters = null, bool negate = false) {
            _predicateName = predicateName;
            _parameters = parameters ?? new SerializableDictionary<string, string>();
            _negate = negate;
        }

        /// <summary>
        /// Creates a PredicateCondition from ConditionData.
        /// </summary>
        /// <param name="data">The condition data to convert.</param>
        /// <returns>A new PredicateCondition instance.</returns>
        public static PredicateCondition FromData(ConditionData data) {
            return new PredicateCondition(
                data.PredicateName,
                data.PredicateParameters,
                data.Negate
            );
        }

        /// <inheritdoc />
        public bool Evaluate(IConditionContext context) {
            if (string.IsNullOrEmpty(_predicateName)) {
                UnityEngine.Debug.LogWarning("PredicateCondition: Predicate name is null or empty.");
                return _negate;
            }

            if (!context.TryGetPredicate(_predicateName, out var predicate)) {
                UnityEngine.Debug.LogWarning($"PredicateCondition: Predicate '{_predicateName}' not found.");
                return _negate;
            }

            try {
                bool result = predicate(_parameters);
                return _negate ? !result : result;
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogError($"PredicateCondition: Error evaluating predicate '{_predicateName}': {ex.Message}");
                return _negate;
            }
        }

        public override string ToString() {
            string negateStr = _negate ? "NOT " : "";
            string paramsStr = _parameters.Count > 0 
                ? $"({string.Join(", ", _parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))})" 
                : "";
            return $"{negateStr}Predicate:{_predicateName}{paramsStr}";
        }
    }
}