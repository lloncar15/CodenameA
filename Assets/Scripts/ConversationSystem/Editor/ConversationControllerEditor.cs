#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Custom inspector for ConversationController.
    /// </summary>
    [CustomEditor(typeof(ConversationController))]
    public class ConversationControllerEditor : UnityEditor.Editor {
        private ConversationController _controller;
        private string _testConversationId = "";
        private bool _showDebug = false;

        private void OnEnable() {
            _controller = (ConversationController)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            // Draw default inspector
            DrawDefaultInspector();

            EditorGUILayout.Space();

            // Runtime controls (only in play mode)
            if (Application.isPlaying) {
                DrawRuntimeControls();
            }
            else {
                DrawEditorControls();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEditorControls() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Open Conversation Browser")) {
                ConversationBrowserWindow.ShowWindow();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeControls() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);

            // Status
            EditorGUILayout.LabelField("Status:", _controller.IsRunning ? "Running" : "Idle");

            if (_controller.IsRunning) {
                EditorGUILayout.LabelField("Current Conversation:", _controller.Context?.CurrentConversationId ?? "N/A");
                EditorGUILayout.LabelField("Current Node:", _controller.Context?.CurrentNodeId ?? "N/A");

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Pause")) {
                    _controller.Pause();
                }
                if (GUILayout.Button("Resume")) {
                    _controller.Resume();
                }
                if (GUILayout.Button("Stop")) {
                    _controller.Stop();
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Skip Typewriter")) {
                    _controller.SkipTypewriter();
                }
            }
            else {
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Conversation ID:", GUILayout.Width(100));
                _testConversationId = EditorGUILayout.TextField(_testConversationId);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Start Conversation")) {
                    if (!string.IsNullOrEmpty(_testConversationId)) {
                        _ = _controller.StartConversationAsync(_testConversationId);
                    }
                }
            }

            EditorGUILayout.EndVertical();

            // Debug info
            EditorGUILayout.Space();
            _showDebug = EditorGUILayout.Foldout(_showDebug, "Debug Info", true);

            if (_showDebug && _controller.StateManager != null) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                ConversationStateData stateData = _controller.StateManager.GetStateData();
                
                EditorGUILayout.LabelField($"Bool Variables: {stateData.BoolVariables.Count}");
                EditorGUILayout.LabelField($"Int Variables: {stateData.IntVariables.Count}");
                EditorGUILayout.LabelField($"Float Variables: {stateData.FloatVariables.Count}");
                EditorGUILayout.LabelField($"String Variables: {stateData.StringVariables.Count}");
                EditorGUILayout.LabelField($"Node Visits: {stateData.NodeVisitCounts.Count}");

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save State")) {
                    _controller.SaveState();
                }
                if (GUILayout.Button("Load State")) {
                    _controller.LoadState();
                }
                if (GUILayout.Button("Reset State")) {
                    _controller.ResetState();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            // Force repaint while running
            if (_controller.IsRunning) {
                Repaint();
            }
        }
    }
}
#endif