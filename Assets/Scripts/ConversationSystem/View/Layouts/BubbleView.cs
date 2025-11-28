using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Speech bubble style dialogue view.
    /// Can be positioned near speakers in world space.
    /// </summary>
    public class BubbleView : ConversationViewBase {
        [Header("Bubble Components")]
        [SerializeField]
        private TMP_Text dialogueText;

        [SerializeField]
        private TMP_Text speakerNameText;

        [SerializeField]
        private Image bubbleBackground;

        [SerializeField]
        private Image bubbleTail;

        [Header("Bubble Settings")]
        [SerializeField]
        private float maxWidth = 400f;

        [SerializeField]
        private float padding = 20f;

        [SerializeField]
        private BubbleTailPosition tailPosition = BubbleTailPosition.BottomLeft;

        [Header("World Space Positioning")]
        [SerializeField]
        private bool useWorldSpacePositioning = false;

        [SerializeField]
        private Vector3 worldOffset = new Vector3(0, 2f, 0);

        [SerializeField]
        private Camera targetCamera;

        [Header("Continue Indicator")]
        [SerializeField]
        private GameObject continueIndicator;

        [Header("Choices")]
        [SerializeField]
        private GameObject choicesContainer;

        [SerializeField]
        private Transform choicesParent;

        [SerializeField]
        private ConversationChoiceButton choiceButtonPrefab;

        // State
        private Transform _followTarget;
        private List<ConversationChoiceButton> _choiceButtons;
        private RectTransform _rectTransform;
        private ContentSizeFitter _sizeFitter;

        protected override void Awake() {
            base.Awake();
            
            _choiceButtons = new List<ConversationChoiceButton>();
            _rectTransform = GetComponent<RectTransform>();
            _sizeFitter = GetComponent<ContentSizeFitter>();

            if (targetCamera == null) {
                targetCamera = Camera.main;
            }
        }

        protected override void Update() {
            base.Update();
            UpdateWorldPosition();
        }

        /// <summary>
        /// Updates position to follow world space target.
        /// </summary>
        private void UpdateWorldPosition() {
            if (!useWorldSpacePositioning || !_followTarget || !targetCamera) {
                return;
            }

            Vector3 worldPos = _followTarget.position + worldOffset;
            Vector3 screenPos = targetCamera.WorldToScreenPoint(worldPos);

            if (_rectTransform) {
                _rectTransform.position = screenPos;
            }
        }

        /// <summary>
        /// Sets the target to follow in world space.
        /// </summary>
        /// <param name="target">The transform to follow.</param>
        /// <param name="offset">Optional offset from the target.</param>
        public void SetFollowTarget(Transform target, Vector3? offset = null) {
            _followTarget = target;
            if (offset.HasValue) {
                worldOffset = offset.Value;
            }
        }

        /// <summary>
        /// Clears the follow target.
        /// </summary>
        public void ClearFollowTarget() {
            _followTarget = null;
        }

        /// <summary>
        /// Sets the tail position.
        /// </summary>
        /// <param name="position">The tail position.</param>
        public void SetTailPosition(BubbleTailPosition position) {
            tailPosition = position;
            UpdateTailVisual();
        }

        /// <summary>
        /// Updates the tail visual based on position.
        /// </summary>
        private void UpdateTailVisual() {
            if (bubbleTail == null) {
                return;
            }

            RectTransform tailRect = bubbleTail.rectTransform;
            
            switch (tailPosition) {
                case BubbleTailPosition.BottomLeft:
                    tailRect.anchorMin = new Vector2(0.2f, 0f);
                    tailRect.anchorMax = new Vector2(0.2f, 0f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case BubbleTailPosition.BottomCenter:
                    tailRect.anchorMin = new Vector2(0.5f, 0f);
                    tailRect.anchorMax = new Vector2(0.5f, 0f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case BubbleTailPosition.BottomRight:
                    tailRect.anchorMin = new Vector2(0.8f, 0f);
                    tailRect.anchorMax = new Vector2(0.8f, 0f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case BubbleTailPosition.TopLeft:
                    tailRect.anchorMin = new Vector2(0.2f, 1f);
                    tailRect.anchorMax = new Vector2(0.2f, 1f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case BubbleTailPosition.TopCenter:
                    tailRect.anchorMin = new Vector2(0.5f, 1f);
                    tailRect.anchorMax = new Vector2(0.5f, 1f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case BubbleTailPosition.TopRight:
                    tailRect.anchorMin = new Vector2(0.8f, 1f);
                    tailRect.anchorMax = new Vector2(0.8f, 1f);
                    tailRect.rotation = Quaternion.Euler(0, 0, 180);
                    break;
            }
        }

        #region Abstract Implementation

        protected override void UpdateSpeakerDisplay(string speakerName, Sprite portrait, Color color) {
            // Bubble view doesn't typically show portrait inline
            // Speaker name can be shown above bubble
            
            if (speakerNameText != null) {
                bool hasName = !string.IsNullOrEmpty(speakerName);
                speakerNameText.gameObject.SetActive(hasName);
                speakerNameText.text = speakerName ?? string.Empty;
                speakerNameText.color = color;
            }

            // Optionally tint bubble based on speaker color
            if (bubbleBackground != null) {
                Color bubbleColor = Color.Lerp(Color.white, color, 0.1f);
                bubbleBackground.color = bubbleColor;
            }
        }

        protected override void UpdatePortrait(Sprite portrait) {
            // Bubble view typically doesn't show portraits
        }

        protected override void ClearSpeakerDisplay() {
            if (speakerNameText != null) {
                speakerNameText.gameObject.SetActive(false);
            }

            if (bubbleBackground != null) {
                bubbleBackground.color = Color.white;
            }
        }

        protected override void DisplayTextImmediate(string text) {
            if (dialogueText != null) {
                dialogueText.text = text;
            }
        }

        protected override void DisplayChoices(List<ChoiceOptionViewData> choices) {
            ClearChoiceButtons();

            if (choicesContainer != null) {
                choicesContainer.SetActive(true);
            }

            ConversationChoiceButton firstSelectable = null;

            foreach (ChoiceOptionViewData choice in choices) {
                if (!choice.IsVisible) {
                    continue;
                }

                if (choiceButtonPrefab == null || choicesParent == null) {
                    break;
                }

                ConversationChoiceButton button = Instantiate(choiceButtonPrefab, choicesParent);
                button.Setup(choice, SelectChoice);
                _choiceButtons.Add(button);

                if (firstSelectable == null && choice.IsSelectable) {
                    firstSelectable = button;
                }
            }

            firstSelectable?.Select();
        }

        protected override void ClearChoiceDisplay() {
            ClearChoiceButtons();

            if (choicesContainer != null) {
                choicesContainer.SetActive(false);
            }
        }

        /// <summary>
        /// Clears and destroys all choice buttons.
        /// </summary>
        private void ClearChoiceButtons() {
            foreach (ConversationChoiceButton button in _choiceButtons) {
                if (button != null) {
                    Destroy(button.gameObject);
                }
            }
            _choiceButtons.Clear();
        }

        public override void SetContinueIndicatorVisible(bool visible) {
            if (continueIndicator) {
                continueIndicator.SetActive(visible);
            }
        }

        #endregion

        /// <summary>
        /// Bubble tail positions.
        /// </summary>
        public enum BubbleTailPosition {
            BottomLeft,
            BottomCenter,
            BottomRight,
            TopLeft,
            TopCenter,
            TopRight
        }
    }
}