using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.AudioManagement {
    /// <summary>
    /// Class which manages clip recording.
    /// </summary>
    [Serializable]
    public class AudioClipRecorder {
        [SerializeField] private List<string> microphones;
        [SerializeField] private string currentMicrophone;

        private int _maxClipLength;
        private AudioClip _recordedClip;

        private static event Action StartClipRecording;
        private static event Action<AudioClip> EndClipRecording;

        public AudioClipRecorder(int maxClipLength) {
            _maxClipLength = maxClipLength;
            microphones = new List<string>();
        }
        
        public void OnEnable() {
            InitializeMicrophones();

            if (microphones.Count > 0) {
                string microphone = microphones[0];
                SetCurrentMicrophone(microphone);
            }
        }

        private void InitializeMicrophones() {
            foreach (string device in Microphone.devices) {
                microphones.Add(device);
            }
        }

        private void SetCurrentMicrophone(string device) {
            currentMicrophone = device;
        }

        /// <summary>
        /// Starts recording if the current microphone exists for a given time.
        /// </summary>
        public void StartRecording() {
            if (currentMicrophone == null)
                return;
            
            _recordedClip = null;
            
            _recordedClip = Microphone.Start(currentMicrophone, 
                false, 
                _maxClipLength,
                AudioSettings.outputSampleRate);
            StartClipRecording?.Invoke();
        }

        /// <summary>
        /// Ends recording if the current microphone exists and is recording.
        /// </summary>
        public void StopRecording() {
            if (currentMicrophone == null)
                return;
            
            if (!Microphone.IsRecording(currentMicrophone))
                return;
            
            Microphone.End(currentMicrophone);
            EndClipRecording?.Invoke(_recordedClip);
        }
    }
}