#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Custom inspector for ConversationDatabase.
    /// </summary>
    [CustomEditor(typeof(ConversationDatabase))]
    public class ConversationDatabaseEditor : UnityEditor.Editor {
        private ConversationDatabase _database;
        private ConversationValidator _validator;
        private bool _showConversations = true;
        private Vector2 _scrollPosition;

        private void OnEnable() {
            _database = (ConversationDatabase)target;
            _validator = new ConversationValidator();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space();

            // Header
            EditorGUILayout.LabelField("Conversation Database", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Draw default inspector for conversation assets list
            DrawPropertiesExcluding(serializedObject, "m_Script");

            EditorGUILayout.Space();

            // Statistics
            DrawStatistics();

            EditorGUILayout.Space();

            // Actions
            DrawActions();

            EditorGUILayout.Space();

            // Conversation list
            DrawConversationList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStatistics() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);

            _database.Initialize();
            
            int conversationCount = 0;
            int totalNodes = 0;

            foreach (string id in _database.GetAllConversationIds()) {
                conversationCount++;
                ConversationData conv = _database.GetConversation(id);
                if (conv != null) {
                    totalNodes += conv.Nodes?.Count ?? 0;
                }
            }

            EditorGUILayout.LabelField($"Conversations: {conversationCount}");
            EditorGUILayout.LabelField($"Total Nodes: {totalNodes}");

            EditorGUILayout.EndVertical();
        }

        private void DrawActions() {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Initialize")) {
                _database.Initialize();
                Debug.Log($"ConversationDatabase: Loaded {_database.Count} conversations.");
            }

            if (GUILayout.Button("Reload")) {
                _database.Reload();
            }

            if (GUILayout.Button("Validate All")) {
                ValidateAll();
            }

            if (GUILayout.Button("Open Browser")) {
                ConversationBrowserWindow.ShowWindow();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawConversationList() {
            _showConversations = EditorGUILayout.Foldout(_showConversations, "Loaded Conversations", true);

            if (!_showConversations) {
                return;
            }

            _database.Initialize();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(200));

            foreach (string id in _database.GetAllConversationIds()) {
                ConversationData conv = _database.GetConversation(id);
                if (conv == null) {
                    continue;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(conv.Name ?? conv.Id, GUILayout.Width(150));
                EditorGUILayout.LabelField($"Nodes: {conv.Nodes?.Count ?? 0}", GUILayout.Width(80));

                if (GUILayout.Button("Validate", GUILayout.Width(60))) {
                    _validator.Validate(conv);
                    if (_validator.IsValid) {
                        Debug.Log($"Conversation '{conv.Id}' is valid.");
                    }
                    else {
                        Debug.LogWarning($"Conversation '{conv.Id}' has issues:\n{_validator.GetReport()}");
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void ValidateAll() {
            _database.Initialize();

            int errors = 0;
            int warnings = 0;
            int valid = 0;

            foreach (string id in _database.GetAllConversationIds()) {
                ConversationData conv = _database.GetConversation(id);
                if (conv == null) {
                    continue;
                }

                _validator.Validate(conv);

                if (_validator.HasErrors) {
                    errors++;
                    Debug.LogError($"Conversation '{id}' has errors:\n{_validator.GetReport()}");
                }
                else if (_validator.HasWarnings) {
                    warnings++;
                    Debug.LogWarning($"Conversation '{id}' has warnings:\n{_validator.GetReport()}");
                }
                else {
                    valid++;
                }
            }

            EditorUtility.DisplayDialog("Validation Complete",
                $"Valid: {valid}\nWith Warnings: {warnings}\nWith Errors: {errors}",
                "OK");
        }
    }
}
#endif