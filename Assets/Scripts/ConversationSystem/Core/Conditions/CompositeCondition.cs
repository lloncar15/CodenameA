using System.Collections.Generic;
using System.Linq;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A condition that combines multiple sub-conditions using AND or OR logic.
    /// </summary>
    public class CompositeCondition : ICondition {
        private readonly LogicalOperator _operator;
        private readonly List<ICondition> _subConditions;
        private readonly bool _negate;

        /// <summary>
        /// Creates a new composite condition.
        /// </summary>
        /// <param name="logicalOperator">The logical operator to combine conditions.</param>
        /// <param name="subConditions">The conditions to combine.</param>
        /// <param name="negate">If true, inverts the final result.</param>
        public CompositeCondition(LogicalOperator logicalOperator, IEnumerable<ICondition> subConditions, bool negate = false) {
            _operator = logicalOperator;
            _subConditions = subConditions?.ToList() ?? new List<ICondition>();
            _negate = negate;
        }

        /// <summary>
        /// Creates a composite condition from ConditionData.
        /// Recursively converts sub-conditions.
        /// </summary>
        /// <param name="data">The condition data to convert.</param>
        /// <param name="factory">Factory to create sub-conditions.</param>
        /// <returns>A new CompositeCondition instance.</returns>
        public static CompositeCondition FromData(ConditionData data, IConditionFactory factory) {
            var subConditions = data.SubConditions?
                .Select(factory.CreateCondition)
                .Where(c => c != null)
                .ToList() ?? new List<ICondition>();

            return new CompositeCondition(data.LogicalOperator, subConditions, data.Negate);
        }

        /// <inheritdoc />
        public bool Evaluate(IConditionContext context) {
            if (_subConditions.Count == 0) {
                return !_negate; // Empty AND is true, empty OR is false, then negate
            }

            bool result = _operator switch {
                LogicalOperator.And => EvaluateAnd(context),
                LogicalOperator.Or => EvaluateOr(context),
                _ => false
            };

            return _negate ? !result : result;
        }

        /// <summary>
        /// Evaluates using AND logic (all must be true).
        /// Short-circuits on first false.
        /// </summary>
        private bool EvaluateAnd(IConditionContext context) {
            foreach (ICondition condition in _subConditions) {
                if (!condition.Evaluate(context)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Evaluates using OR logic (at least one must be true).
        /// Short-circuits on first true.
        /// </summary>
        private bool EvaluateOr(IConditionContext context) {
            foreach (ICondition condition in _subConditions) {
                if (condition.Evaluate(context)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates an AND composite condition.
        /// </summary>
        /// <param name="conditions">The conditions to combine.</param>
        /// <returns>A new CompositeCondition with AND logic.</returns>
        public static CompositeCondition And(params ICondition[] conditions) {
            return new CompositeCondition(LogicalOperator.And, conditions);
        }

        /// <summary>
        /// Creates an OR composite condition.
        /// </summary>
        /// <param name="conditions">The conditions to combine.</param>
        /// <returns>A new CompositeCondition with OR logic.</returns>
        public static CompositeCondition Or(params ICondition[] conditions) {
            return new CompositeCondition(LogicalOperator.Or, conditions);
        }

        public override string ToString() {
            string op = _operator == LogicalOperator.And ? " AND " : " OR ";
            string combined = string.Join(op, _subConditions.Select(c => c.ToString()));
            string negateStr = _negate ? "NOT " : "";
            return $"{negateStr}({combined})";
        }
    }
}