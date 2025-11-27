using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes BranchNode instances.
    /// </summary>
    public class BranchNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Branch;

        public Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not BranchNode branchNode) {
                Debug.LogError("BranchNodeProcessor: Invalid node type.");
                return Task.FromResult<string>(null);
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, branchNode.Id);

            // Evaluate branches in priority order
            string nextNodeId = EvaluateBranches(branchNode, context);

            return Task.FromResult(nextNodeId);
        }

        /// <summary>
        /// Evaluates branches and returns the first matching branch's target.
        /// </summary>
        private string EvaluateBranches(BranchNode node, ConversationContext context) {
            if (node.Branches == null || node.Branches.Count == 0) {
                return node.DefaultNodeId;
            }

            // Sort by priority (higher first)
            List<BranchOption> sortedBranches = new(node.Branches);
            sortedBranches.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            foreach (BranchOption branch in sortedBranches) {
                if (branch.Condition == null) {
                    // No condition = always true
                    return branch.NextNodeId;
                }

                bool result = EvaluateCondition(branch.Condition, context);
                if (result) {
                    return branch.NextNodeId;
                }
            }

            // No branch matched, use default
            return node.DefaultNodeId;
        }

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        private bool EvaluateCondition(ConditionData condition, ConversationContext context) {
            if (context.ConditionContext == null) {
                return true;
            }

            ICondition cond = context.ConditionFactory.CreateCondition(condition);
            return cond?.Evaluate(context.ConditionContext) ?? true;
        }
    }
}