using System;
using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A wrapper command that executes a custom action.
    /// Allows registering arbitrary code as commands.
    /// </summary>
    public class CustomCommand : ConversationCommandBase {
        private readonly string _commandType;
        private readonly Action<ICommandContext, CommandData> _syncAction;
        private readonly Func<ICommandContext, CommandData, Task> _asyncAction;

        public override string CommandType => _commandType;

        /// <summary>
        /// Creates a synchronous custom command.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="action">The action to execute.</param>
        public CustomCommand(string commandType, Action<ICommandContext, CommandData> action) {
            _commandType = commandType;
            _syncAction = action;
        }

        /// <summary>
        /// Creates an asynchronous custom command.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="asyncAction">The async action to execute.</param>
        public CustomCommand(string commandType, Func<ICommandContext, CommandData, Task> asyncAction) {
            _commandType = commandType;
            _asyncAction = asyncAction;
        }

        /// <summary>
        /// Sets the source data for parameter access.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>This command for chaining.</returns>
        public CustomCommand WithData(CommandData data) {
            SourceData = data;
            return this;
        }

        /// <inheritdoc />
        public override void Execute(ICommandContext context) {
            _syncAction?.Invoke(context, SourceData);
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(ICommandContext context) {
            if (_asyncAction != null) {
                await _asyncAction(context, SourceData);
            }
            else {
                Execute(context);
            }
        }

        public override string ToString() {
            return $"CustomCommand({_commandType})";
        }
    }
}