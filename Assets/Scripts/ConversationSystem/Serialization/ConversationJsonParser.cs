using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Parses raw JSON data into runtime conversation objects.
    /// </summary>
    public class ConversationJsonParser {
        /// <summary>
        /// Parses raw conversation data into a ConversationData object.
        /// </summary>
        /// <param name="raw">The raw JSON data.</param>
        /// <returns>The parsed ConversationData, or null if parsing fails.</returns>
        public ConversationData Parse(RawConversationData raw) {
            if (raw == null) {
                Debug.LogError("ConversationJsonParser: Raw data is null.");
                return null;
            }

            try {
                ConversationData conversation = new() {
                    Id = raw.id,
                    Name = raw.name,
                    Description = raw.description,
                    StartNodeId = raw.startNodeId,
                    Version = raw.version
                };

                // Parse participants
                if (raw.participantIds != null) {
                    conversation.ParticipantIds.AddRange(raw.participantIds);
                }

                // Parse required predicates
                if (raw.requiredPredicates != null) {
                    foreach (KeyValuePair<string, string> kvp in raw.requiredPredicates) {
                        conversation.RequiredPredicates[kvp.Key] = kvp.Value;
                    }
                }

                // Parse metadata
                if (raw.metadata != null) {
                    foreach (KeyValuePair<string, string> kvp in raw.metadata) {
                        conversation.Metadata[kvp.Key] = kvp.Value;
                    }
                }

                // Parse nodes
                if (raw.nodes != null) {
                    foreach (RawNodeData rawNode in raw.nodes) {
                        ConversationNode node = ParseNode(rawNode);
                        if (node != null) {
                            conversation.AddNode(node);
                        }
                    }
                }

                return conversation;
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationJsonParser: Error parsing conversation: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parses a raw node into the appropriate ConversationNode subclass.
        /// </summary>
        /// <param name="raw">The raw node data.</param>
        /// <returns>The parsed node, or null if parsing fails.</returns>
        private ConversationNode ParseNode(RawNodeData raw) {
            if (raw == null || string.IsNullOrEmpty(raw.type)) {
                Debug.LogWarning("ConversationJsonParser: Node has no type.");
                return null;
            }

            ConversationNode node = raw.type.ToLower() switch {
                "text" => ParseTextNode(raw),
                "choice" => ParseChoiceNode(raw),
                "branch" => ParseBranchNode(raw),
                "event" => ParseEventNode(raw),
                "random" => ParseRandomNode(raw),
                "wait" => ParseWaitNode(raw),
                "jump" => ParseJumpNode(raw),
                _ => null
            };

            if (node == null) {
                Debug.LogWarning($"ConversationJsonParser: Unknown node type '{raw.type}'.");
                return null;
            }

            // Parse common metadata
            if (raw.metadata != null) {
                foreach (KeyValuePair<string, string> kvp in raw.metadata) {
                    node.Metadata[kvp.Key] = kvp.Value;
                }
            }

            return node;
        }

        /// <summary>
        /// Parses a TextNode from raw data.
        /// </summary>
        private TextNode ParseTextNode(RawNodeData raw) {
            TextNode node = new() {
                Id = raw.id,
                SpeakerId = raw.speakerId,
                Expression = raw.expression,
                Text = raw.text,
                NextNodeId = raw.nextNodeId,
                RequiresInput = raw.requiresInput,
                AutoAdvanceDelay = raw.autoAdvanceDelay
            };

            if (raw.onEnterCommands != null) {
                foreach (RawCommandData cmd in raw.onEnterCommands) {
                    node.OnEnterCommands.Add(ParseCommand(cmd));
                }
            }

            if (raw.onExitCommands != null) {
                foreach (RawCommandData cmd in raw.onExitCommands) {
                    node.OnExitCommands.Add(ParseCommand(cmd));
                }
            }

            return node;
        }

        /// <summary>
        /// Parses a ChoiceNode from raw data.
        /// </summary>
        private ChoiceNode ParseChoiceNode(RawNodeData raw) {
            ChoiceNode node = new() {
                Id = raw.id,
                PromptText = raw.promptText,
                SpeakerId = raw.speakerId,
                Expression = raw.expression,
                ShuffleChoices = raw.shuffleChoices,
                TimeLimit = raw.timeLimit,
                TimeoutNodeId = raw.timeoutNodeId
            };

            if (raw.choices != null) {
                foreach (RawChoiceData rawChoice in raw.choices) {
                    node.Choices.Add(ParseChoice(rawChoice));
                }
            }

            return node;
        }

        /// <summary>
        /// Parses a ChoiceData from raw data.
        /// </summary>
        private ChoiceData ParseChoice(RawChoiceData raw) {
            ChoiceData choice = new() {
                Id = raw.id,
                Text = raw.text,
                NextNodeId = raw.nextNodeId,
                UnavailableReason = raw.unavailableReason,
                ConsequencePreview = raw.consequencePreview
            };

            if (raw.visibilityCondition != null) {
                choice.VisibilityCondition = ParseCondition(raw.visibilityCondition);
            }

            if (raw.selectableCondition != null) {
                choice.SelectableCondition = ParseCondition(raw.selectableCondition);
            }

            if (raw.onSelectCommands != null) {
                foreach (RawCommandData cmd in raw.onSelectCommands) {
                    choice.OnSelectCommands.Add(ParseCommand(cmd));
                }
            }

            return choice;
        }

        /// <summary>
        /// Parses a BranchNode from raw data.
        /// </summary>
        private BranchNode ParseBranchNode(RawNodeData raw) {
            BranchNode node = new() {
                Id = raw.id,
                DefaultNodeId = raw.defaultNodeId
            };

            if (raw.branches != null) {
                foreach (RawBranchData rawBranch in raw.branches) {
                    BranchOption branch = new() {
                        Condition = ParseCondition(rawBranch.condition),
                        NextNodeId = rawBranch.nextNodeId,
                        Priority = rawBranch.priority
                    };
                    node.Branches.Add(branch);
                }
            }

            return node;
        }

        /// <summary>
        /// Parses an EventNode from raw data.
        /// </summary>
        private EventNode ParseEventNode(RawNodeData raw) {
            EventNode node = new() {
                Id = raw.id,
                NextNodeId = raw.nextNodeId
            };

            if (raw.commands != null) {
                foreach (RawCommandData cmd in raw.commands) {
                    node.Commands.Add(ParseCommand(cmd));
                }
            }

            return node;
        }

        /// <summary>
        /// Parses a RandomNode from raw data.
        /// </summary>
        private RandomNode ParseRandomNode(RawNodeData raw) {
            RandomNode node = new() {
                Id = raw.id,
                AvoidRepeat = raw.avoidRepeat
            };

            if (raw.options != null) {
                foreach (RawRandomOptionData rawOption in raw.options) {
                    RandomOption option = new() {
                        NextNodeId = rawOption.nextNodeId,
                        Weight = rawOption.weight
                    };

                    if (rawOption.condition != null) {
                        option.Condition = ParseCondition(rawOption.condition);
                    }

                    node.Options.Add(option);
                }
            }

            return node;
        }

        /// <summary>
        /// Parses a WaitNode from raw data.
        /// </summary>
        private WaitNode ParseWaitNode(RawNodeData raw) {
            WaitType waitType = WaitType.Time;
            if (!string.IsNullOrEmpty(raw.waitType)) {
                Enum.TryParse(raw.waitType, true, out waitType);
            }

            WaitNode node = new() {
                Id = raw.id,
                WaitType = waitType,
                Duration = raw.duration,
                WaitEventName = raw.waitEventName,
                Timeout = raw.timeout,
                TimeoutNodeId = raw.timeoutNodeId,
                NextNodeId = raw.nextNodeId
            };

            if (raw.waitCondition != null) {
                node.WaitCondition = ParseCondition(raw.waitCondition);
            }

            return node;
        }

        /// <summary>
        /// Parses a JumpNode from raw data.
        /// </summary>
        private JumpNode ParseJumpNode(RawNodeData raw) {
            return new JumpNode {
                Id = raw.id,
                TargetNodeId = raw.targetNodeId,
                TargetConversationId = raw.targetConversationId,
                TargetConversationStartNodeId = raw.targetConversationStartNodeId,
                ReturnAfterTarget = raw.returnAfterTarget,
                ReturnNodeId = raw.returnNodeId
            };
        }

        /// <summary>
        /// Parses a ConditionData from raw data.
        /// </summary>
        private ConditionData ParseCondition(RawConditionData raw) {
            if (raw == null) {
                return null;
            }

            ConditionData condition = new() {
                Type = raw.type ?? "simple",
                Variable = raw.variable,
                Value = raw.value,
                ValueType = raw.valueType ?? "bool",
                PredicateName = raw.predicateName,
                Negate = raw.negate
            };

            // Parse operator
            if (!string.IsNullOrEmpty(raw.@operator)) {
                if (Enum.TryParse(raw.@operator, true, out ComparisonOperator op)) {
                    condition.Operator = op;
                }
            }

            // Parse logical operator
            if (!string.IsNullOrEmpty(raw.logicalOperator)) {
                if (Enum.TryParse(raw.logicalOperator, true, out LogicalOperator logOp)) {
                    condition.LogicalOperator = logOp;
                }
            }

            // Parse sub-conditions
            if (raw.subConditions != null) {
                foreach (RawConditionData sub in raw.subConditions) {
                    condition.SubConditions.Add(ParseCondition(sub));
                }
            }

            // Parse predicate parameters
            if (raw.predicateParameters != null) {
                foreach (KeyValuePair<string, string> kvp in raw.predicateParameters) {
                    condition.PredicateParameters[kvp.Key] = kvp.Value;
                }
            }

            return condition;
        }

        /// <summary>
        /// Parses a CommandData from raw data.
        /// </summary>
        private CommandData ParseCommand(RawCommandData raw) {
            if (raw == null) {
                return null;
            }

            CommandData command = new() {
                CommandType = raw.commandType
            };

            if (raw.parameters != null) {
                foreach (KeyValuePair<string, string> kvp in raw.parameters) {
                    command.Parameters[kvp.Key] = kvp.Value;
                }
            }

            return command;
        }
    }
}