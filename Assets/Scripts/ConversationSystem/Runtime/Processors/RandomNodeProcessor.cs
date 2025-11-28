using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes RandomNode instances.
    /// </summary>
    public class RandomNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Random;

        public Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not RandomNode randomNode) {
                Debug.LogError("RandomNodeProcessor: Invalid node type.");
                return Task.FromResult<string>(null);
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, randomNode.Id);

            // Select random option
            string selectedNodeId = SelectRandomOption(randomNode, context);

            return Task.FromResult(selectedNodeId);
        }

        /// <summary>
        /// Selects a random option based on weights.
        /// </summary>
        private string SelectRandomOption(RandomNode node, ConversationContext context) {
            if (node.Options == null || node.Options.Count == 0) {
                return null;
            }

            // Build list of valid options (respecting avoid repeat)
            List<RandomOption> validOptions = new();
            string lastSelection = null;

            if (node.AvoidRepeat && context.StateManager != null) {
                // Get last selection for this node
                string key = ConversationStateData.MakeNodeKey(context.CurrentConversationId, node.Id);
                // Would need to extend state manager for this
            }

            float totalWeight = 0f;
            foreach (RandomOption option in node.Options) {
                if (node.AvoidRepeat && option.NextNodeId == lastSelection && node.Options.Count > 1) {
                    continue; // Skip last selection if avoiding repeats
                }

                validOptions.Add(option);
                totalWeight += option.Weight;
            }

            if (validOptions.Count == 0) {
                // Fall back to all options if none valid
                validOptions.AddRange(node.Options);
                totalWeight = 0f;
                foreach (RandomOption option in validOptions) {
                    totalWeight += option.Weight;
                }
            }

            // Weighted random selection
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (RandomOption option in validOptions) {
                cumulative += option.Weight;
                if (randomValue <= cumulative) {
                    return option.NextNodeId;
                }
            }

            // Fallback to last option
            return validOptions[^1].NextNodeId;
        }
    }
}