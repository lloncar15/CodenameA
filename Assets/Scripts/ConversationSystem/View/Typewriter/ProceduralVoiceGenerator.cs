using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Generates procedural voice sounds as a fallback when no audio clips are available.
    /// Creates simple sine wave blips.
    /// </summary>
    public static class ProceduralVoiceGenerator {
        private static AudioClip _cachedClip;
        private static float _cachedFrequency = -1f;

        /// <summary>
        /// Generates a simple blip sound.
        /// </summary>
        /// <param name="frequency">Base frequency in Hz.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="sampleRate">Sample rate.</param>
        /// <returns>An AudioClip containing the generated sound.</returns>
        public static AudioClip GenerateBlip(float frequency = 440f, float duration = 0.05f, int sampleRate = 44100) {
            // Cache the clip if same frequency (for performance)
            if (_cachedClip != null && Mathf.Approximately(_cachedFrequency, frequency)) {
                return _cachedClip;
            }

            int sampleCount = Mathf.RoundToInt(duration * sampleRate);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++) {
                float t = (float)i / sampleRate;
                float envelope = GetEnvelope(i, sampleCount);
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope;
            }

            AudioClip clip = AudioClip.Create("ProceduralBlip", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);

            _cachedClip = clip;
            _cachedFrequency = frequency;

            return clip;
        }

        /// <summary>
        /// Generates an envelope for smooth attack/decay.
        /// </summary>
        private static float GetEnvelope(int sample, int totalSamples) {
            float position = (float)sample / totalSamples;
            
            // Quick attack, longer decay
            float attack = 0.1f;
            float decay = 0.9f;

            if (position < attack) {
                return position / attack;
            }
            else {
                float decayPosition = (position - attack) / decay;
                return 1f - decayPosition;
            }
        }

        /// <summary>
        /// Generates multiple blips at different frequencies.
        /// </summary>
        /// <param name="count">Number of clips to generate.</param>
        /// <param name="baseFrequency">Base frequency.</param>
        /// <param name="frequencyVariation">Frequency variation range.</param>
        /// <returns>Array of generated AudioClips.</returns>
        public static AudioClip[] GenerateBlipSet(int count = 5, float baseFrequency = 440f, float frequencyVariation = 100f) {
            AudioClip[] clips = new AudioClip[count];

            for (int i = 0; i < count; i++) {
                float freq = baseFrequency + (i - count / 2f) * (frequencyVariation / count);
                clips[i] = GenerateBlip(freq);
            }

            return clips;
        }

        /// <summary>
        /// Clears the cached clip.
        /// </summary>
        public static void ClearCache() {
            if (_cachedClip != null) {
                Object.Destroy(_cachedClip);
                _cachedClip = null;
            }
            _cachedFrequency = -1f;
        }
    }
}