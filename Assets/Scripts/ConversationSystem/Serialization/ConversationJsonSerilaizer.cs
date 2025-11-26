using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Serializes ConversationData objects to raw JSON-compatible structures.
    /// </summary>
    public class ConversationJsonSerializer {
        /// <summary>
        /// Serializes a ConversationData object to raw data.
        /// </summary>
        /// <param name="conversation">The conversation to serialize.</param>
        /// <returns>The raw data structure.</returns>
        public RawConversationData Serialize(ConversationData conversation) {
            if (conversation == null) {
                return null;
            }

            RawConversationData raw = new() {
                id = conversation.Id,
                name = conversation.Name,
                description = conversation.Description,
                startNodeId = conversation.StartNodeId,
                version = conversation.Version,
                participantIds = new List<string>(conversation.ParticipantIds),
                requiredPredicates = new Dictionary<string, string>(conversation.RequiredPredicates),
                metadata = new Dictionary<string, string>(conversation.Metadata),
                nodes = new List<RawNodeData>()
            };

            foreach (ConversationNode node in conversation.Nodes.Values) {
                raw.nodes.Add(SerializeNode(node));
            }

            return raw;
        }

        /// <summary>
        /// Serializes a conversation node to raw data.
        /// </summary>
        private RawNodeData SerializeNode(ConversationNode node) {
            RawNodeData raw = new() {
                id = node.Id,
                type = node.NodeType.ToString(),
                metadata = new Dictionary<string, string>(node.Metadata)
            };

            switch (node) {
                case TextNode textNode:
                    SerializeTextNode(textNode, raw);
                    break;
                case ChoiceNode choiceNode:
                    SerializeChoiceNode(choiceNode, raw);
                    break;
                case BranchNode branchNode:
                    SerializeBranchNode(branchNode, raw);
                    break;
                case EventNode eventNode:
                    SerializeEventNode(eventNode, raw);
                    break;
                case RandomNode randomNode:
                    SerializeRandomNode(randomNode, raw);
                    break;
                case WaitNode waitNode:
                    SerializeWaitNode(waitNode, raw);
                    break;
                case JumpNode jumpNode:
                    SerializeJumpNode(jumpNode, raw);
                    break;
            }

            return raw;
        }

        private void SerializeTextNode(TextNode node, RawNodeData raw) {
            raw.speakerId = node.SpeakerId;
            raw.expression = node.Expression;
            raw.text = node.Text;
            raw.nextNodeId = node.NextNodeId;
            raw.requiresInput = node.RequiresInput;
            raw.autoAdvanceDelay = node.AutoAdvanceDelay;
            raw.onEnterCommands = SerializeCommands(node.OnEnterCommands);
            raw.onExitCommands = SerializeCommands(node.OnExitCommands);
        }

        private void SerializeChoiceNode(ChoiceNode node, RawNodeData raw) {
            raw.promptText = node.PromptText;
            raw.speakerId = node.SpeakerId;
            raw.expression = node.Expression;
            raw.shuffleChoices = node.ShuffleChoices;
            raw.timeLimit = node.TimeLimit;
            raw.timeoutNodeId = node.TimeoutNodeId;
            raw.choices = new List<RawChoiceData>();

            foreach (ChoiceData choice in node.Choices) {
                raw.choices.Add(SerializeChoice(choice));
            }
        }

        private RawChoiceData SerializeChoice(ChoiceData choice) {
            return new RawChoiceData {
                id = choice.Id,
                text = choice.Text,
                nextNodeId = choice.NextNodeId,
                visibilityCondition = SerializeCondition(choice.VisibilityCondition),
                selectableCondition = SerializeCondition(choice.SelectableCondition),
                unavailableReason = choice.UnavailableReason,
                consequencePreview = choice.ConsequencePreview,
                onSelectCommands = SerializeCommands(choice.OnSelectCommands)
            };
        }

        private void SerializeBranchNode(BranchNode node, RawNodeData raw) {
            raw.defaultNodeId = node.DefaultNodeId;
            raw.branches = new List<RawBranchData>();

            foreach (BranchOption branch in node.Branches) {
                raw.branches.Add(new RawBranchData {
                    condition = SerializeCondition(branch.Condition),
                    nextNodeId = branch.NextNodeId,
                    priority = branch.Priority
                });
            }
        }

        private void SerializeEventNode(EventNode node, RawNodeData raw) {
            raw.nextNodeId = node.NextNodeId;
            raw.commands = SerializeCommands(node.Commands);
        }

        private void SerializeRandomNode(RandomNode node, RawNodeData raw) {
            raw.avoidRepeat = node.AvoidRepeat;
            raw.options = new List<RawRandomOptionData>();

            foreach (RandomOption option in node.Options) {
                raw.options.Add(new RawRandomOptionData {
                    nextNodeId = option.NextNodeId,
                    weight = option.Weight,
                    condition = SerializeCondition(option.Condition)
                });
            }
        }

        private void SerializeWaitNode(WaitNode node, RawNodeData raw) {
            raw.waitType = node.WaitType.ToString();
            raw.duration = node.Duration;
            raw.waitCondition = SerializeCondition(node.WaitCondition);
            raw.waitEventName = node.WaitEventName;
            raw.timeout = node.Timeout;
            raw.timeoutNodeId = node.TimeoutNodeId;
            raw.nextNodeId = node.NextNodeId;
        }

        private void SerializeJumpNode(JumpNode node, RawNodeData raw) {
            raw.targetNodeId = node.TargetNodeId;
            raw.targetConversationId = node.TargetConversationId;
            raw.targetConversationStartNodeId = node.TargetConversationStartNodeId;
            raw.returnAfterTarget = node.ReturnAfterTarget;
            raw.returnNodeId = node.ReturnNodeId;
        }

        private RawConditionData SerializeCondition(ConditionData condition) {
            if (condition == null) {
                return null;
            }

            RawConditionData raw = new() {
                type = condition.Type,
                variable = condition.Variable,
                @operator = condition.Operator.ToString(),
                value = condition.Value,
                valueType = condition.ValueType,
                logicalOperator = condition.LogicalOperator.ToString(),
                predicateName = condition.PredicateName,
                predicateParameters = new Dictionary<string, string>(condition.PredicateParameters),
                negate = condition.Negate,
                subConditions = new List<RawConditionData>()
            };

            foreach (var sub in condition.SubConditions) {
                raw.subConditions.Add(SerializeCondition(sub));
            }

            return raw;
        }

        private List<RawCommandData> SerializeCommands(List<CommandData> commands) {
            if (commands == null || commands.Count == 0) {
                return null;
            }

            List<RawCommandData> result = new();
            foreach (CommandData cmd in commands) {
                result.Add(new RawCommandData {
                    commandType = cmd.CommandType,
                    parameters = new Dictionary<string, string>(cmd.Parameters)
                });
            }

            return result;
        }
    }
}