namespace GimGim.ConversationSystem {
    /// <summary>
    /// Command that sets a variable in the conversation state.
    /// Supports bool, int, float, and string types.
    /// </summary>
    public class SetVariableCommand : ConversationCommandBase {
        public override string CommandType => "SetVariable";

        private readonly string _variableName;
        private readonly string _value;
        private readonly string _valueType;
        private readonly VariableOperation _operation;

        /// <summary>
        /// Defines operations that can be performed on variables.
        /// </summary>
        public enum VariableOperation {
            Set,
            Add,
            Subtract,
            Multiply,
            Divide,
            Toggle
        }

        /// <summary>
        /// Creates a new SetVariableCommand.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="value">The value (as string).</param>
        /// <param name="valueType">The type: "bool", "int", "float", "string".</param>
        /// <param name="operation">The operation to perform.</param>
        public SetVariableCommand(string variableName, string value, string valueType = "bool", VariableOperation operation = VariableOperation.Set) {
            _variableName = variableName;
            _value = value;
            _valueType = valueType?.ToLower() ?? "bool";
            _operation = operation;
        }

        /// <summary>
        /// Creates a SetVariableCommand from CommandData.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>A new SetVariableCommand instance.</returns>
        public static SetVariableCommand FromData(CommandData data) {
            string variableName = data.GetString("variable");
            string value = data.GetString("value");
            string valueType = data.GetString("valueType", "bool");
            string operationStr = data.GetString("operation", "Set");

            VariableOperation operation = VariableOperation.Set;
            if (System.Enum.TryParse(operationStr, true, out VariableOperation parsedOp)) {
                operation = parsedOp;
            }

            return new SetVariableCommand(variableName, value, valueType, operation) {
                SourceData = data
            };
        }

        /// <inheritdoc />
        public override void Execute(ICommandContext context) {
            switch (_valueType) {
                case "bool":
                    ExecuteBool(context);
                    break;
                case "int":
                    ExecuteInt(context);
                    break;
                case "float":
                    ExecuteFloat(context);
                    break;
                case "string":
                    ExecuteString(context);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"SetVariableCommand: Unknown value type '{_valueType}'.");
                    break;
            }
        }

        /// <summary>
        /// Executes boolean variable operation.
        /// </summary>
        private void ExecuteBool(ICommandContext context) {
            bool currentValue = context.StateManager.GetBool(_variableName);
            bool targetValue = bool.TryParse(_value, out bool parsed) && parsed;

            bool newValue = _operation switch {
                VariableOperation.Set => targetValue,
                VariableOperation.Toggle => !currentValue,
                _ => targetValue
            };

            context.SetBool(_variableName, newValue);
        }

        /// <summary>
        /// Executes integer variable operation.
        /// </summary>
        private void ExecuteInt(ICommandContext context) {
            int currentValue = context.StateManager.GetInt(_variableName);
            int targetValue = int.TryParse(_value, out int parsed) ? parsed : 0;

            int newValue = _operation switch {
                VariableOperation.Set => targetValue,
                VariableOperation.Add => currentValue + targetValue,
                VariableOperation.Subtract => currentValue - targetValue,
                VariableOperation.Multiply => currentValue * targetValue,
                VariableOperation.Divide => targetValue != 0 ? currentValue / targetValue : currentValue,
                _ => targetValue
            };

            context.SetInt(_variableName, newValue);
        }

        /// <summary>
        /// Executes float variable operation.
        /// </summary>
        private void ExecuteFloat(ICommandContext context) {
            float currentValue = context.StateManager.GetFloat(_variableName);
            float targetValue = float.TryParse(_value, System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture, out float parsed) ? parsed : 0f;

            float newValue = _operation switch {
                VariableOperation.Set => targetValue,
                VariableOperation.Add => currentValue + targetValue,
                VariableOperation.Subtract => currentValue - targetValue,
                VariableOperation.Multiply => currentValue * targetValue,
                VariableOperation.Divide => targetValue != 0 ? currentValue / targetValue : currentValue,
                _ => targetValue
            };

            context.SetFloat(_variableName, newValue);
        }

        /// <summary>
        /// Executes string variable operation.
        /// </summary>
        private void ExecuteString(ICommandContext context) {
            string currentValue = context.StateManager.GetString(_variableName);

            string newValue = _operation switch {
                VariableOperation.Set => _value,
                VariableOperation.Add => currentValue + _value, // Concatenation
                _ => _value
            };

            context.SetString(_variableName, newValue);
        }

        /// <inheritdoc />
        public override bool Validate(out string errorMessage) {
            if (string.IsNullOrEmpty(_variableName)) {
                errorMessage = "SetVariableCommand: Variable name cannot be null or empty.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public override string ToString() {
            return $"SetVariable({_variableName} {_operation} {_value} [{_valueType}])";
        }
    }
}