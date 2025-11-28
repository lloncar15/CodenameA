using System;
using UnityEngine.Events;

namespace GimGim.ConversationSystem {
    //TODO: implement custom events from the notification center
    /// <summary>
    /// UnityEvent for conversation start/end.
    /// </summary>
    [Serializable]
    public class ConversationEvent : UnityEvent { }

    /// <summary>
    /// UnityEvent with conversation ID parameter.
    /// </summary>
    [Serializable]
    public class ConversationIdEvent : UnityEvent<string> { }

    /// <summary>
    /// UnityEvent with node parameter.
    /// </summary>
    [Serializable]
    public class ConversationNodeEvent : UnityEvent<ConversationNode> { }

    /// <summary>
    /// UnityEvent with choice ID parameter.
    /// </summary>
    [Serializable]
    public class ChoiceSelectedEvent : UnityEvent<string, string> { } // nodeId, choiceId

    /// <summary>
    /// UnityEvent for dialogue events triggered from commands.
    /// </summary>
    [Serializable]
    public class DialogueEventTriggered : UnityEvent<string> { } // eventName

    /// <summary>
    /// UnityEvent for dialogue events with parameters.
    /// </summary>
    [Serializable]
    public class DialogueEventWithParamsTriggered : UnityEvent<string, string> { } // eventName, paramsJson

    /// <summary>
    /// UnityEvent for variable changes.
    /// </summary>
    [Serializable]
    public class VariableChangedEvent : UnityEvent<string, string> { } // variableName, newValue

    /// <summary>
    /// UnityEvent for speaker changes.
    /// </summary>
    [Serializable]
    public class SpeakerChangedEvent : UnityEvent<string, string> { } // speakerId, speakerName

    /// <summary>
    /// UnityEvent for expression changes.
    /// </summary>
    [Serializable]
    public class ExpressionChangedEvent : UnityEvent<string, string> { } // speakerId, expressionKey
}