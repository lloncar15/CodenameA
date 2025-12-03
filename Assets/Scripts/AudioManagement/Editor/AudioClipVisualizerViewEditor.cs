#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GimGim.AudioManagement {
    [CustomEditor(typeof(AudioClipVisualizerView))]
    public class AudioClipVisualizerViewEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            AudioClipVisualizerView visualizerView = (AudioClipVisualizerView)target;
            
            EditorGUILayout.Space(10);

            if (!GUILayout.Button("Refresh Visualization", GUILayout.Height(30)))
                return;
            
            if (Application.isPlaying) {
                if (visualizerView.clipVisual.clip) {
                    visualizerView.RefreshVisualization(visualizerView.clipVisual);
                }
            }
            else {
                EditorUtility.DisplayDialog("Editor Mode", "Please enter Play Mode to refresh visualization", "OK");
            }
        }
    }
}
#endif