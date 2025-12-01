#if UNITY_EDITOR
using UnityEditor;

namespace GimGim.Player.Controller {
    [CustomEditor(typeof(PlayerDash))]
    public class PlayerDashEditor : Editor {
        private Editor _settingsEditor;

        public override void OnInspectorGUI()
        {
            PlayerDash playerDash = (PlayerDash)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings"));
            
            if (playerDash.settings) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Dash Settings", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                CreateCachedEditor(playerDash.settings, null, ref _settingsEditor);
                
                _settingsEditor.OnInspectorGUI();
            
                EditorGUILayout.EndVertical();
            }
            else {
                EditorGUILayout.HelpBox("No Dash Settings assigned. Create or assign a DashSettings ScriptableObject.", MessageType.Warning);
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(10);
        }

        private void OnDisable()
        {
            if (_settingsEditor) {
                DestroyImmediate(_settingsEditor);
            }
        }
    }
}
#endif