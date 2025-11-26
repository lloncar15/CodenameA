namespace GimGim.ConversationSystem {
    /// <summary>
    /// A condition that always returns a constant value.
    /// Useful for testing and default cases.
    /// </summary>
    public class ConstantCondition : ICondition {
        private readonly bool _value;

        /// <summary>
        /// A condition that always returns true.
        /// </summary>
        public static readonly ConstantCondition True = new ConstantCondition(true);

        /// <summary>
        /// A condition that always returns false.
        /// </summary>
        public static readonly ConstantCondition False = new ConstantCondition(false);

        /// <summary>
        /// Creates a constant condition.
        /// </summary>
        /// <param name="value">The constant value to return.</param>
        public ConstantCondition(bool value) {
            _value = value;
        }

        /// <inheritdoc />
        public bool Evaluate(IConditionContext context) {
            return _value;
        }

        public override string ToString() {
            return _value ? "TRUE" : "FALSE";
        }
    }
}