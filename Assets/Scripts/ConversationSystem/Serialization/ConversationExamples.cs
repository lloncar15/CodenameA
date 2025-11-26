namespace GimGim.ConversationSystem {
    /// <summary>
    /// Documentation class containing example JSON schemas for reference.
    /// This class is not used at runtime - it's for documentation purposes.
    /// </summary>
    public static class ConversationExamples {
        /// <summary>
        /// Example of a complete conversation JSON structure.
        /// </summary>
        public const string EXAMPLE_CONVERSATION = @"
{
    ""id"": ""merchant_greeting"",
    ""name"": ""Merchant Greeting"",
    ""description"": ""Initial conversation with the village merchant"",
    ""startNodeId"": ""start"",
    ""version"": 1,
    ""participantIds"": [""merchant"", ""player""],
    ""requiredPredicates"": {
        ""IsPlayerSneaking"": ""Checks if player is in stealth mode""
    },
    ""metadata"": {
        ""author"": ""GameDesigner"",
        ""lastModified"": ""2024-01-15""
    },
    ""nodes"": [
        {
            ""id"": ""start"",
            ""type"": ""Text"",
            ""speakerId"": ""merchant"",
            ""expression"": ""happy"",
            ""text"": ""Welcome to my shop, traveler! What can I help you with today?"",
            ""nextNodeId"": ""main_choice"",
            ""requiresInput"": true
        },
        {
            ""id"": ""main_choice"",
            ""type"": ""Choice"",
            ""promptText"": ""What would you like to do?"",
            ""choices"": [
                {
                    ""id"": ""buy"",
                    ""text"": ""I'd like to buy something."",
                    ""nextNodeId"": ""buy_response""
                },
                {
                    ""id"": ""sell"",
                    ""text"": ""I have items to sell."",
                    ""nextNodeId"": ""sell_response""
                },
                {
                    ""id"": ""special"",
                    ""text"": ""[Requires 100 gold] Show me your special wares."",
                    ""nextNodeId"": ""special_response"",
                    ""selectableCondition"": {
                        ""type"": ""simple"",
                        ""variable"": ""playerGold"",
                        ""operator"": ""GreaterThanOrEquals"",
                        ""value"": ""100"",
                        ""valueType"": ""int""
                    },
                    ""unavailableReason"": ""You need at least 100 gold.""
                },
                {
                    ""id"": ""leave"",
                    ""text"": ""Never mind, goodbye."",
                    ""nextNodeId"": ""goodbye""
                }
            ]
        },
        {
            ""id"": ""buy_response"",
            ""type"": ""Event"",
            ""commands"": [
                {
                    ""commandType"": ""TriggerEvent"",
                    ""parameters"": {
                        ""event"": ""OpenShopUI"",
                        ""shopType"": ""buy""
                    }
                }
            ],
            ""nextNodeId"": null
        },
        {
            ""id"": ""goodbye"",
            ""type"": ""Text"",
            ""speakerId"": ""merchant"",
            ""expression"": ""neutral"",
            ""text"": ""Come back anytime!"",
            ""nextNodeId"": null
        }
    ]
}";

        /// <summary>
        /// Example of a branch node with conditions.
        /// </summary>
        public const string EXAMPLE_BRANCH_NODE = @"
{
    ""id"": ""check_reputation"",
    ""type"": ""Branch"",
    ""branches"": [
        {
            ""condition"": {
                ""type"": ""simple"",
                ""variable"": ""merchantReputation"",
                ""operator"": ""GreaterThanOrEquals"",
                ""value"": ""50"",
                ""valueType"": ""int""
            },
            ""nextNodeId"": ""high_reputation_greeting"",
            ""priority"": 1
        },
        {
            ""condition"": {
                ""type"": ""simple"",
                ""variable"": ""merchantReputation"",
                ""operator"": ""LessThan"",
                ""value"": ""0"",
                ""valueType"": ""int""
            },
            ""nextNodeId"": ""negative_reputation_greeting"",
            ""priority"": 0
        }
    ],
    ""defaultNodeId"": ""neutral_greeting""
}";

        /// <summary>
        /// Example of a composite condition.
        /// </summary>
        public const string EXAMPLE_COMPOSITE_CONDITION = @"
{
    ""type"": ""composite"",
    ""logicalOperator"": ""And"",
    ""subConditions"": [
        {
            ""type"": ""simple"",
            ""variable"": ""hasKey"",
            ""operator"": ""Equals"",
            ""value"": ""true"",
            ""valueType"": ""bool""
        },
        {
            ""type"": ""composite"",
            ""logicalOperator"": ""Or"",
            ""subConditions"": [
                {
                    ""type"": ""simple"",
                    ""variable"": ""playerGold"",
                    ""operator"": ""GreaterThanOrEquals"",
                    ""value"": ""50"",
                    ""valueType"": ""int""
                },
                {
                    ""type"": ""predicate"",
                    ""predicateName"": ""IsPlayerVIP""
                }
            ]
        }
    ]
}";
    }
}