#if UNITY_EDITOR
using UnityEditor;

namespace GimGim.Player.Controller {
    [CustomEditor(typeof(PlayerMovement))]
    public class PlayerMovementEditor : Editor {
        private Editor _settingsEditor;

        public override void OnInspectorGUI() {
            PlayerMovement playerMovement = (PlayerMovement)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings"));
            
            if (playerMovement.settings) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Movement Settings", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                CreateCachedEditor(playerMovement.settings, null, ref _settingsEditor);
                
                _settingsEditor.OnInspectorGUI();
                
                EditorGUILayout.Space(10);

                EditorGUILayout.EndVertical();
            }
            else {
                EditorGUILayout.HelpBox(
                    "No Movement Settings assigned. Create or assign a MovementSettings ScriptableObject.",
                    MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(10);
        }

        private void OnDisable() {
            if (_settingsEditor) {
                DestroyImmediate(_settingsEditor);
            }
        }
    }
}
#endif