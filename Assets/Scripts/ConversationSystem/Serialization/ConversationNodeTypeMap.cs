using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Maps node type strings to their concrete types for deserialization.
    /// </summary>
    public static class ConversationNodeTypeMap {
        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) {
            { "Text", typeof(TextNode) },
            { "Choice", typeof(ChoiceNode) },
            { "Branch", typeof(BranchNode) },
            { "Event", typeof(EventNode) },
            { "Random", typeof(RandomNode) },
            { "Wait", typeof(WaitNode) },
            { "Jump", typeof(JumpNode) }
        };

        private static readonly Dictionary<Type, string> ReverseMap;

        static ConversationNodeTypeMap() {
            ReverseMap = new Dictionary<Type, string>();
            foreach (KeyValuePair<string, Type> kvp in TypeMap) {
                ReverseMap[kvp.Value] = kvp.Key;
            }
        }

        /// <summary>
        /// Gets the concrete type for a node type string.
        /// </summary>
        /// <param name="typeName">The type name (e.g., "Text", "Choice").</param>
        /// <returns>The concrete type, or null if not found.</returns>
        public static Type GetNodeType(string typeName) {
            return string.IsNullOrEmpty(typeName) ? null : TypeMap.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Gets the type string for a concrete node type.
        /// </summary>
        /// <param name="type">The concrete type.</param>
        /// <returns>The type string, or null if not found.</returns>
        public static string GetTypeName(Type type) {
            return type == null ? null : ReverseMap.GetValueOrDefault(type);
        }

        /// <summary>
        /// Gets the type string for a node instance.
        /// </summary>
        /// <param name="node">The node instance.</param>
        /// <returns>The type string.</returns>
        public static string GetTypeName(ConversationNode node) {
            return node?.NodeType.ToString();
        }

        /// <summary>
        /// Registers a custom node type.
        /// </summary>
        /// <param name="typeName">The type name string.</param>
        /// <param name="type">The concrete type.</param>
        /// <returns>True if registered, false if name already exists.</returns>
        public static bool RegisterType(string typeName, Type type) {
            if (string.IsNullOrEmpty(typeName) || type == null) {
                return false;
            }

            if (!TypeMap.TryAdd(typeName, type)) {
                return false;
            }

            ReverseMap[type] = typeName;
            return true;
        }

        /// <summary>
        /// Gets all registered type names.
        /// </summary>
        /// <returns>Enumerable of type names.</returns>
        public static IEnumerable<string> GetAllTypeNames() {
            return TypeMap.Keys;
        }
    }
}