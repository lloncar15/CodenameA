// Assets/Scripts/ConversationSystem/Runtime/Processors/ChoiceNodeProcessor.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes ChoiceNode instances.
    /// </summary>
    public class ChoiceNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Choice;

        public async Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not ChoiceNode choiceNode) {
                Debug.LogError("ChoiceNodeProcessor: Invalid node type.");
                return null;
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, choiceNode.Id);

            // Build choice view data
            ChoiceNodeViewData viewData = CreateViewData(choiceNode, context);

            // Check if any choices are available
            bool hasSelectableChoices = false;
            foreach (var choice in viewData.Choices) {
                if (choice.IsVisible && choice.IsSelectable) {
                    hasSelectableChoices = true;
                    break;
                }
            }

            if (!hasSelectableChoices) {
                Debug.LogWarning($"ChoiceNodeProcessor: No selectable choices in node '{choiceNode.Id}'.");
                // If there's a timeout node, use that
                if (!string.IsNullOrEmpty(choiceNode.TimeoutNodeId)) {
                    return choiceNode.TimeoutNodeId;
                }
                return null;
            }

            // Show choices
            string selectedChoiceId = null;

            if (context.View != null) {
                await context.View.ShowChoicesAsync(viewData);

                // Wait for selection (with optional timeout)
                selectedChoiceId = await WaitForChoice(context, viewData.TimeLimit, choiceNode.TimeoutNodeId);

                context.View.HideChoices();
            }

            // Process selection
            if (string.IsNullOrEmpty(selectedChoiceId)) {
                // Timeout or no selection
                return choiceNode.TimeoutNodeId;
            }

            // Find selected choice
            ChoiceData selectedChoice = FindChoice(choiceNode, selectedChoiceId);
            if (selectedChoice == null) {
                Debug.LogError($"ChoiceNodeProcessor: Choice '{selectedChoiceId}' not found.");
                return null;
            }

            // Record choice
            context.StateManager?.RecordChoice(context.CurrentConversationId, choiceNode.Id, selectedChoiceId);

            // Execute OnSelect commands
            if (selectedChoice.OnSelectCommands != null && selectedChoice.OnSelectCommands.Count > 0) {
                CommandExecutor executor = new(context.CommandFactory);
                await executor.ExecuteAllAsync(selectedChoice.OnSelectCommands, context.CommandContext);
            }

            return selectedChoice.NextNodeId;
        }

        /// <summary>
        /// Creates view data from the choice node.
        /// </summary>
        private ChoiceNodeViewData CreateViewData(ChoiceNode node, ConversationContext context) {
            var viewData = new ChoiceNodeViewData {
                PromptText = node.PromptText,
                TimeLimit = node.TimeLimit,
                UseTypewriter = true,
                Choices = new List<ChoiceOptionViewData>()
            };

            // Build choices with condition evaluation
            List<ChoiceData> choices = node.Choices;
            if (node.ShuffleChoices) {
                choices = new List<ChoiceData>(node.Choices);
                ShuffleList(choices);
            }

            for (int i = 0; i < choices.Count; i++) {
                ChoiceData choice = choices[i];
                ChoiceOptionViewData optionData = CreateChoiceOptionData(choice, i, context);
                viewData.Choices.Add(optionData);
            }

            return viewData;
        }

        /// <summary>
        /// Creates view data for a single choice option.
        /// </summary>
        private ChoiceOptionViewData CreateChoiceOptionData(ChoiceData choice, int index, ConversationContext context) {
            var optionData = new ChoiceOptionViewData {
                Id = choice.Id,
                Text = choice.Text,
                ConsequencePreview = choice.ConsequencePreview,
                Index = index,
                IsVisible = true,
                IsSelectable = true
            };

            // Evaluate visibility condition
            if (choice.VisibilityCondition != null) {
                bool conditionMet = context.ConditionContext != null && 
                                    ConditionEvaluator.Evaluate(choice.VisibilityCondition, context.ConditionContext);

                // Handle visibility based on ChoiceVisibility setting
                switch (choice.Visibility) {
                    case ChoiceVisibility.Visible:
                        // Always visible, but may be greyed out
                        optionData.IsVisible = true;
                        optionData.IsSelectable = conditionMet;
                        if (!conditionMet) {
                            optionData.UnavailableReason = choice.UnavailableReason ?? "Not available";
                        }
                        break;

                    case ChoiceVisibility.Hidden:
                        // Hidden when condition not met
                        optionData.IsVisible = conditionMet;
                        optionData.IsSelectable = conditionMet;
                        break;

                    case ChoiceVisibility.GreyedOut:
                        // Always visible, greyed out when condition not met
                        optionData.IsVisible = true;
                        optionData.IsSelectable = conditionMet;
                        if (!conditionMet) {
                            optionData.UnavailableReason = choice.UnavailableReason ?? "Requirements not met";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Evaluate selectable condition (if visibility passed)
            if (optionData.IsVisible && choice.SelectableCondition != null) {
                bool selectable = EvaluateCondition(choice.SelectableCondition, context);
                
                if (!selectable) {
                    optionData.IsSelectable = false;
                    optionData.UnavailableReason = choice.UnavailableReason ?? "Requirements not met";
                }
            }

            return optionData;
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

        /// <summary>
        /// Waits for the player to select a choice.
        /// </summary>
        private async Task<string> WaitForChoice(ConversationContext context, float timeLimit, string timeoutNodeId) {
            TaskCompletionSource<string> tcs = new();
            float elapsed = 0f;
            bool hasTimeout = timeLimit > 0;

            void OnChoiceSelected(string choiceId) {
                context.View.OnChoiceSelected -= OnChoiceSelected;
                tcs.TrySetResult(choiceId);
            }

            context.View.OnChoiceSelected += OnChoiceSelected;

            // Handle timeout
            if (hasTimeout) {
                _ = Task.Run(async () => {
                    while (elapsed < timeLimit && !tcs.Task.IsCompleted) {
                        await Task.Delay(100);
                        elapsed += 0.1f;
                    }

                    if (!tcs.Task.IsCompleted) {
                        context.View.OnChoiceSelected -= OnChoiceSelected;
                        tcs.TrySetResult(null); // Timeout
                    }
                });
            }

            return await tcs.Task;
        }

        /// <summary>
        /// Finds a choice by ID.
        /// </summary>
        private ChoiceData FindChoice(ChoiceNode node, string choiceId) {
            foreach (ChoiceData choice in node.Choices) {
                if (choice.Id == choiceId) {
                    return choice;
                }
            }
            return null;
        }

        /// <summary>
        /// Shuffles a list in place.
        /// </summary>
        private void ShuffleList<T>(List<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}