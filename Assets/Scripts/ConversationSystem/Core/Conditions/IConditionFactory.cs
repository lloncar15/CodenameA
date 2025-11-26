namespace GimGim.ConversationSystem {
    /// <summary>
    /// Factory interface for creating ICondition instances from ConditionData.
    /// </summary>
    public interface IConditionFactory {
        /// <summary>
        /// Creates an ICondition from ConditionData.
        /// </summary>
        /// <param name="data">The condition data to convert.</param>
        /// <returns>An ICondition instance, or null if data is invalid.</returns>
        ICondition CreateCondition(ConditionData data);
    }
}