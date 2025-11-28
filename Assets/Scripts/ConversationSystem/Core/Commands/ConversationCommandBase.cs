using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Abstract base class for conversation commands.
    /// Provides default implementations for common functionality.
    /// </summary>
    public abstract class ConversationCommandBase : IConversationCommand {
        /// <inheritdoc />
        public abstract string CommandType { get; }

        /// <summary>
        /// The original command data used to create this command.
        /// </summary>
        protected CommandData SourceData { get; set; }

        /// <inheritdoc />
        public abstract void Execute(ICommandContext context);

        /// <inheritdoc />
        public virtual Task ExecuteAsync(ICommandContext context) {
            Execute(context);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual bool Validate(out string errorMessage) {
            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Gets a parameter from the source data as a string.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The parameter value or default.</returns>
        protected string GetParameter(string key, string defaultValue = "") {
            return SourceData?.GetString(key, defaultValue) ?? defaultValue;
        }

        /// <summary>
        /// Gets a parameter from the source data as an integer.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The parameter value or default.</returns>
        protected int GetParameterInt(string key, int defaultValue = 0) {
            return SourceData?.GetInt(key, defaultValue) ?? defaultValue;
        }

        /// <summary>
        /// Gets a parameter from the source data as a float.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The parameter value or default.</returns>
        protected float GetParameterFloat(string key, float defaultValue = 0f) {
            return SourceData?.GetFloat(key, defaultValue) ?? defaultValue;
        }

        /// <summary>
        /// Gets a parameter from the source data as a boolean.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">Default value if not found.</param>
        /// <returns>The parameter value or default.</returns>
        protected bool GetParameterBool(string key, bool defaultValue = false) {
            return SourceData?.GetBool(key, defaultValue) ?? defaultValue;
        }
    }
}