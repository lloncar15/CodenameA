using UnityEngine;

namespace GimGim.AudioManagement {
    /// <summary>
    /// Generates a waveform <see cref="Texture2D"/> for an <see cref="AudioClipData"/> instance.
    /// The texture is written directly onto <see cref="AudioClipData.waveformTexture"/>.
    /// </summary>
    public class AudioClipVisualizer {
        private int _width;
        private int _height;
        private StereoMode _stereoMode;

        public AudioClipVisualizer(int width = 1024, int height = 256, StereoMode mode = StereoMode.Mono) {
            _width  = width;
            _height = height;
            _stereoMode = mode;
        }
        
        public void SetStereoMode(StereoMode mode) {
            _stereoMode = mode;
        }

        public void SetResolution(int width, int height) {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Depending on the StereoMode, creates a Texture2D for an AudioClipVisual.
        /// </summary>
        /// <param name="clipData">The AudioClipVisual to create the visual for.</param>
        /// <param name="waveformColor">The color of the waveform.</param>
        /// <param name="backgroundColor">The color of the background.</param>
        public void GenerateTexture(AudioClipData clipData, Color waveformColor, Color backgroundColor) {
            if (!clipData.originalClip)
                return;
            
            if (clipData.waveformTexture)
                Object.Destroy(clipData.waveformTexture);
            
            Texture2D texture = new(_width, _height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[_width * _height];

            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = backgroundColor;
            }
            
            AudioClip clip  = clipData.originalClip;
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            if (clip.channels == 2 && _stereoMode == StereoMode.Stereo) {
                DrawStereoWaveform(pixels, samples, waveformColor);
            }
            else {
                DrawMonoWaveform(pixels, samples, clip.channels, waveformColor);
            }
            
            texture.SetPixels(pixels);
            texture.Apply();

            clipData.waveformTexture = texture;
        } 
        
        /// <summary>
        /// Fills the pixel data with the appropriate color depending on the samples in stereo mode.
        /// </summary>
        /// <param name="pixels">Pixels array from which the Texture will be drawn.</param>
        /// <param name="samples">Samples from the audio clip.</param>
        /// <param name="waveformColor">The color to draw waveform pixels.</param>
        private void DrawStereoWaveform(Color[] pixels, float[] samples, Color waveformColor) {
            int samplesPerPixel = samples.Length / (_width * 2);
            if (samplesPerPixel < 1) samplesPerPixel = 1;
            
            int quarterHeight = _height / 4;
            int leftChannelCenter = _height / 4;
            int rightChannelCenter = 3 * _height / 4;
            
            for (int x = 0; x < _width; x++) {
                int sampleIndex = x * samplesPerPixel * 2;
                
                if (sampleIndex >= samples.Length)
                    break;
                
                float leftMin = 0f;
                float leftMax = 0f;
                float rightMin = 0f;
                float rightMax = 0f;
                
                // Peak sampling for both channels
                for (int i = 0; i < samplesPerPixel && (sampleIndex + i * 2 + 1) < samples.Length; i++) {
                    float leftSample = samples[sampleIndex + i * 2];
                    float rightSample = samples[sampleIndex + i * 2 + 1];
                    
                    if (leftSample < leftMin) leftMin = leftSample;
                    if (leftSample > leftMax) leftMax = leftSample;
                    if (rightSample < rightMin) rightMin = rightSample;
                    if (rightSample > rightMax) rightMax = rightSample;
                }
                
                // Draw left channel
                int leftYMin = leftChannelCenter + Mathf.RoundToInt(leftMin * quarterHeight);
                int leftYMax = leftChannelCenter + Mathf.RoundToInt(leftMax * quarterHeight);
                leftYMin = Mathf.Clamp(leftYMin, 0, _height / 2 - 1);
                leftYMax = Mathf.Clamp(leftYMax, 0, _height / 2 - 1);
                
                for (int y = leftYMin; y <= leftYMax; y++) {
                    pixels[y * _width + x] = waveformColor;
                }
                
                // Draw right channel
                int rightYMin = rightChannelCenter + Mathf.RoundToInt(rightMin * quarterHeight);
                int rightYMax = rightChannelCenter + Mathf.RoundToInt(rightMax * quarterHeight);
                rightYMin = Mathf.Clamp(rightYMin, _height / 2, _height - 1);
                rightYMax = Mathf.Clamp(rightYMax, _height / 2, _height - 1);
                
                for (int y = rightYMin; y <= rightYMax; y++) {
                    pixels[y * _width + x] = waveformColor;
                }
            }
        }
        
        /// <summary>
        /// Fills the pixel data with the appropriate color depending on the samples in mono mode.
        /// </summary>
        /// <param name="pixels">Pixels array from which the Texture will be drawn.</param>
        /// <param name="samples">Samples from the audio clip.</param>
        /// <param name="channels">Number of channels.</param>
        /// <param name="waveformColor">The color to draw waveform pixels.</param>
        private void DrawMonoWaveform(Color[] pixels, float[] samples, int channels, Color waveformColor) {
            int samplesPerPixel = samples.Length / (_width * channels);
            if (samplesPerPixel < 1) samplesPerPixel = 1;
        
            int centerY = _height / 2;
            int halfHeight = _height / 2;
        
            for (int x = 0; x < _width; x++) {
                int sampleIndex = x * samplesPerPixel * channels;
            
                if (sampleIndex >= samples.Length)
                    break;
            
                float min = 0f;
                float max = 0f;
            
                // Peak sampling
                for (int i = 0; i < samplesPerPixel && (sampleIndex + i * channels) < samples.Length; i++) {
                    float sampleValue = 0f;
                
                    // Average all channels for mono visualization
                    for (int ch = 0; ch < channels; ch++) {
                        int idx = sampleIndex + i * channels + ch;
                        if (idx < samples.Length)
                        {
                            sampleValue += samples[idx];
                        }
                    }
                    sampleValue /= channels;
                
                    if (sampleValue < min) min = sampleValue;
                    if (sampleValue > max) max = sampleValue;
                }
            
                int yMin = centerY + Mathf.RoundToInt(min * halfHeight);
                int yMax = centerY + Mathf.RoundToInt(max * halfHeight);
            
                yMin = Mathf.Clamp(yMin, 0, _height - 1);
                yMax = Mathf.Clamp(yMax, 0, _height - 1);
            
                // Draw vertical line from min to max
                for (int y = yMin; y <= yMax; y++) {
                    pixels[y * _width + x] = waveformColor;
                }
            }
        }

        /// <summary>
        /// Creates an empty texture with the set background color.
        /// </summary>
        /// <param name="backgroundColor">The color of the texture.</param>
        /// <returns>An empty texture colored with the set color.</returns>
        private Texture2D CreateEmptyTexture(Color backgroundColor) {
            Texture2D texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[_width * _height];

            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = backgroundColor;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
    }

    public enum StereoMode {
        Mono,
        Stereo
    }
}
