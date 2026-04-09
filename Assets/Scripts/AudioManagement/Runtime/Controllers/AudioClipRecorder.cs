using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.AudioManagement {
    /// <summary>
    /// Handles microphone recording. Fires <see cref="RecordingFinished"/> with the
    /// captured <see cref="AudioClip"/> when recording stops.
    /// </summary>
    [Serializable]
    public class AudioClipRecorder {
        [SerializeField] private List<string> microphones = new();
        [SerializeField] private string currentMicrophone;

        private int _maxClipLength;
        private AudioClip _recordedClip;

        public event Action<AudioClip> RecordingFinished;

        public AudioClipRecorder(int maxClipLength) {
            _maxClipLength = maxClipLength;
        }
        
        /// <summary>
        /// Populates the microphone list and selects the first available device.
        /// </summary>
        public void Initialize() {
            microphones.Clear();
            
            foreach (string device in Microphone.devices) {
                microphones.Add(device);
            }

            if (microphones.Count > 0) {
                string microphone = microphones[0];
                SetCurrentMicrophone(microphone);
            }
        }

        private void SetCurrentMicrophone(string device) {
            currentMicrophone = device;
        }

        /// <summary>
        /// Starts recording from <see cref="currentMicrophone"/>.
        /// Does nothing if no microphone is available.
        /// </summary>
        public void StartRecording() {
            if (currentMicrophone == null)
                return;
            
            _recordedClip = Microphone.Start(currentMicrophone, 
                false, 
                _maxClipLength,
                AudioSettings.outputSampleRate);
        }

        /// <summary>
        /// Stops recording and raises <see cref="RecordingFinished"/>.
        /// Does nothing if no microphone is recording.
        /// </summary>
        public void StopRecording() {
            if (currentMicrophone == null)
                return;
            
            if (!Microphone.IsRecording(currentMicrophone))
                return;
            
            Microphone.End(currentMicrophone);
            OnStoppedRecording();
        }

        public void OnStoppedRecording() {
            RecordingFinished?.Invoke(_recordedClip);
        }
        
        public bool IsRecording() {
            return currentMicrophone != null && Microphone.IsRecording(currentMicrophone);
        }
    }
}