namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for all evaluatable conditions in the conversation system.
    /// Conditions determine branching, choice visibility, and flow control.
    /// </summary>
    public interface ICondition {
        /// <summary>
        /// Evaluates the condition against the provided context.
        /// </summary>
        /// <param name="context">The evaluation context containing variable providers and predicates.</param>
        /// <returns>True if the condition is met, false otherwise.</returns>
        bool Evaluate(IConditionContext context);
    }
}