using System;
using UnityEngine;

namespace GimGim.AudioManagement {
    [Serializable]
    public class AudioClipVisual {
        [SerializeField]
        public AudioClip clip;
        [SerializeField]
        public Texture2D texture;
    }
}