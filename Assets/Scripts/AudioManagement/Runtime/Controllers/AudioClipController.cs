using System;
using UnityEngine;

namespace GimGim.AudioManagement {
    public class AudioClipController : MonoBehaviour {
        [Header("Visualization Settings")]
        [SerializeField] private int textureWidth = 1024;
        [SerializeField] private int textureHeight = 256;
        [SerializeField] private Color waveformColor = new(0.3f, 0.5f, 1f);
        [SerializeField] private Color backgroundColor = new(0.15f, 0.15f, 0.15f);
        [SerializeField] private StereoMode stereoMode = StereoMode.Mono;
        
        [Header("Recorder Settings")]
        [SerializeField] private int maxClipLength = 3;
        
        [Header("Playback")]
        [SerializeField] private AudioSource previewSource;
        
        public event Action<AudioClipData> ClipDataChanged;
        public event Action<AudioClipData> TrimChanged;
        
        private AudioClipVisualizer _visualizer;
        private AudioClipRecorder _recorder;
        private AudioClipProvider _provider;

        private float _previewEndTime;

        private const float CLIP_LENGTH_THRESHOLD = 0.01f;
        
        private void Awake() {
            InitializeVisualizer();
            InitializeRecorder();
            InitializeProvider();
        }
        
        private void InitializeVisualizer() {
            _visualizer = new AudioClipVisualizer(textureWidth, textureHeight, stereoMode);
        }

        private void InitializeRecorder() {
            _recorder = new AudioClipRecorder(maxClipLength);
            _recorder.Initialize();
        }

        private void InitializeProvider() {
            _provider = new AudioClipProvider();
        }

        private void OnEnable() {
            _recorder.RecordingFinished += OnRecordingFinished;
        }

        private void OnDisable() {
            _recorder.RecordingFinished -= OnRecordingFinished;
        }

        private void Update() {
            StopPreviewPlayback();
        }

        /// <summary>
        /// Called by <see cref="AudioClipRecorderView"/> when a new clip has been recorded.
        /// Creates a new <see cref="AudioClipData"/>, generates its waveform texture,
        /// and notifies listeners.
        /// </summary>
        /// <param name="clip">The freshly recorded <see cref="AudioClip"/>.</param>
        public void OnRecordingFinished(AudioClip clip) {
            AudioClipData clipData = new(clip);
            _visualizer.GenerateTexture(clipData, waveformColor, backgroundColor);
            _provider.ClipData = clipData;
            ClipDataChanged?.Invoke(clipData);
        }
        
        /// <summary>
        /// Updates the trim range on the current clip data and notifies listeners.
        /// </summary>
        /// <param name="start">Normalized trim start (0–1).</param>
        /// <param name="end">Normalized trim end (0–1).</param>
        public void SetTrim(float start, float end) {
            if (ClipData == null)
                return;
            
            ClipData.trimStart = Mathf.Clamp01(start);
            ClipData.trimEnd   = Mathf.Clamp01(end);
            TrimChanged?.Invoke(ClipData);
        }

        public void PlayTrimmedAudioClip(AudioClipData clipData) {
            PlayClip(clipData.trimStart, clipData.trimEnd);
        }
        
        /// <summary>
        /// Starts preview playback from the trim start position.
        /// </summary>
        public void PlayWholeClip() {
            if (ClipData?.originalClip == null) 
                return;
            
            PlayClip(ClipData.trimStart, ClipData.trimEnd);
        }
        
        /// <summary>
        /// Starts preview playback from a normalized position in the clip.
        /// </summary>
        /// <param name="start">Start position in the clip (0–1).</param>
        /// <param name="end">End position in the clip (0–1).</param>
        public void PlayClip(float start, float end) {
            if (ClipData?.originalClip == null) 
                return;

            if (IsPlaying) {
                StopPlayback();
                return;
            }

            AudioClip originalClip = ClipData.originalClip;
            float clipLength = originalClip.length;
            
            float startTime = start * clipLength;
            _previewEndTime = end * clipLength;
            
            previewSource.clip = ClipData.originalClip;
            previewSource.Play();
            previewSource.time = Mathf.Clamp(startTime, 0f, ClipData.originalClip.length - CLIP_LENGTH_THRESHOLD);
        }
        
        /// <summary>
        /// Checks if there is a preview playing and stops it if it went past the end time.
        /// </summary>
        private void StopPreviewPlayback() {
            if (!IsPlaying)
                return;
            
            if (ClipData?.originalClip == null) 
                return;

            if (previewSource.time < _previewEndTime)
                return;
            
            StopPlayback();
        }
        
        /// <summary>
        /// Stops preview playback.
        /// </summary>
        public void StopPlayback() {
            previewSource.Stop();
        }
        
        public bool IsPlaying => previewSource.isPlaying;
        public AudioClipRecorder Recorder => _recorder;
        public AudioClipData ClipData => _provider.ClipData;
    }
}