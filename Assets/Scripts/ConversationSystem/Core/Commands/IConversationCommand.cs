using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for all executable commands in the conversation system.
    /// Commands encapsulate actions that can be triggered during conversations.
    /// </summary>
    public interface IConversationCommand {
        /// <summary>
        /// Gets the unique type identifier for this command.
        /// Used for serialization and factory registration.
        /// </summary>
        string CommandType { get; }

        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        /// <param name="context">The execution context providing access to conversation state.</param>
        void Execute(ICommandContext context);

        /// <summary>
        /// Executes the command asynchronously.
        /// Override for commands that need to wait for external operations.
        /// </summary>
        /// <param name="context">The execution context providing access to conversation state.</param>
        /// <returns>A task representing the async operation.</returns>
        Task ExecuteAsync(ICommandContext context);

        /// <summary>
        /// Validates the command configuration.
        /// </summary>
        /// <param name="errorMessage">Error description if validation fails.</param>
        /// <returns>True if valid, false otherwise.</returns>
        bool Validate(out string errorMessage);
    }
}