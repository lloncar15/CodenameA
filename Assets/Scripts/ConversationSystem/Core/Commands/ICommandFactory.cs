namespace GimGim.ConversationSystem {
    /// <summary>
    /// Factory interface for creating IConversationCommand instances from CommandData.
    /// </summary>
    public interface ICommandFactory {
        /// <summary>
        /// Creates a command from CommandData.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>A command instance, or null if the type is unknown.</returns>
        IConversationCommand CreateCommand(CommandData data);

        /// <summary>
        /// Checks if a command type is registered.
        /// </summary>
        /// <param name="commandType">The command type to check.</param>
        /// <returns>True if registered, false otherwise.</returns>
        bool IsRegistered(string commandType);
    }
}