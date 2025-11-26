using System;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// A condition that performs a simple comparison between a variable and a value.
    /// Supports bool, int, float, and string comparisons.
    /// </summary>
    public class SimpleCondition : ICondition {
        private readonly string _variableName;
        private readonly ComparisonOperator _operator;
        private readonly string _value;
        private readonly string _valueType;
        private readonly bool _negate;

        /// <summary>
        /// Creates a new simple condition.
        /// </summary>
        /// <param name="variableName">The name of the variable to check.</param>
        /// <param name="comparisonOperator">The comparison operator to use.</param>
        /// <param name="value">The value to compare against (as string).</param>
        /// <param name="valueType">The type of value: "bool", "int", "float", or "string".</param>
        /// <param name="negate">If true, inverts the result.</param>
        public SimpleCondition(string variableName, ComparisonOperator comparisonOperator, string value, string valueType = "bool", bool negate = false) {
            _variableName = variableName;
            _operator = comparisonOperator;
            _value = value;
            _valueType = valueType?.ToLower() ?? "bool";
            _negate = negate;
        }

        /// <summary>
        /// Creates a SimpleCondition from ConditionData.
        /// </summary>
        /// <param name="data">The condition data to convert.</param>
        /// <returns>A new SimpleCondition instance.</returns>
        public static SimpleCondition FromData(ConditionData data) {
            return new SimpleCondition(
                data.Variable,
                data.Operator,
                data.Value,
                data.ValueType,
                data.Negate
            );
        }

        /// <inheritdoc />
        public bool Evaluate(IConditionContext context) {
            bool result = EvaluateInternal(context);
            return _negate ? !result : result;
        }

        /// <summary>
        /// Internal evaluation logic without negation.
        /// </summary>
        private bool EvaluateInternal(IConditionContext context) {
            switch (_valueType) {
                case "bool":
                    return EvaluateBool(context);
                case "int":
                    return EvaluateInt(context);
                case "float":
                    return EvaluateFloat(context);
                case "string":
                    return EvaluateString(context);
                default:
                    UnityEngine.Debug.LogWarning($"SimpleCondition: Unknown value type '{_valueType}', defaulting to bool.");
                    return EvaluateBool(context);
            }
        }

        /// <summary>
        /// Evaluates a boolean comparison.
        /// </summary>
        private bool EvaluateBool(IConditionContext context) {
            if (!context.TryGetBool(_variableName, out bool variableValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Boolean variable '{_variableName}' not found, defaulting to false.");
                variableValue = false;
            }

            if (!bool.TryParse(_value, out bool targetValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Could not parse '{_value}' as bool, defaulting to false.");
                targetValue = false;
            }

            return _operator switch {
                ComparisonOperator.Equals => variableValue == targetValue,
                ComparisonOperator.NotEquals => variableValue != targetValue,
                _ => throw new InvalidOperationException($"Operator '{_operator}' is not valid for boolean comparison.")
            };
        }

        /// <summary>
        /// Evaluates an integer comparison.
        /// </summary>
        private bool EvaluateInt(IConditionContext context) {
            if (!context.TryGetInt(_variableName, out int variableValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Integer variable '{_variableName}' not found, defaulting to 0.");
                variableValue = 0;
            }

            if (!int.TryParse(_value, out int targetValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Could not parse '{_value}' as int, defaulting to 0.");
                targetValue = 0;
            }

            return _operator switch {
                ComparisonOperator.Equals => variableValue == targetValue,
                ComparisonOperator.NotEquals => variableValue != targetValue,
                ComparisonOperator.GreaterThan => variableValue > targetValue,
                ComparisonOperator.GreaterThanOrEquals => variableValue >= targetValue,
                ComparisonOperator.LessThan => variableValue < targetValue,
                ComparisonOperator.LessThanOrEquals => variableValue <= targetValue,
                _ => throw new InvalidOperationException($"Unknown operator '{_operator}'.")
            };
        }

        /// <summary>
        /// Evaluates a float comparison.
        /// </summary>
        private bool EvaluateFloat(IConditionContext context) {
            if (!context.TryGetFloat(_variableName, out float variableValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Float variable '{_variableName}' not found, defaulting to 0.");
                variableValue = 0f;
            }

            if (!float.TryParse(_value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float targetValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: Could not parse '{_value}' as float, defaulting to 0.");
                targetValue = 0f;
            }

            // Use small epsilon for float comparison
            const float epsilon = 0.0001f;

            return _operator switch {
                ComparisonOperator.Equals => Math.Abs(variableValue - targetValue) < epsilon,
                ComparisonOperator.NotEquals => Math.Abs(variableValue - targetValue) >= epsilon,
                ComparisonOperator.GreaterThan => variableValue > targetValue,
                ComparisonOperator.GreaterThanOrEquals => variableValue >= targetValue - epsilon,
                ComparisonOperator.LessThan => variableValue < targetValue,
                ComparisonOperator.LessThanOrEquals => variableValue <= targetValue + epsilon,
                _ => throw new InvalidOperationException($"Unknown operator '{_operator}'.")
            };
        }

        /// <summary>
        /// Evaluates a string comparison.
        /// </summary>
        private bool EvaluateString(IConditionContext context) {
            if (!context.TryGetString(_variableName, out string variableValue)) {
                UnityEngine.Debug.LogWarning($"SimpleCondition: String variable '{_variableName}' not found, defaulting to empty string.");
                variableValue = string.Empty;
            }

            string targetValue = _value ?? string.Empty;
            int comparison = string.Compare(variableValue, targetValue, StringComparison.Ordinal);

            return _operator switch {
                ComparisonOperator.Equals => comparison == 0,
                ComparisonOperator.NotEquals => comparison != 0,
                ComparisonOperator.GreaterThan => comparison > 0,
                ComparisonOperator.GreaterThanOrEquals => comparison >= 0,
                ComparisonOperator.LessThan => comparison < 0,
                ComparisonOperator.LessThanOrEquals => comparison <= 0,
                _ => throw new InvalidOperationException($"Unknown operator '{_operator}'.")
            };
        }

        public override string ToString() {
            string negateStr = _negate ? "NOT " : "";
            return $"{negateStr}({_variableName} {_operator} {_value})";
        }
    }
}