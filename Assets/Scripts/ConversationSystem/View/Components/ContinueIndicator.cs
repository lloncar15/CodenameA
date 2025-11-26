using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Animated indicator showing the player can continue.
    /// </summary>
    public class ContinueIndicator : MonoBehaviour {
        [Header("Animation")]
        [SerializeField]
        private AnimationType animationType = AnimationType.Bounce;

        [SerializeField]
        private float animationSpeed = 2f;

        [SerializeField]
        private float animationAmount = 10f;

        [Header("Fade")]
        [SerializeField]
        private bool useFade = true;

        [SerializeField]
        private float fadeSpeed = 3f;

        [SerializeField]
        private float minAlpha = 0.3f;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector3 _startPosition;
        private float _time;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_rectTransform != null) {
                _startPosition = _rectTransform.anchoredPosition;
            }
        }

        private void OnEnable() {
            _time = 0f;
        }

        private void Update() {
            _time += Time.unscaledDeltaTime;

            AnimatePosition();
            AnimateFade();
        }

        /// <summary>
        /// Animates the position.
        /// </summary>
        private void AnimatePosition() {
            if (!_rectTransform) {
                return;
            }

            float offset = 0f;

            switch (animationType) {
                case AnimationType.Bounce:
                    offset = Mathf.Abs(Mathf.Sin(_time * animationSpeed)) * animationAmount;
                    break;
                case AnimationType.Sine:
                    offset = Mathf.Sin(_time * animationSpeed) * animationAmount;
                    break;
                case AnimationType.Pulse:
                    offset = (Mathf.Sin(_time * animationSpeed) * 0.5f + 0.5f) * animationAmount;
                    break;
            }

            _rectTransform.anchoredPosition = _startPosition + Vector3.down * offset;
        }

        /// <summary>
        /// Animates the fade.
        /// </summary>
        private void AnimateFade() {
            if (!useFade || !_canvasGroup) {
                return;
            }

            float alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(_time * fadeSpeed) + 1f) * 0.5f);
            _canvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Resets the animation.
        /// </summary>
        public void ResetAnimation() {
            _time = 0f;
            
            if (_rectTransform != null) {
                _rectTransform.anchoredPosition = _startPosition;
            }

            if (_canvasGroup != null) {
                _canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// Animation types for the indicator.
        /// </summary>
        public enum AnimationType {
            Bounce,
            Sine,
            Pulse
        }
    }
}