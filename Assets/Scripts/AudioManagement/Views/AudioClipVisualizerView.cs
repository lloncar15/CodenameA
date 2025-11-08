using UnityEngine;
using UnityEngine.UI;

namespace GimGim.AudioManagement {
    [RequireComponent(typeof(RawImage))]
    public class AudioClipVisualizerView : MonoBehaviour {
        [Header("Visualization Settings")]
        [SerializeField] private int textureWidth = 1024;
        [SerializeField] private int textureHeight = 256;
        [SerializeField] private Color waveformColor = new Color(0.3f, 0.5f, 1f, 1f);
        [SerializeField] private Color backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] private StereoMode stereoMode = StereoMode.Mono;
        [SerializeField] public AudioClip audioClip;
        
        private RawImage _rawImage;
        private AudioClipVisualizer _visualizer;
        private Texture2D _currentTexture;

        private void Awake() {
            _rawImage = GetComponent<RawImage>();
            _visualizer = new AudioClipVisualizer(textureWidth, textureHeight);
        }
        
        public void SetAudioClip(AudioClip clip) {
            _visualizer.SetAudioClip(clip);
        }

        public void SetStereoMode(StereoMode mode) {
            stereoMode = mode;
            _visualizer.SetStereoMode(stereoMode);
        }
        
        public void SetWaveformColor(Color color) {
            waveformColor = color;
        }
    
        public void SetBackgroundColor(Color color) {
            backgroundColor = color;
        }
    
        public void SetResolution(int width, int height) {
            textureWidth = width;
            textureHeight = height;
            _visualizer.SetResolution(width, height);
        }

        /// <summary>
        /// Deletes the current texture if it exists and generates a texture from the audio clip in the visualizer.
        /// </summary>
        public void RefreshVisualization() {
            if (_currentTexture) {
                Destroy(_currentTexture);
            }

            _currentTexture = _visualizer.GenerateWaveformTexture(waveformColor, backgroundColor);
            _rawImage.texture = _currentTexture;
        }

        private void OnDestroy() {
            if (_currentTexture)
                Destroy(_currentTexture);
        }
    }
}