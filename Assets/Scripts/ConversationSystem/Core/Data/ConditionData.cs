// Assets/Scripts/ConversationSystem/Core/Data/ConditionData.cs
using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Serializable condition data for JSON storage.
    /// Supports simple comparisons, composite (AND/OR) conditions, and custom predicates.
    /// </summary>
    [Serializable]
    public class ConditionData {
        /// <summary>
        /// The type of condition: "simple", "composite", or "predicate".
        /// </summary>
        public string Type { get; set; }

        // Simple condition fields
        /// <summary>
        /// The variable name to check (for simple conditions).
        /// </summary>
        public string Variable { get; set; }

        /// <summary>
        /// The comparison operator (for simple conditions).
        /// </summary>
        public ComparisonOperator Operator { get; set; }

        /// <summary>
        /// The value to compare against (stored as string, parsed at runtime).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The expected type of the value: "bool", "int", "float", "string".
        /// </summary>
        public string ValueType { get; set; } = "bool";

        // Composite condition fields
        /// <summary>
        /// The logical operator for combining sub-conditions (for composite conditions).
        /// </summary>
        public LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        /// Sub-conditions to combine (for composite conditions).
        /// </summary>
        public List<ConditionData> SubConditions { get; set; }

        // Predicate condition fields
        /// <summary>
        /// The registered name of the custom predicate (for predicate conditions).
        /// </summary>
        public string PredicateName { get; set; }

        /// <summary>
        /// Optional parameters to pass to the predicate.
        /// </summary>
        public SerializableDictionary<string, string> PredicateParameters { get; set; }

        /// <summary>
        /// If true, inverts the result of the condition.
        /// </summary>
        public bool Negate { get; set; } = false;

        public ConditionData() {
            SubConditions = new List<ConditionData>();
            PredicateParameters = new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// Creates a simple boolean condition.
        /// </summary>
        public static ConditionData SimpleBool(string variable, bool expectedValue, bool negate = false) {
            return new ConditionData {
                Type = "simple",
                Variable = variable,
                Operator = ComparisonOperator.Equals,
                Value = expectedValue.ToString(),
                ValueType = "bool",
                Negate = negate
            };
        }

        /// <summary>
        /// Creates a simple integer comparison condition.
        /// </summary>
        public static ConditionData SimpleInt(string variable, ComparisonOperator op, int value, bool negate = false) {
            return new ConditionData {
                Type = "simple",
                Variable = variable,
                Operator = op,
                Value = value.ToString(),
                ValueType = "int",
                Negate = negate
            };
        }

        /// <summary>
        /// Creates a composite AND condition.
        /// </summary>
        public static ConditionData And(params ConditionData[] conditions) {
            return new ConditionData {
                Type = "composite",
                LogicalOperator = LogicalOperator.And,
                SubConditions = new List<ConditionData>(conditions)
            };
        }

        /// <summary>
        /// Creates a composite OR condition.
        /// </summary>
        public static ConditionData Or(params ConditionData[] conditions) {
            return new ConditionData {
                Type = "composite",
                LogicalOperator = LogicalOperator.Or,
                SubConditions = new List<ConditionData>(conditions)
            };
        }

        /// <summary>
        /// Creates a custom predicate condition.
        /// </summary>
        public static ConditionData Predicate(string predicateName, SerializableDictionary<string, string> parameters = null, bool negate = false) {
            return new ConditionData {
                Type = "predicate",
                PredicateName = predicateName,
                PredicateParameters = parameters ?? new SerializableDictionary<string, string>(),
                Negate = negate
            };
        }
    }
}