using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Controls text effects like shake, wave, rainbow, etc.
    /// This is a placeholder for future text animation effects.
    /// </summary>
    public class TextEffectController : MonoBehaviour {
        [SerializeField]
        private TMP_Text targetText;

        private readonly List<TextEffect> _activeEffects = new List<TextEffect>();

        /// <summary>
        /// Adds a text effect.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        public void AddEffect(TextEffect effect) {
            if (effect != null) {
                _activeEffects.Add(effect);
            }
        }

        /// <summary>
        /// Removes a text effect.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        public void RemoveEffect(TextEffect effect) {
            _activeEffects.Remove(effect);
        }

        /// <summary>
        /// Clears all effects.
        /// </summary>
        public void ClearEffects() {
            _activeEffects.Clear();
        }

        private void Update() {
            if (!targetText || _activeEffects.Count == 0) {
                return;
            }

            // Force mesh update to get latest vertex data
            targetText.ForceMeshUpdate();

            TMP_TextInfo textInfo = targetText.textInfo;
            if (textInfo == null || textInfo.characterCount == 0) {
                return;
            }

            // Apply all active effects
            foreach (TextEffect effect in _activeEffects) {
                effect.Apply(textInfo, Time.time);
            }

            // Update mesh
            for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                targetText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }

    /// <summary>
    /// Base class for text effects.
    /// </summary>
    public abstract class TextEffect {
        /// <summary>
        /// The start character index.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// The end character index (-1 for rest of text).
        /// </summary>
        public int EndIndex { get; set; } = -1;

        /// <summary>
        /// Applies the effect to the text mesh.
        /// </summary>
        /// <param name="textInfo">The TMP text info.</param>
        /// <param name="time">Current time for animation.</param>
        public abstract void Apply(TMP_TextInfo textInfo, float time);

        /// <summary>
        /// Gets the actual end index.
        /// </summary>
        protected int GetEndIndex(TMP_TextInfo textInfo) {
            return EndIndex < 0 ? textInfo.characterCount - 1 : Mathf.Min(EndIndex, textInfo.characterCount - 1);
        }
    }

    /// <summary>
    /// Shake text effect.
    /// </summary>
    public class ShakeTextEffect : TextEffect {
        public float Intensity { get; set; } = 2f;
        public float Speed { get; set; } = 20f;

        public override void Apply(TMP_TextInfo textInfo, float time) {
            int end = GetEndIndex(textInfo);

            for (int i = StartIndex; i <= end; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) {
                    continue;
                }

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                float offsetX = Mathf.Sin(time * Speed + i) * Intensity;
                float offsetY = Mathf.Cos(time * Speed + i * 1.5f) * Intensity;
                Vector3 offset = new Vector3(offsetX, offsetY, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }
        }
    }

    /// <summary>
    /// Wave text effect.
    /// </summary>
    public class WaveTextEffect : TextEffect {
        public float Amplitude { get; set; } = 5f;
        public float Frequency { get; set; } = 2f;
        public float Speed { get; set; } = 5f;

        public override void Apply(TMP_TextInfo textInfo, float time) {
            int end = GetEndIndex(textInfo);

            for (int i = StartIndex; i <= end; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) {
                    continue;
                }

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                float offsetY = Mathf.Sin(time * Speed + i * Frequency) * Amplitude;
                Vector3 offset = new Vector3(0, offsetY, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }
        }
    }
}