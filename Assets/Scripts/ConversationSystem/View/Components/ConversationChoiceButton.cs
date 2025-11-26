using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// UI component for a dialogue choice button.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ConversationChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [Header("Components")]
        [SerializeField]
        private Button button;

        [SerializeField]
        private TMP_Text choiceText;

        [SerializeField]
        private TMP_Text unavailableReasonText;

        [SerializeField]
        private TMP_Text consequencePreviewText;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private GameObject selectedIndicator;

        [Header("Colors")]
        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color hoverColor = new Color(0.9f, 0.9f, 1f);

        [SerializeField]
        private Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

        [SerializeField]
        private Color disabledTextColor = new Color(0.6f, 0.6f, 0.6f);

        // Data
        private string _choiceId;
        private bool _isSelectable;
        private Action<string> _onClickCallback;

        /// <summary>
        /// Gets the choice ID.
        /// </summary>
        public string ChoiceId => _choiceId;

        /// <summary>
        /// Gets whether this choice is selectable.
        /// </summary>
        public bool IsSelectable => _isSelectable;

        private void Awake() {
            if (button == null) {
                button = GetComponent<Button>();
            }

            button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy() {
            button.onClick.RemoveListener(HandleClick);
        }

        /// <summary>
        /// Sets up the choice button with data.
        /// </summary>
        /// <param name="data">The choice data.</param>
        /// <param name="onClickCallback">Callback when clicked.</param>
        public void Setup(ChoiceOptionViewData data, Action<string> onClickCallback) {
            _choiceId = data.Id;
            _isSelectable = data.IsSelectable;
            _onClickCallback = onClickCallback;

            // Set text
            if (choiceText != null) {
                choiceText.text = data.Text;
                choiceText.color = _isSelectable ? normalColor : disabledTextColor;
            }

            // Set unavailable reason
            if (unavailableReasonText != null) {
                bool showReason = !_isSelectable && !string.IsNullOrEmpty(data.UnavailableReason);
                unavailableReasonText.gameObject.SetActive(showReason);
                if (showReason) {
                    unavailableReasonText.text = data.UnavailableReason;
                }
            }

            // Set consequence preview
            if (consequencePreviewText != null) {
                bool showPreview = !string.IsNullOrEmpty(data.ConsequencePreview);
                consequencePreviewText.gameObject.SetActive(showPreview);
                if (showPreview) {
                    consequencePreviewText.text = data.ConsequencePreview;
                }
            }

            // Set button interactability
            button.interactable = _isSelectable;

            // Set background color
            if (backgroundImage != null) {
                backgroundImage.color = _isSelectable ? normalColor : disabledColor;
            }

            // Hide selected indicator
            if (selectedIndicator != null) {
                selectedIndicator.SetActive(false);
            }

            gameObject.SetActive(data.IsVisible);
        }

        /// <summary>
        /// Handles button click.
        /// </summary>
        private void HandleClick() {
            if (_isSelectable) {
                _onClickCallback?.Invoke(_choiceId);
            }
        }

        /// <summary>
        /// Handles pointer enter (hover).
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) {
            if (!_isSelectable) {
                return;
            }

            if (backgroundImage != null) {
                backgroundImage.color = hoverColor;
            }

            if (selectedIndicator != null) {
                selectedIndicator.SetActive(true);
            }
        }

        /// <summary>
        /// Handles pointer exit.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData) {
            if (backgroundImage != null) {
                backgroundImage.color = _isSelectable ? normalColor : disabledColor;
            }

            if (selectedIndicator != null) {
                selectedIndicator.SetActive(false);
            }
        }

        /// <summary>
        /// Selects this button programmatically.
        /// </summary>
        public void Select() {
            if (button != null && _isSelectable) {
                button.Select();
            }
        }

        /// <summary>
        /// Clears the button state.
        /// </summary>
        public void Clear() {
            _choiceId = null;
            _isSelectable = false;
            _onClickCallback = null;

            if (choiceText != null) {
                choiceText.text = string.Empty;
            }

            if (unavailableReasonText != null) {
                unavailableReasonText.gameObject.SetActive(false);
            }

            if (consequencePreviewText != null) {
                consequencePreviewText.gameObject.SetActive(false);
            }

            if (selectedIndicator != null) {
                selectedIndicator.SetActive(false);
            }
        }
    }
}