#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Custom inspector for CharacterDatabase.
    /// </summary>
    [CustomEditor(typeof(CharacterDatabase))]
    public class CharacterDatabaseEditor : UnityEditor.Editor {
        private CharacterDatabase _database;
        private bool _showCharacters = true;
        private Vector2 _scrollPosition;

        private void OnEnable() {
            _database = (CharacterDatabase)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Character Database", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Default inspector
            DrawPropertiesExcluding(serializedObject, "m_Script");

            EditorGUILayout.Space();

            // Statistics
            DrawStatistics();

            EditorGUILayout.Space();

            // Actions
            DrawActions();

            EditorGUILayout.Space();

            // Character list
            DrawCharacterList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStatistics() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);

            _database.Initialize();
            
            int characterCount = 0;
            int totalEmotions = 0;

            foreach (CharacterDefinition character in _database.GetAllCharacters()) {
                if (character) {
                    characterCount++;
                    totalEmotions += character.Emotions?.Count ?? 0;
                }
            }

            EditorGUILayout.LabelField($"Characters: {characterCount}");
            EditorGUILayout.LabelField($"Total Emotions: {totalEmotions}");

            EditorGUILayout.EndVertical();
        }

        private void DrawActions() {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Initialize")) {
                _database.Initialize();
            }

            if (GUILayout.Button("Reload")) {
                _database.Reload();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawCharacterList() {
            _showCharacters = EditorGUILayout.Foldout(_showCharacters, "Loaded Characters", true);

            if (!_showCharacters) {
                return;
            }

            _database.Initialize();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(200));

            foreach (CharacterDefinition character in _database.GetAllCharacters()) {
                if (!character) {
                    continue;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                // Portrait preview
                if (character.DefaultPortrait) {
                    GUILayout.Label(AssetPreview.GetAssetPreview(character.DefaultPortrait), GUILayout.Width(40), GUILayout.Height(40));
                }
                else {
                    GUILayout.Label("", GUILayout.Width(40), GUILayout.Height(40));
                }

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(character.DisplayName, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"ID: {character.CharacterId}");
                EditorGUILayout.LabelField($"Emotions: {character.Emotions?.Count ?? 0}");
                EditorGUILayout.EndVertical();

                // Color preview
                Rect colorRect = GUILayoutUtility.GetRect(20, 40, GUILayout.Width(20));
                EditorGUI.DrawRect(colorRect, character.CharacterColor);

                if (GUILayout.Button("Select", GUILayout.Width(50))) {
                    Selection.activeObject = character;
                    EditorGUIUtility.PingObject(character);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif