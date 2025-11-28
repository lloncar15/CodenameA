namespace GimGim.ConversationSystem {
    /// <summary>
    /// Defines the types of nodes available in a conversation.
    /// </summary>
    public enum ConversationNodeType {
        /// <summary>
        /// Displays text to the player, optionally with a speaker.
        /// </summary>
        Text,
        
        /// <summary>
        /// Presents choices to the player and branches based on selection.
        /// </summary>
        Choice,
        
        /// <summary>
        /// Automatically branches based on conditions without player input.
        /// </summary>
        Branch,
        
        /// <summary>
        /// Executes commands/events without displaying anything.
        /// </summary>
        Event,
        
        /// <summary>
        /// Randomly selects one of several paths.
        /// </summary>
        Random,
        
        /// <summary>
        /// Waits for a duration, condition, or event before continuing.
        /// </summary>
        Wait,
        
        /// <summary>
        /// Jumps to another node or conversation.
        /// </summary>
        Jump
    }

    /// <summary>
    /// Defines comparison operators for conditions.
    /// </summary>
    public enum ComparisonOperator {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals
    }

    /// <summary>
    /// Defines logical operators for combining conditions.
    /// </summary>
    public enum LogicalOperator {
        And,
        Or
    }

    /// <summary>
    /// Defines the type of wait operation.
    /// </summary>
    public enum WaitType {
        /// <summary>
        /// Wait for a specific duration in seconds.
        /// </summary>
        Time,
        
        /// <summary>
        /// Wait until a condition becomes true.
        /// </summary>
        Condition,
        
        /// <summary>
        /// Wait until a specific event is triggered.
        /// </summary>
        Event
    }

    /// <summary>
    /// Defines the visibility state of a choice.
    /// </summary>
    public enum ChoiceVisibility {
        /// <summary>
        /// Choice is visible and selectable.
        /// </summary>
        Visible,
        
        /// <summary>
        /// Choice is visible but greyed out and not selectable.
        /// </summary>
        GreyedOut,
        
        /// <summary>
        /// Choice is completely hidden.
        /// </summary>
        Hidden
    }
}