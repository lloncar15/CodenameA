using System.Collections;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Handles UI animations for conversation elements.
    /// </summary>
    public class ConversationUIAnimator : MonoBehaviour {
        [Header("Animation Settings")]
        [SerializeField]
        private float fadeInDuration = 0.3f;

        [SerializeField]
        private float fadeOutDuration = 0.2f;

        [SerializeField]
        private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Slide Animation")]
        [SerializeField]
        private bool useSlideAnimation = true;

        [SerializeField]
        private Vector2 slideOffset = new Vector2(0f, -100f);

        [SerializeField]
        private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Scale Animation")]
        [SerializeField]
        private bool useScaleAnimation = false;

        [SerializeField]
        private Vector3 scaleFrom = new Vector3(0.8f, 0.8f, 1f);

        [SerializeField]
        private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// Animates showing a UI element.
        /// </summary>
        /// <param name="target">The target RectTransform.</param>
        /// <param name="canvasGroup">The target CanvasGroup.</param>
        public Coroutine AnimateShow(RectTransform target, CanvasGroup canvasGroup) {
            return StartCoroutine(ShowCoroutine(target, canvasGroup));
        }

        /// <summary>
        /// Animates hiding a UI element.
        /// </summary>
        /// <param name="target">The target RectTransform.</param>
        /// <param name="canvasGroup">The target CanvasGroup.</param>
        public Coroutine AnimateHide(RectTransform target, CanvasGroup canvasGroup) {
            return StartCoroutine(HideCoroutine(target, canvasGroup));
        }

        /// <summary>
        /// Show animation coroutine.
        /// </summary>
        private IEnumerator ShowCoroutine(RectTransform target, CanvasGroup canvasGroup) {
            if (!target && !canvasGroup) {
                yield break;
            }

            Vector2 originalPosition = target?.anchoredPosition ?? Vector2.zero;
            Vector3 originalScale = target?.localScale ?? Vector3.one;

            float elapsed = 0f;

            while (elapsed < fadeInDuration) {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / fadeInDuration;

                // Fade
                if (canvasGroup) {
                    canvasGroup.alpha = fadeCurve.Evaluate(t);
                }

                // Slide
                if (useSlideAnimation && target) {
                    float slideT = slideCurve.Evaluate(t);
                    target.anchoredPosition = Vector2.Lerp(originalPosition + slideOffset, originalPosition, slideT);
                }

                // Scale
                if (useScaleAnimation && target) {
                    float scaleT = scaleCurve.Evaluate(t);
                    target.localScale = Vector3.Lerp(scaleFrom, originalScale, scaleT);
                }

                yield return null;
            }

            // Ensure final values
            if (canvasGroup) {
                canvasGroup.alpha = 1f;
            }

            if (target) {
                target.anchoredPosition = originalPosition;
                target.localScale = originalScale;
            }
        }

        /// <summary>
        /// Hide animation coroutine.
        /// </summary>
        private IEnumerator HideCoroutine(RectTransform target, CanvasGroup canvasGroup) {
            if (!target && !canvasGroup) {
                yield break;
            }

            Vector2 originalPosition = target?.anchoredPosition ?? Vector2.zero;
            Vector3 originalScale = target?.localScale ?? Vector3.one;

            float elapsed = 0f;

            while (elapsed < fadeOutDuration) {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / fadeOutDuration;

                // Fade
                if (canvasGroup) {
                    canvasGroup.alpha = fadeCurve.Evaluate(1f - t);
                }

                // Slide
                if (useSlideAnimation && target) {
                    float slideT = slideCurve.Evaluate(t);
                    target.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + slideOffset, slideT);
                }

                // Scale
                if (useScaleAnimation && target) {
                    float scaleT = scaleCurve.Evaluate(t);
                    target.localScale = Vector3.Lerp(originalScale, scaleFrom, scaleT);
                }

                yield return null;
            }

            // Ensure final values
            if (canvasGroup) {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Pulses a UI element.
        /// </summary>
        public Coroutine Pulse(RectTransform target, float scale = 1.1f, float duration = 0.2f) {
            return StartCoroutine(PulseCoroutine(target, scale, duration));
        }

        /// <summary>
        /// Pulse animation coroutine.
        /// </summary>
        private IEnumerator PulseCoroutine(RectTransform target, float scale, float duration) {
            if (!target) {
                yield break;
            }

            Vector3 originalScale = target.localScale;
            Vector3 pulseScale = originalScale * scale;
            float halfDuration = duration / 2f;

            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration) {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(originalScale, pulseScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration) {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(pulseScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        /// <summary>
        /// Bounces a UI element.
        /// </summary>
        public Coroutine Bounce(RectTransform target, float height = 20f, float duration = 0.3f) {
            return StartCoroutine(BounceCoroutine(target, height, duration));
        }

        /// <summary>
        /// Bounce animation coroutine.
        /// </summary>
        private IEnumerator BounceCoroutine(RectTransform target, float height, float duration) {
            if (!target) {
                yield break;
            }

            Vector2 originalPosition = target.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float bounce = Mathf.Sin(t * Mathf.PI) * height;
                target.anchoredPosition = originalPosition + Vector2.up * bounce;
                yield return null;
            }

            target.anchoredPosition = originalPosition;
        }
    }
}