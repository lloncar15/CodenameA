// Assets/Scripts/ConversationSystem/Core/Commands/CommandFactory.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Factory for creating command instances from CommandData.
    /// Supports registration of custom command types.
    /// </summary>
    public class CommandFactory : ICommandFactory {
        private readonly Dictionary<string, Func<CommandData, IConversationCommand>> _creators;

        /// <summary>
        /// Default factory instance with built-in commands registered.
        /// </summary>
        public static CommandFactory Default { get; } = CreateDefault();

        /// <summary>
        /// Creates a new CommandFactory.
        /// </summary>
        public CommandFactory() {
            _creators = new Dictionary<string, Func<CommandData, IConversationCommand>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a factory with built-in commands pre-registered.
        /// </summary>
        /// <returns>A new CommandFactory with default commands.</returns>
        public static CommandFactory CreateDefault() {
            CommandFactory factory = new();
            factory.RegisterBuiltInCommands();
            return factory;
        }

        /// <summary>
        /// Registers all built-in command types.
        /// </summary>
        private void RegisterBuiltInCommands() {
            Register("SetVariable", SetVariableCommand.FromData);
            Register("TriggerEvent", TriggerEventCommand.FromData);
            Register("Log", LogCommand.FromData);
            Register("Delay", DelayCommand.FromData);
        }

        /// <summary>
        /// Registers a command type with a creator function.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="creator">Function that creates command from data.</param>
        /// <returns>True if registered, false if type already exists.</returns>
        public bool Register(string commandType, Func<CommandData, IConversationCommand> creator) {
            if (string.IsNullOrEmpty(commandType)) {
                Debug.LogWarning("CommandFactory: Cannot register command with null or empty type.");
                return false;
            }

            if (creator == null) {
                Debug.LogWarning($"CommandFactory: Cannot register null creator for '{commandType}'.");
                return false;
            }

            if (_creators.TryAdd(commandType, creator)) return true;
            
            Debug.LogWarning($"CommandFactory: Command type '{commandType}' already registered.");
            return false;

        }

        /// <summary>
        /// Registers a synchronous custom command.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>True if registered successfully.</returns>
        public bool RegisterCustom(string commandType, Action<ICommandContext, CommandData> action) {
            return Register(commandType, data => new CustomCommand(commandType, action).WithData(data));
        }

        /// <summary>
        /// Registers an asynchronous custom command.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="asyncAction">The async action to execute.</param>
        /// <returns>True if registered successfully.</returns>
        public bool RegisterCustomAsync(string commandType, Func<ICommandContext, CommandData, System.Threading.Tasks.Task> asyncAction) {
            return Register(commandType, data => new CustomCommand(commandType, asyncAction).WithData(data));
        }

        /// <summary>
        /// Replaces an existing command registration.
        /// </summary>
        /// <param name="commandType">The command type identifier.</param>
        /// <param name="creator">Function that creates command from data.</param>
        public void Replace(string commandType, Func<CommandData, IConversationCommand> creator) {
            if (string.IsNullOrEmpty(commandType) || creator == null) {
                return;
            }

            _creators[commandType] = creator;
        }

        /// <summary>
        /// Unregisters a command type.
        /// </summary>
        /// <param name="commandType">The command type to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        public bool Unregister(string commandType) {
            return _creators.Remove(commandType);
        }

        /// <inheritdoc />
        public IConversationCommand CreateCommand(CommandData data) {
            if (data == null) {
                return null;
            }

            if (string.IsNullOrEmpty(data.CommandType)) {
                Debug.LogWarning("CommandFactory: CommandData has null or empty CommandType.");
                return null;
            }

            if (!_creators.TryGetValue(data.CommandType, out Func<CommandData, IConversationCommand> creator)) {
                Debug.LogWarning($"CommandFactory: Unknown command type '{data.CommandType}'.");
                return null;
            }

            try {
                return creator(data);
            }
            catch (Exception ex) {
                Debug.LogError($"CommandFactory: Error creating command '{data.CommandType}': {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc />
        public bool IsRegistered(string commandType) {
            return !string.IsNullOrEmpty(commandType) && _creators.ContainsKey(commandType);
        }

        /// <summary>
        /// Gets all registered command types.
        /// </summary>
        /// <returns>Enumerable of command type names.</returns>
        public IEnumerable<string> GetRegisteredTypes() {
            return _creators.Keys;
        }
    }
}