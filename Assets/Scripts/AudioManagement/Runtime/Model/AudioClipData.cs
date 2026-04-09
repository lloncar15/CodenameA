using System;
using UnityEngine;

namespace GimGim.AudioManagement {
    [Serializable]
    public class AudioClipData {
        public AudioClip originalClip;
        public Texture2D waveformTexture;

        [Range(0f, 1f)] public float trimStart = 0f;
        [Range(0f, 1f)] public float trimEnd = 1f;

        public AudioClipData(AudioClip clip) {
            originalClip = clip;
        }
    }
}