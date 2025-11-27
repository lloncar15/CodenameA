#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Validates conversation data for errors and warnings.
    /// </summary>
    public class ConversationValidator {
        private readonly List<ValidationResult> _results;
        private ConversationData _conversation;

        public ConversationValidator() {
            _results = new List<ValidationResult>();
        }

        /// <summary>
        /// Gets the validation results.
        /// </summary>
        public IReadOnlyList<ValidationResult> Results => _results;

        /// <summary>
        /// Gets whether validation passed with no errors.
        /// </summary>
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Gets whether there are any errors.
        /// </summary>
        public bool HasErrors {
            get {
                foreach (ValidationResult result in _results) {
                    if (result.Severity == ValidationSeverity.Error) {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets whether there are any warnings.
        /// </summary>
        public bool HasWarnings {
            get {
                foreach (ValidationResult result in _results) {
                    if (result.Severity == ValidationSeverity.Warning) {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Validates a conversation.
        /// </summary>
        /// <param name="conversation">The conversation to validate.</param>
        /// <returns>True if valid (no errors), false otherwise.</returns>
        public bool Validate(ConversationData conversation) {
            _results.Clear();
            _conversation = conversation;

            if (conversation == null) {
                AddError("Conversation", "Conversation data is null.");
                return false;
            }

            ValidateBasicInfo();
            ValidateStartNode();
            ValidateNodes();
            ValidateReferences();
            ValidateOrphanedNodes();

            return IsValid;
        }

        /// <summary>
        /// Validates basic conversation info.
        /// </summary>
        private void ValidateBasicInfo() {
            if (string.IsNullOrEmpty(_conversation.Id)) {
                AddError("Conversation", "Conversation ID is empty.");
            }

            if (string.IsNullOrEmpty(_conversation.Name)) {
                AddWarning("Conversation", "Conversation name is empty.");
            }

            if (_conversation.Nodes == null || _conversation.Nodes.Count == 0) {
                AddError("Conversation", "Conversation has no nodes.");
            }
        }

        /// <summary>
        /// Validates the start node.
        /// </summary>
        private void ValidateStartNode() {
            if (string.IsNullOrEmpty(_conversation.StartNodeId)) {
                AddError("StartNode", "Start node ID is not set.");
                return;
            }

            if (!_conversation.Nodes.ContainsKey(_conversation.StartNodeId)) {
                AddError("StartNode", $"Start node '{_conversation.StartNodeId}' does not exist.");
            }
        }

        /// <summary>
        /// Validates all nodes.
        /// </summary>
        private void ValidateNodes() {
            if (_conversation.Nodes == null) {
                return;
            }

            foreach (var kvp in _conversation.Nodes) {
                ValidateNode(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Validates a single node.
        /// </summary>
        private void ValidateNode(string nodeId, ConversationNode node) {
            if (node == null) {
                AddError(nodeId, "Node is null.");
                return;
            }

            if (string.IsNullOrEmpty(node.Id)) {
                AddError(nodeId, "Node ID is empty.");
            }
            else if (node.Id != nodeId) {
                AddWarning(nodeId, $"Node ID '{node.Id}' doesn't match dictionary key '{nodeId}'.");
            }

            switch (node) {
                case TextNode textNode:
                    ValidateTextNode(nodeId, textNode);
                    break;
                case ChoiceNode choiceNode:
                    ValidateChoiceNode(nodeId, choiceNode);
                    break;
                case BranchNode branchNode:
                    ValidateBranchNode(nodeId, branchNode);
                    break;
                case EventNode eventNode:
                    ValidateEventNode(nodeId, eventNode);
                    break;
                case RandomNode randomNode:
                    ValidateRandomNode(nodeId, randomNode);
                    break;
                case WaitNode waitNode:
                    ValidateWaitNode(nodeId, waitNode);
                    break;
                case JumpNode jumpNode:
                    ValidateJumpNode(nodeId, jumpNode);
                    break;
            }
        }

        /// <summary>
        /// Validates a text node.
        /// </summary>
        private void ValidateTextNode(string nodeId, TextNode node) {
            if (string.IsNullOrEmpty(node.Text)) {
                AddWarning(nodeId, "Text node has no text content.");
            }

            // NextNodeId can be null (end of conversation)
            if (!string.IsNullOrEmpty(node.NextNodeId)) {
                ValidateNodeReference(nodeId, node.NextNodeId, "NextNodeId");
            }

            ValidateCommands(nodeId, node.OnEnterCommands, "OnEnterCommands");
            ValidateCommands(nodeId, node.OnExitCommands, "OnExitCommands");
        }

        /// <summary>
        /// Validates a choice node.
        /// </summary>
        private void ValidateChoiceNode(string nodeId, ChoiceNode node) {
            if (node.Choices == null || node.Choices.Count == 0) {
                AddError(nodeId, "Choice node has no choices.");
                return;
            }

            HashSet<string> choiceIds = new HashSet<string>();

            foreach (ChoiceData choice in node.Choices) {
                if (string.IsNullOrEmpty(choice.Id)) {
                    AddError(nodeId, "Choice has no ID.");
                    continue;
                }

                if (!choiceIds.Add(choice.Id)) {
                    AddError(nodeId, $"Duplicate choice ID '{choice.Id}'.");
                }

                if (string.IsNullOrEmpty(choice.Text)) {
                    AddWarning(nodeId, $"Choice '{choice.Id}' has no text.");
                }

                if (string.IsNullOrEmpty(choice.NextNodeId)) {
                    AddWarning(nodeId, $"Choice '{choice.Id}' has no next node (will end conversation).");
                }
                else {
                    ValidateNodeReference(nodeId, choice.NextNodeId, $"Choice '{choice.Id}' NextNodeId");
                }

                ValidateCondition(nodeId, choice.VisibilityCondition, $"Choice '{choice.Id}' VisibilityCondition");
                ValidateCondition(nodeId, choice.SelectableCondition, $"Choice '{choice.Id}' SelectableCondition");
                ValidateCommands(nodeId, choice.OnSelectCommands, $"Choice '{choice.Id}' OnSelectCommands");
            }

            if (node.TimeLimit > 0 && string.IsNullOrEmpty(node.TimeoutNodeId)) {
                AddWarning(nodeId, "Choice node has time limit but no timeout node.");
            }

            if (!string.IsNullOrEmpty(node.TimeoutNodeId)) {
                ValidateNodeReference(nodeId, node.TimeoutNodeId, "TimeoutNodeId");
            }
        }

        /// <summary>
        /// Validates a branch node.
        /// </summary>
        private void ValidateBranchNode(string nodeId, BranchNode node) {
            if (node.Branches == null || node.Branches.Count == 0) {
                AddWarning(nodeId, "Branch node has no branches, will always use default.");
            }
            else {
                foreach (var branch in node.Branches) {
                    if (string.IsNullOrEmpty(branch.NextNodeId)) {
                        AddError(nodeId, "Branch has no target node.");
                    }
                    else {
                        ValidateNodeReference(nodeId, branch.NextNodeId, "Branch TargetNodeId");
                    }

                    if (branch.Condition == null) {
                        AddWarning(nodeId, "Branch has no condition (will always be taken).");
                    }
                    else {
                        ValidateCondition(nodeId, branch.Condition, "Branch Condition");
                    }
                }
            }

            if (string.IsNullOrEmpty(node.DefaultNodeId)) {
                AddWarning(nodeId, "Branch node has no default node.");
            }
            else {
                ValidateNodeReference(nodeId, node.DefaultNodeId, "DefaultNodeId");
            }
        }

        /// <summary>
        /// Validates an event node.
        /// </summary>
        private void ValidateEventNode(string nodeId, EventNode node) {
            if (node.Commands == null || node.Commands.Count == 0) {
                AddWarning(nodeId, "Event node has no commands.");
            }
            else {
                ValidateCommands(nodeId, node.Commands, "Commands");
            }

            if (!string.IsNullOrEmpty(node.NextNodeId)) {
                ValidateNodeReference(nodeId, node.NextNodeId, "NextNodeId");
            }
        }

        /// <summary>
        /// Validates a random node.
        /// </summary>
        private void ValidateRandomNode(string nodeId, RandomNode node) {
            if (node.Options == null || node.Options.Count == 0) {
                AddError(nodeId, "Random node has no options.");
                return;
            }

            float totalWeight = 0f;
            foreach (RandomOption option in node.Options) {
                if (string.IsNullOrEmpty(option.NextNodeId)) {
                    AddError(nodeId, "Random option has no target node.");
                }
                else {
                    ValidateNodeReference(nodeId, option.NextNodeId, "Random Option TargetNodeId");
                }

                if (option.Weight <= 0) {
                    AddWarning(nodeId, "Random option has zero or negative weight.");
                }

                totalWeight += option.Weight;
            }

            if (totalWeight <= 0) {
                AddError(nodeId, "Random node has no positive weights.");
            }
        }

        /// <summary>
        /// Validates a wait node.
        /// </summary>
        private void ValidateWaitNode(string nodeId, WaitNode node) {
            switch (node.WaitType) {
                case WaitType.Time:
                    if (node.Duration <= 0) {
                        AddWarning(nodeId, "Wait node has zero or negative duration.");
                    }
                    break;

                case WaitType.Condition:
                    if (node.WaitCondition == null) {
                        AddError(nodeId, "Wait node (condition type) has no condition.");
                    }
                    else {
                        ValidateCondition(nodeId, node.WaitCondition, "WaitCondition");
                    }
                    break;

                case WaitType.Event:
                    if (string.IsNullOrEmpty(node.WaitEventName)) {
                        AddError(nodeId, "Wait node (event type) has no event name.");
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(node.NextNodeId)) {
                ValidateNodeReference(nodeId, node.NextNodeId, "NextNodeId");
            }

            if (!string.IsNullOrEmpty(node.TimeoutNodeId)) {
                ValidateNodeReference(nodeId, node.TimeoutNodeId, "TimeoutNodeId");
            }
        }

        /// <summary>
        /// Validates a jump node.
        /// </summary>
        private void ValidateJumpNode(string nodeId, JumpNode node) {
            bool hasLocalTarget = !string.IsNullOrEmpty(node.TargetNodeId);
            bool hasConversationTarget = !string.IsNullOrEmpty(node.TargetConversationId);

            if (!hasLocalTarget && !hasConversationTarget) {
                AddError(nodeId, "Jump node has no target.");
            }

            if (hasLocalTarget && !hasConversationTarget) {
                ValidateNodeReference(nodeId, node.TargetNodeId, "TargetNodeId");
            }

            if (node.ReturnAfterTarget && string.IsNullOrEmpty(node.ReturnNodeId)) {
                AddWarning(nodeId, "Jump node set to return but has no return node ID.");
            }

            if (!string.IsNullOrEmpty(node.ReturnNodeId)) {
                ValidateNodeReference(nodeId, node.ReturnNodeId, "ReturnNodeId");
            }
        }

        /// <summary>
        /// Validates a node reference exists.
        /// </summary>
        private void ValidateNodeReference(string sourceNodeId, string targetNodeId, string fieldName) {
            if (_conversation.Nodes == null || !_conversation.Nodes.ContainsKey(targetNodeId)) {
                AddError(sourceNodeId, $"{fieldName} references non-existent node '{targetNodeId}'.");
            }
        }

        /// <summary>
        /// Validates a condition.
        /// </summary>
        private void ValidateCondition(string nodeId, ConditionData condition, string fieldName) {
            if (condition == null) {
                return;
            }

            if (string.IsNullOrEmpty(condition.Type)) {
                AddError(nodeId, $"{fieldName}: Condition has no type.");
                return;
            }

            switch (condition.Type.ToLower()) {
                case "simple":
                    if (string.IsNullOrEmpty(condition.Variable)) {
                        AddError(nodeId, $"{fieldName}: Simple condition has no variable.");
                    }
                    break;

                case "composite":
                    if (condition.SubConditions == null || condition.SubConditions.Count == 0) {
                        AddError(nodeId, $"{fieldName}: Composite condition has no sub-conditions.");
                    }
                    else {
                        for (int i = 0; i < condition.SubConditions.Count; i++) {
                            ValidateCondition(nodeId, condition.SubConditions[i], $"{fieldName}[{i}]");
                        }
                    }
                    break;

                case "predicate":
                    if (string.IsNullOrEmpty(condition.PredicateName)) {
                        AddError(nodeId, $"{fieldName}: Predicate condition has no predicate name.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Validates commands.
        /// </summary>
        private void ValidateCommands(string nodeId, List<CommandData> commands, string fieldName) {
            if (commands == null) {
                return;
            }

            for (int i = 0; i < commands.Count; i++) {
                CommandData cmd = commands[i];
                if (string.IsNullOrEmpty(cmd.CommandType)) {
                    AddError(nodeId, $"{fieldName}[{i}]: Command has no type.");
                }
            }
        }

        /// <summary>
        /// Validates all node references and finds orphaned nodes.
        /// </summary>
        private void ValidateReferences() {
            // Additional reference validation can go here
        }

        /// <summary>
        /// Finds nodes that are not reachable from the start node.
        /// </summary>
        private void ValidateOrphanedNodes() {
            if (_conversation.Nodes == null || string.IsNullOrEmpty(_conversation.StartNodeId)) {
                return;
            }

            HashSet<string> reachable = new HashSet<string>();
            Queue<string> toVisit = new Queue<string>();
            
            toVisit.Enqueue(_conversation.StartNodeId);

            while (toVisit.Count > 0) {
                string nodeId = toVisit.Dequeue();
                
                if (!reachable.Add(nodeId)) {
                    continue;
                }

                if (!_conversation.Nodes.TryGetValue(nodeId, out ConversationNode node)) {
                    continue;
                }

                foreach (string nextId in GetNextNodeIds(node)) {
                    if (!string.IsNullOrEmpty(nextId) && !reachable.Contains(nextId)) {
                        toVisit.Enqueue(nextId);
                    }
                }
            }

            foreach (string nodeId in _conversation.Nodes.Keys) {
                if (!reachable.Contains(nodeId)) {
                    AddWarning(nodeId, "Node is not reachable from start node (orphaned).");
                }
            }
        }

        /// <summary>
        /// Gets all possible next node IDs from a node.
        /// </summary>
        private IEnumerable<string> GetNextNodeIds(ConversationNode node) {
            switch (node) {
                case TextNode textNode:
                    if (!string.IsNullOrEmpty(textNode.NextNodeId)) {
                        yield return textNode.NextNodeId;
                    }
                    break;

                case ChoiceNode choiceNode:
                    if (choiceNode.Choices != null) {
                        foreach (ChoiceData choice in choiceNode.Choices) {
                            if (!string.IsNullOrEmpty(choice.NextNodeId)) {
                                yield return choice.NextNodeId;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(choiceNode.TimeoutNodeId)) {
                        yield return choiceNode.TimeoutNodeId;
                    }
                    break;

                case BranchNode branchNode:
                    if (branchNode.Branches != null) {
                        foreach (BranchOption branch in branchNode.Branches) {
                            if (!string.IsNullOrEmpty(branch.NextNodeId)) {
                                yield return branch.NextNodeId;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(branchNode.DefaultNodeId)) {
                        yield return branchNode.DefaultNodeId;
                    }
                    break;

                case EventNode eventNode:
                    if (!string.IsNullOrEmpty(eventNode.NextNodeId)) {
                        yield return eventNode.NextNodeId;
                    }
                    break;

                case RandomNode randomNode:
                    if (randomNode.Options != null) {
                        foreach (RandomOption option in randomNode.Options) {
                            if (!string.IsNullOrEmpty(option.NextNodeId)) {
                                yield return option.NextNodeId;
                            }
                        }
                    }
                    break;

                case WaitNode waitNode:
                    if (!string.IsNullOrEmpty(waitNode.NextNodeId)) {
                        yield return waitNode.NextNodeId;
                    }
                    if (!string.IsNullOrEmpty(waitNode.TimeoutNodeId)) {
                        yield return waitNode.TimeoutNodeId;
                    }
                    break;

                case JumpNode jumpNode:
                    if (!string.IsNullOrEmpty(jumpNode.TargetNodeId)) {
                        yield return jumpNode.TargetNodeId;
                    }
                    if (!string.IsNullOrEmpty(jumpNode.ReturnNodeId)) {
                        yield return jumpNode.ReturnNodeId;
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds an error result.
        /// </summary>
        private void AddError(string nodeId, string message) {
            _results.Add(new ValidationResult {
                Severity = ValidationSeverity.Error,
                NodeId = nodeId,
                Message = message
            });
        }

        /// <summary>
        /// Adds a warning result.
        /// </summary>
        private void AddWarning(string nodeId, string message) {
            _results.Add(new ValidationResult {
                Severity = ValidationSeverity.Warning,
                NodeId = nodeId,
                Message = message
            });
        }

        /// <summary>
        /// Adds an info result.
        /// </summary>
        private void AddInfo(string nodeId, string message) {
            _results.Add(new ValidationResult {
                Severity = ValidationSeverity.Info,
                NodeId = nodeId,
                Message = message
            });
        }

        /// <summary>
        /// Gets a summary of the validation results.
        /// </summary>
        public string GetSummary() {
            int errors = 0;
            int warnings = 0;
            int infos = 0;

            foreach (ValidationResult result in _results) {
                switch (result.Severity) {
                    case ValidationSeverity.Error:
                        errors++;
                        break;
                    case ValidationSeverity.Warning:
                        warnings++;
                        break;
                    case ValidationSeverity.Info:
                        infos++;
                        break;
                }
            }

            return $"Validation complete: {errors} errors, {warnings} warnings, {infos} info messages.";
        }

        /// <summary>
        /// Gets the full validation report.
        /// </summary>
        public string GetReport() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetSummary());
            sb.AppendLine();

            foreach (ValidationResult result in _results) {
                string prefix = result.Severity switch {
                    ValidationSeverity.Error => "[ERROR]",
                    ValidationSeverity.Warning => "[WARN]",
                    _ => "[INFO]"
                };

                sb.AppendLine($"{prefix} [{result.NodeId}] {result.Message}");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Result of a validation check.
    /// </summary>
    public class ValidationResult {
        public ValidationSeverity Severity { get; set; }
        public string NodeId { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Severity levels for validation results.
    /// </summary>
    public enum ValidationSeverity {
        Info,
        Warning,
        Error
    }
}
#endif