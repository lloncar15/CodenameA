using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes WaitNode instances.
    /// </summary>
    public class WaitNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Wait;

        public async Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not WaitNode waitNode) {
                Debug.LogError("WaitNodeProcessor: Invalid node type.");
                return null;
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, waitNode.Id);

            bool completed = false;

            switch (waitNode.WaitType) {
                case WaitType.Time:
                    completed = await WaitForTime(waitNode.Duration, waitNode.Timeout);
                    break;

                case WaitType.Condition:
                    completed = await WaitForCondition(waitNode, context);
                    break;

                case WaitType.Event:
                    completed = await WaitForEvent(waitNode, context);
                    break;
            }

            // Return timeout node if not completed and timeout node exists
            if (!completed && !string.IsNullOrEmpty(waitNode.TimeoutNodeId)) {
                return waitNode.TimeoutNodeId;
            }

            return waitNode.NextNodeId;
        }

        /// <summary>
        /// Waits for a specified duration.
        /// </summary>
        private async Task<bool> WaitForTime(float duration, float timeout) {
            float waitTime = timeout > 0 ? Mathf.Min(duration, timeout) : duration;
            await Task.Delay((int)(waitTime * 1000));
            return true;
        }

        /// <summary>
        /// Waits for a condition to become true.
        /// </summary>
        private async Task<bool> WaitForCondition(WaitNode node, ConversationContext context) {
            if (node.WaitCondition == null) {
                return true;
            }

            ICondition condition = context.ConditionFactory.CreateCondition(node.WaitCondition);
            if (condition == null) {
                return true;
            }

            float elapsed = 0f;
            float checkInterval = 0.1f;
            bool hasTimeout = node.Timeout > 0;

            while (true) {
                if (condition.Evaluate(context.ConditionContext)) {
                    return true;
                }

                if (hasTimeout && elapsed >= node.Timeout) {
                    return false;
                }

                await Task.Delay((int)(checkInterval * 1000));
                elapsed += checkInterval;
            }
        }

        /// <summary>
        /// Waits for a named event to be triggered.
        /// </summary>
        private async Task<bool> WaitForEvent(WaitNode node, ConversationContext context) {
            if (string.IsNullOrEmpty(node.WaitEventName)) {
                return true;
            }

            TaskCompletionSource<bool> tcs = new();
            float elapsed = 0f;
            bool hasTimeout = node.Timeout > 0;

            // Subscribe to event
            void OnEvent(string eventName, SerializableDictionary<string, string> parameters) {
                if (eventName == node.WaitEventName) {
                    context.OnEventTriggered -= OnEvent;
                    tcs.TrySetResult(true);
                }
            }

            context.OnEventTriggered += OnEvent;

            // Timeout task
            if (hasTimeout) {
                _ = Task.Run(async () => {
                    while (elapsed < node.Timeout && !tcs.Task.IsCompleted) {
                        await Task.Delay(100);
                        elapsed += 0.1f;
                    }

                    if (!tcs.Task.IsCompleted) {
                        context.OnEventTriggered -= OnEvent;
                        tcs.TrySetResult(false);
                    }
                });
            }

            return await tcs.Task;
        }
    }
}