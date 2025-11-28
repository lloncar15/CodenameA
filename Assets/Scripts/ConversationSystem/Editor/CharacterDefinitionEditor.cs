#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Custom inspector for CharacterDefinition.
    /// </summary>
    [CustomEditor(typeof(CharacterDefinition))]
    public class CharacterDefinitionEditor : UnityEditor.Editor {
        private CharacterDefinition _character;
        private bool _showEmotions = true;
        private bool _showVoice = true;
        private bool _showMetadata = false;

        private void OnEnable() {
            _character = (CharacterDefinition)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space();

            // Portrait preview
            DrawPortraitPreview();

            EditorGUILayout.Space();

            // Identity
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shortName"));

            EditorGUILayout.Space();

            // Appearance
            EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultPortrait"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterColor"));

            EditorGUILayout.Space();

            // Emotions
            _showEmotions = EditorGUILayout.Foldout(_showEmotions, "Emotions", true);
            if (_showEmotions) {
                DrawEmotions();
            }

            EditorGUILayout.Space();

            // Voice
            _showVoice = EditorGUILayout.Foldout(_showVoice, "Voice Settings", true);
            if (_showVoice) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("voiceSettings"), true);
            }

            EditorGUILayout.Space();

            // Metadata
            _showMetadata = EditorGUILayout.Foldout(_showMetadata, "Metadata", true);
            if (_showMetadata) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("metadata"), true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPortraitPreview() {
            EditorGUILayout.BeginHorizontal();

            // Default portrait
            if (_character.DefaultPortrait) {
                Texture2D preview = AssetPreview.GetAssetPreview(_character.DefaultPortrait);
                if (preview) {
                    GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
                }
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(_character.DisplayName ?? "(No Name)", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"ID: {_character.CharacterId ?? "(No ID)"}");
            EditorGUILayout.LabelField($"Emotions: {_character.Emotions?.Count ?? 0}");

            // Color preview
            Rect colorRect = GUILayoutUtility.GetRect(100, 20);
            EditorGUI.DrawRect(colorRect, _character.CharacterColor);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEmotions() {
            SerializedProperty emotionsProperty = serializedObject.FindProperty("emotions");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = 0; i < emotionsProperty.arraySize; i++) {
                SerializedProperty emotionProp = emotionsProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = emotionProp.FindPropertyRelative("emotionKey");
                SerializedProperty portraitProp = emotionProp.FindPropertyRelative("portrait");

                EditorGUILayout.BeginHorizontal();

                // Portrait preview
                Sprite portrait = portraitProp.objectReferenceValue as Sprite;
                if (portrait) {
                    Texture2D preview = AssetPreview.GetAssetPreview(portrait);
                    if (preview) {
                        GUILayout.Label(preview, GUILayout.Width(40), GUILayout.Height(40));
                    }
                }
                else {
                    GUILayout.Label("", GUILayout.Width(40), GUILayout.Height(40));
                }

                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(keyProp, GUIContent.none);
                EditorGUILayout.PropertyField(portraitProp, GUIContent.none);
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    emotionsProperty.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (i < emotionsProperty.arraySize - 1) {
                    EditorGUILayout.Space(5);
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Emotion")) {
                emotionsProperty.InsertArrayElementAtIndex(emotionsProperty.arraySize);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif