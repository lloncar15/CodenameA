using UnityEngine;
using UnityEngine.UI;

namespace GimGim.AudioManagement {
    /// <summary>
    /// Displays the waveform texture and renders a trim overlay via a second
    /// stacked <see cref="RawImage"/>. Subscribes to <see cref="AudioClipController"/>
    /// events to stay in sync.
    /// Expects a child GameObject named "TrimOverlay" with a <see cref="RawImage"/>.
    /// </summary>
    public class AudioClipVisualizerView : MonoBehaviour {
        [Header("Visualization Settings")]
        [SerializeField] private AudioClipController controller;
        [SerializeField] private RawImage waveformImage;
        
        private Texture2D _overlayTexture;

        private void OnEnable() {
            controller.ClipDataChanged += OnClipDataChanged;
        }
        
        private void OnDisable() {
            controller.ClipDataChanged -= OnClipDataChanged;
        }

        private void OnDestroy() {
            if (_overlayTexture)
                Destroy(_overlayTexture);
        }

        private void OnClipDataChanged(AudioClipData clipData) {
            waveformImage.texture = clipData.waveformTexture;
            RefreshVisualization(clipData);
        }

        /// <summary>
        /// Deletes the current texture if it exists and generates a texture from the audio clip in the visualizer.
        /// </summary>
        public void RefreshVisualization(AudioClipData clip) {
            if (!clip.waveformTexture)
                return;
            
            if (_overlayTexture)
                Destroy(_overlayTexture);
            
            _overlayTexture = clip.waveformTexture;
            waveformImage.texture = _overlayTexture;
        }
    }
}