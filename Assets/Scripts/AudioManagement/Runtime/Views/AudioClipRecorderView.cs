using UnityEngine;

namespace GimGim.AudioManagement {
    public class AudioClipRecorderView : MonoBehaviour {
        [SerializeField] private AudioClipController controller;

        private bool _wasRecording;

        private void Update() {
            bool isRecording = controller.Recorder.IsRecording();

            if (_wasRecording && !isRecording) {
                controller.Recorder.OnStoppedRecording();
            }

            _wasRecording = isRecording;
        }

        public void StartRecording() {
            controller.Recorder.StartRecording();
        }

        public void StopRecording() {
            _wasRecording = false;
            controller.Recorder.StopRecording();
        }
    }
}