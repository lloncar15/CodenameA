using System;
using System.Collections.Generic;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Raw JSON structure for conversation data.
    /// Used as an intermediate format before converting to runtime objects.
    /// </summary>
    [Serializable]
    public class RawConversationData {
        public string id;
        public string name;
        public string description;
        public string startNodeId;
        public int version = 1;
        public List<string> participantIds;
        public Dictionary<string, string> requiredPredicates;
        public Dictionary<string, string> metadata;
        public List<RawNodeData> nodes;
    }

    /// <summary>
    /// Raw JSON structure for a conversation node.
    /// Contains all possible fields for any node type.
    /// </summary>
    [Serializable]
    public class RawNodeData {
        // Common fields
        public string id;
        public string type;
        public Dictionary<string, string> metadata;

        // TextNode fields
        public string speakerId;
        public string expression;
        public string text;
        public string nextNodeId;
        public bool requiresInput = true;
        public float autoAdvanceDelay;
        public List<RawCommandData> onEnterCommands;
        public List<RawCommandData> onExitCommands;

        // ChoiceNode fields
        public string promptText;
        public List<RawChoiceData> choices;
        public bool shuffleChoices;
        public float timeLimit;
        public string timeoutNodeId;

        // BranchNode fields
        public List<RawBranchData> branches;
        public string defaultNodeId;

        // EventNode fields
        public List<RawCommandData> commands;

        // RandomNode fields
        public List<RawRandomOptionData> options;
        public bool avoidRepeat;

        // WaitNode fields
        public string waitType;
        public float duration;
        public RawConditionData waitCondition;
        public string waitEventName;
        public float timeout;

        // JumpNode fields
        public string targetNodeId;
        public string targetConversationId;
        public string targetConversationStartNodeId;
        public bool returnAfterTarget;
        public string returnNodeId;
    }

    /// <summary>
    /// Raw JSON structure for a choice.
    /// </summary>
    [Serializable]
    public class RawChoiceData {
        public string id;
        public string text;
        public string nextNodeId;
        public RawConditionData visibilityCondition;
        public RawConditionData selectableCondition;
        public string unavailableReason;
        public string consequencePreview;
        public List<RawCommandData> onSelectCommands;
    }

    /// <summary>
    /// Raw JSON structure for a branch option.
    /// </summary>
    [Serializable]
    public class RawBranchData {
        public RawConditionData condition;
        public string nextNodeId;
        public int priority;
    }

    /// <summary>
    /// Raw JSON structure for a random option.
    /// </summary>
    [Serializable]
    public class RawRandomOptionData {
        public string nextNodeId;
        public float weight = 1f;
        public RawConditionData condition;
    }

    /// <summary>
    /// Raw JSON structure for a condition.
    /// </summary>
    [Serializable]
    public class RawConditionData {
        public string type = "simple";
        
        // Simple condition fields
        public string variable;
        public string @operator;
        public string value;
        public string valueType = "bool";
        
        // Composite condition fields
        public string logicalOperator;
        public List<RawConditionData> subConditions;
        
        // Predicate condition fields
        public string predicateName;
        public Dictionary<string, string> predicateParameters;
        
        // Common
        public bool negate;
    }

    /// <summary>
    /// Raw JSON structure for a command.
    /// </summary>
    [Serializable]
    public class RawCommandData {
        public string commandType;
        public Dictionary<string, string> parameters;
    }
}