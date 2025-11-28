using UnityEngine;

namespace GimGim.ConversationSystem.Testing {
    /// <summary>
    /// Simple test script to start a conversation with a key press.
    /// </summary>
    public class SimpleConversationTest : MonoBehaviour {
        [SerializeField]
        private ConversationController controller;

        [SerializeField]
        private string conversationId = "test_conversation";

        [SerializeField]
        private KeyCode startKey = KeyCode.T;

        [SerializeField]
        private KeyCode skipKey = KeyCode.Return;

        private void Update() {
            if (!controller) return;

            // Start conversation
            if (UnityEngine.Input.GetKeyDown(startKey) && !controller.IsRunning) {
                StartConversation();
            }

            // Skip typewriter
            if (UnityEngine.Input.GetKeyDown(skipKey) && controller.IsRunning) {
                controller.SkipTypewriter();
            }
        }

        private async void StartConversation() {
            Debug.Log($"Starting conversation: {conversationId}");
            
            try {
                await controller.StartConversationAsync(conversationId);
                Debug.Log("Conversation completed!");
            }
            catch (System.Exception ex) {
                Debug.LogError($"Conversation error: {ex.Message}");
            }
        }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            
            if (!controller) {
                GUILayout.Label("No ConversationController assigned!");
            }
            else if (controller.IsRunning) {
                GUILayout.Label("Conversation running...");
                GUILayout.Label($"Press {skipKey} to skip typewriter");
                GUILayout.Label("Press Space to advance");
            }
            else {
                GUILayout.Label($"Press {startKey} to start conversation");
            }
            
            GUILayout.EndArea();
        }
    }
}