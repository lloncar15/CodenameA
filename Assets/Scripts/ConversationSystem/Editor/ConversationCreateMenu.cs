#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Editor menu items for creating conversation system assets.
    /// </summary>
    public static class ConversationCreateMenu {
        [MenuItem("Assets/Create/GimGim/Conversation System/Empty Conversation JSON", priority = 100)]
        public static void CreateEmptyConversationJson() {
            string json = @"{
    ""id"": ""new_conversation"",
    ""name"": ""New Conversation"",
    ""description"": """",
    ""startNodeId"": ""start"",
    ""nodes"": {
        ""start"": {
            ""type"": ""text"",
            ""id"": ""start"",
            ""speakerId"": """",
            ""text"": ""Hello! This is a new conversation."",
            ""nextNodeId"": null
        }
    },
    ""participantIds"": [],
    ""requiredPredicates"": []
}";

            string path = GetSelectedPath() + "/NewConversation.json";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("Assets/Create/GimGim/Conversation System/Sample Conversation JSON", priority = 101)]
        public static void CreateSampleConversationJson() {
            string json = @"{
    ""id"": ""sample_conversation"",
    ""name"": ""Sample Conversation"",
    ""description"": ""A sample conversation demonstrating various node types."",
    ""startNodeId"": ""greeting"",
    ""nodes"": {
        ""greeting"": {
            ""type"": ""text"",
            ""id"": ""greeting"",
            ""speakerId"": ""npc"",
            ""expression"": ""happy"",
            ""text"": ""Hello there, traveler! [pause:0.3]How can I help you today?"",
            ""nextNodeId"": ""main_choice""
        },
        ""main_choice"": {
            ""type"": ""choice"",
            ""id"": ""main_choice"",
            ""promptText"": ""What would you like to do?"",
            ""choices"": [
                {
                    ""id"": ""ask_quest"",
                    ""text"": ""Do you have any quests?"",
                    ""nextNodeId"": ""quest_response""
                },
                {
                    ""id"": ""ask_shop"",
                    ""text"": ""I'd like to see your wares."",
                    ""nextNodeId"": ""shop_response""
                },
                {
                    ""id"": ""leave"",
                    ""text"": ""Goodbye."",
                    ""nextNodeId"": ""farewell""
                }
            ]
        },
        ""quest_response"": {
            ""type"": ""text"",
            ""id"": ""quest_response"",
            ""speakerId"": ""npc"",
            ""expression"": ""thinking"",
            ""text"": ""Hmm, let me think... [pause:0.5]Actually, I do have something!"",
            ""nextNodeId"": ""farewell""
        },
        ""shop_response"": {
            ""type"": ""event"",
            ""id"": ""shop_response"",
            ""commands"": [
                {
                    ""commandType"": ""triggerEvent"",
                    ""parameters"": {
                        ""eventName"": ""OpenShop"",
                        ""shopId"": ""general_store""
                    }
                }
            ],
            ""nextNodeId"": null
        },
        ""farewell"": {
            ""type"": ""text"",
            ""id"": ""farewell"",
            ""speakerId"": ""npc"",
            ""expression"": ""happy"",
            ""text"": ""Safe travels, friend!"",
            ""nextNodeId"": null
        }
    },
    ""participantIds"": [""npc""],
    ""requiredPredicates"": []
}";

            string path = GetSelectedPath() + "/SampleConversation.json";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private static string GetSelectedPath() {
            string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path)) {
                    path = System.IO.Path.GetDirectoryName(path);
                }
                break;
            }

            return path;
        }
    }
}
#endif