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
        //TODO: just for testing purposes, otherwise audio transform manager will provide the audio clip visual
        [SerializeField] public AudioClipVisual clipVisual;
        
        private RawImage _rawImage;
        private AudioClipVisualizer _visualizer;
        private Texture2D _currentTexture;

        private void Awake() {
            _rawImage = GetComponent<RawImage>();
            _visualizer = new AudioClipVisualizer(textureWidth, textureHeight);
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
        public void RefreshVisualization(AudioClipVisual clip) {
            if (_currentTexture) {
                Destroy(_currentTexture);
            }
            
            if (!clip.texture) {
                _visualizer.GenerateWaveformTextureForClipVisual(clip, waveformColor, backgroundColor);
            }

            _currentTexture = clip.texture;
            _rawImage.texture = _currentTexture;
        }

        private void OnDestroy() {
            if (_currentTexture)
                Destroy(_currentTexture);
        }
    }
}