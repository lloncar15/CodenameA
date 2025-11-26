using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Executes conversation commands from CommandData.
    /// Handles both synchronous and asynchronous execution.
    /// </summary>
    public class CommandExecutor {
        private readonly ICommandFactory _factory;

        /// <summary>
        /// Creates a new CommandExecutor.
        /// </summary>
        /// <param name="factory">The command factory to use. Uses default if null.</param>
        public CommandExecutor(ICommandFactory factory = null) {
            _factory = factory ?? CommandFactory.Default;
        }

        /// <summary>
        /// Executes a single command synchronously.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>True if executed successfully, false otherwise.</returns>
        public bool Execute(CommandData data, ICommandContext context) {
            if (data == null || context == null) {
                return false;
            }

            IConversationCommand command = _factory.CreateCommand(data);
            if (command == null) {
                return false;
            }

            if (!command.Validate(out string error)) {
                Debug.LogWarning($"CommandExecutor: Command validation failed: {error}");
                return false;
            }

            try {
                command.Execute(context);
                return true;
            }
            catch (System.Exception ex) {
                Debug.LogError($"CommandExecutor: Error executing command '{data.CommandType}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes a single command asynchronously.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>True if executed successfully, false otherwise.</returns>
        public async Task<bool> ExecuteAsync(CommandData data, ICommandContext context) {
            if (data == null || context == null) {
                return false;
            }

            IConversationCommand command = _factory.CreateCommand(data);
            if (command == null) {
                return false;
            }

            if (!command.Validate(out string error)) {
                Debug.LogWarning($"CommandExecutor: Command validation failed: {error}");
                return false;
            }

            try {
                await command.ExecuteAsync(context);
                return true;
            }
            catch (System.Exception ex) {
                Debug.LogError($"CommandExecutor: Error executing command '{data.CommandType}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes multiple commands synchronously in sequence.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Number of commands executed successfully.</returns>
        public int ExecuteAll(IEnumerable<CommandData> commands, ICommandContext context) {
            if (commands == null || context == null) {
                return 0;
            }

            int successCount = 0;
            foreach (CommandData data in commands) {
                if (Execute(data, context)) {
                    successCount++;
                }
            }

            return successCount;
        }

        /// <summary>
        /// Executes multiple commands asynchronously in sequence.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Number of commands executed successfully.</returns>
        public async Task<int> ExecuteAllAsync(IEnumerable<CommandData> commands, ICommandContext context) {
            if (commands == null || context == null) {
                return 0;
            }

            int successCount = 0;
            foreach (CommandData data in commands) {
                if (await ExecuteAsync(data, context)) {
                    successCount++;
                }
            }

            return successCount;
        }

        /// <summary>
        /// Executes multiple commands asynchronously in parallel.
        /// Note: Use with caution - commands may have dependencies.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Number of commands executed successfully.</returns>
        public async Task<int> ExecuteAllParallelAsync(IList<CommandData> commands, ICommandContext context) {
            if (commands == null || context == null || commands.Count == 0) {
                return 0;
            }

            List<Task<bool>> tasks = new();
            foreach (CommandData data in commands) {
                tasks.Add(ExecuteAsync(data, context));
            }

            bool[] results = await Task.WhenAll(tasks);
            
            int successCount = 0;
            foreach (bool result in results) {
                if (result) {
                    successCount++;
                }
            }

            return successCount;
        }
    }
}