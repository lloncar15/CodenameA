using System;
using UnityEngine;

namespace GimGim.AudioManagement {
    public class AudioClipRecorderView : MonoBehaviour {
        [SerializeField] private AudioClipRecorder recorder;
        [SerializeField] private int maxClipLength = 3;

        private void OnEnable() {
            recorder = new AudioClipRecorder(maxClipLength);
            recorder.OnEnable();
        }

        private void Update() {
            
        }

        public void StartRecording() {
            recorder.StartRecording();
        }

        public void StopRecording() {
            recorder.StopRecording();
        }
    }
}