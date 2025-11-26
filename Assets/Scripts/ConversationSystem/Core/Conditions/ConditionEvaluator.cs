namespace GimGim.ConversationSystem {
    /// <summary>
    /// Utility class for evaluating conditions.
    /// Provides convenient methods for condition evaluation.
    /// </summary>
    public static class ConditionEvaluator {
        /// <summary>
        /// Evaluates a ConditionData against a context.
        /// </summary>
        /// <param name="data">The condition data to evaluate.</param>
        /// <param name="context">The evaluation context.</param>
        /// <param name="factory">Optional condition factory (uses default if null).</param>
        /// <returns>True if condition is met, false otherwise. Returns true if data is null.</returns>
        public static bool Evaluate(ConditionData data, IConditionContext context, IConditionFactory factory = null) {
            if (data == null) {
                return true; // No condition means always pass
            }

            factory ??= ConditionFactory.Default;
            ICondition condition = factory.CreateCondition(data);

            if (condition == null) {
                UnityEngine.Debug.LogWarning("ConditionEvaluator: Failed to create condition from data.");
                return false;
            }

            return condition.Evaluate(context);
        }

        /// <summary>
        /// Evaluates an ICondition against a context.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="context">The evaluation context.</param>
        /// <returns>True if condition is met, false otherwise. Returns true if condition is null.</returns>
        public static bool Evaluate(ICondition condition, IConditionContext context) {
            if (condition == null) {
                return true; // No condition means always pass
            }

            return condition.Evaluate(context);
        }

        /// <summary>
        /// Evaluates multiple conditions with AND logic.
        /// </summary>
        /// <param name="conditions">The conditions to evaluate.</param>
        /// <param name="context">The evaluation context.</param>
        /// <returns>True if all conditions pass, false otherwise.</returns>
        public static bool EvaluateAll(System.Collections.Generic.IEnumerable<ConditionData> conditions, IConditionContext context) {
            if (conditions == null) {
                return true;
            }

            foreach (ConditionData data in conditions) {
                if (!Evaluate(data, context)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Evaluates multiple conditions with OR logic.
        /// </summary>
        /// <param name="conditions">The conditions to evaluate.</param>
        /// <param name="context">The evaluation context.</param>
        /// <returns>True if any condition passes, false otherwise.</returns>
        public static bool EvaluateAny(System.Collections.Generic.IEnumerable<ConditionData> conditions, IConditionContext context) {
            if (conditions == null) {
                return true;
            }

            foreach (ConditionData data in conditions) {
                if (Evaluate(data, context)) {
                    return true;
                }
            }

            return false;
        }
    }
}