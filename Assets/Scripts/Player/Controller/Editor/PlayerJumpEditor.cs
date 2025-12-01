#if UNITY_EDITOR
using UnityEditor;

namespace GimGim.Player.Controller {
    [CustomEditor(typeof(PlayerJump))]
    public class PlayerJumpEditor : Editor {
        private Editor _settingsEditor;

        public override void OnInspectorGUI() {
            PlayerJump playerJump = (PlayerJump)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings"));

            if (playerJump.settings) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Jump Settings", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                CreateCachedEditor(playerJump.settings, null, ref _settingsEditor);
                
                _settingsEditor.OnInspectorGUI();
                
                EditorGUILayout.Space(10);
            
                EditorGUILayout.EndVertical();
            }
            else {
                EditorGUILayout.HelpBox("No Jump Settings assigned. Create or assign a JumpSettings ScriptableObject.", MessageType.Warning);
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