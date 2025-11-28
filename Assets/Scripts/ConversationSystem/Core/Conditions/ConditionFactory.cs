using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Default implementation of IConditionFactory.
    /// Creates appropriate ICondition instances based on ConditionData type.
    /// </summary>
    public class ConditionFactory : IConditionFactory {
        /// <summary>
        /// Singleton instance for convenience.
        /// </summary>
        public static ConditionFactory Default { get; } = new ConditionFactory();

        /// <inheritdoc />
        public ICondition CreateCondition(ConditionData data) {
            if (data == null) {
                return null;
            }

            string type = data.Type?.ToLower() ?? "simple";

            return type switch {
                "simple" => CreateSimpleCondition(data),
                "composite" => CreateCompositeCondition(data),
                "predicate" => CreatePredicateCondition(data),
                _ => HandleUnknownType(data)
            };
        }

        /// <summary>
        /// Creates a SimpleCondition from data.
        /// </summary>
        private ICondition CreateSimpleCondition(ConditionData data) {
            if (string.IsNullOrEmpty(data.Variable)) {
                Debug.LogWarning("ConditionFactory: Simple condition missing variable name.");
                return null;
            }

            return SimpleCondition.FromData(data);
        }

        /// <summary>
        /// Creates a CompositeCondition from data.
        /// </summary>
        private ICondition CreateCompositeCondition(ConditionData data) {
            if (data.SubConditions == null || data.SubConditions.Count == 0) {
                Debug.LogWarning("ConditionFactory: Composite condition has no sub-conditions.");
                return null;
            }

            return CompositeCondition.FromData(data, this);
        }

        /// <summary>
        /// Creates a PredicateCondition from data.
        /// </summary>
        private ICondition CreatePredicateCondition(ConditionData data) {
            if (string.IsNullOrEmpty(data.PredicateName)) {
                Debug.LogWarning("ConditionFactory: Predicate condition missing predicate name.");
                return null;
            }

            return PredicateCondition.FromData(data);
        }

        /// <summary>
        /// Handles unknown condition types.
        /// </summary>
        private ICondition HandleUnknownType(ConditionData data) {
            Debug.LogWarning($"ConditionFactory: Unknown condition type '{data.Type}'.");
            return null;
        }
    }
}