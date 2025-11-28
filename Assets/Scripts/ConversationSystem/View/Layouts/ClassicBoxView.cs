using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Classic RPG-style dialogue box at the bottom of the screen.
    /// </summary>
    public class ClassicBoxView : ConversationViewBase {
        [Header("Classic Box Components")]
        [SerializeField]
        private TMP_Text dialogueText;

        [SerializeField]
        private TMP_Text speakerNameText;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private GameObject portraitContainer;

        [SerializeField]
        private GameObject nameContainer;

        [SerializeField]
        private Image nameBackground;

        [Header("Continue Indicator")]
        [SerializeField]
        private GameObject continueIndicator;

        [SerializeField]
        private ContinueIndicator continueIndicatorComponent;

        [Header("Choices")]
        [SerializeField]
        private GameObject choicesContainer;

        [SerializeField]
        private Transform choicesParent;

        [SerializeField]
        private ConversationChoiceButton choiceButtonPrefab;

        [SerializeField]
        private int maxPooledButtons = 6;

        // Pooled choice buttons
        private List<ConversationChoiceButton> _choiceButtonPool;

        protected override void Awake() {
            base.Awake();
            InitializeChoicePool();
        }

        /// <summary>
        /// Initializes the choice button pool.
        /// </summary>
        private void InitializeChoicePool() {
            _choiceButtonPool = new List<ConversationChoiceButton>();

            if (choiceButtonPrefab == null || choicesParent == null) {
                return;
            }

            for (int i = 0; i < maxPooledButtons; i++) {
                ConversationChoiceButton button = Instantiate(choiceButtonPrefab, choicesParent);
                button.gameObject.SetActive(false);
                _choiceButtonPool.Add(button);
            }
        }

        /// <summary>
        /// Gets a choice button from the pool.
        /// </summary>
        private ConversationChoiceButton GetChoiceButton() {
            foreach (ConversationChoiceButton button in _choiceButtonPool) {
                if (!button.gameObject.activeSelf) {
                    return button;
                }
            }

            // Create new if pool exhausted
            if (choiceButtonPrefab != null && choicesParent != null) {
                ConversationChoiceButton newButton = Instantiate(choiceButtonPrefab, choicesParent);
                _choiceButtonPool.Add(newButton);
                return newButton;
            }

            return null;
        }

        /// <summary>
        /// Returns all choice buttons to the pool.
        /// </summary>
        private void ReturnAllChoiceButtons() {
            foreach (ConversationChoiceButton button in _choiceButtonPool) {
                button.Clear();
                button.gameObject.SetActive(false);
            }
        }

        #region Abstract Implementation

        protected override void UpdateSpeakerDisplay(string speakerName, Sprite portrait, Color color) {
            // Update name
            bool hasName = !string.IsNullOrEmpty(speakerName);
            
            if (nameContainer != null) {
                nameContainer.SetActive(hasName);
            }

            if (speakerNameText != null) {
                speakerNameText.text = speakerName ?? string.Empty;
                speakerNameText.color = color;
            }

            if (nameBackground != null) {
                nameBackground.color = new Color(color.r, color.g, color.b, 0.8f);
            }

            // Update portrait
            bool hasPortrait = portrait != null;
            
            if (portraitContainer != null) {
                portraitContainer.SetActive(hasPortrait);
            }

            if (portraitImage != null && hasPortrait) {
                portraitImage.sprite = portrait;
                portraitImage.color = Color.white;
            }
        }

        protected override void UpdatePortrait(Sprite portrait) {
            if (portraitImage != null && portrait != null) {
                portraitImage.sprite = portrait;
            }
        }

        protected override void ClearSpeakerDisplay() {
            if (nameContainer != null) {
                nameContainer.SetActive(false);
            }

            if (portraitContainer != null) {
                portraitContainer.SetActive(false);
            }

            if (speakerNameText != null) {
                speakerNameText.text = string.Empty;
            }

            if (portraitImage != null) {
                portraitImage.sprite = null;
            }
        }

        protected override void DisplayTextImmediate(string text) {
            if (dialogueText != null) {
                dialogueText.text = text;
            }
        }

        protected override void DisplayChoices(List<ChoiceOptionViewData> choices) {
            ReturnAllChoiceButtons();

            if (choicesContainer != null) {
                choicesContainer.SetActive(true);
            }

            ConversationChoiceButton firstSelectable = null;

            foreach (ChoiceOptionViewData choice in choices) {
                if (!choice.IsVisible) {
                    continue;
                }

                ConversationChoiceButton button = GetChoiceButton();
                if (button == null) {
                    Debug.LogWarning("ClassicBoxView: No more choice buttons available.");
                    break;
                }

                button.Setup(choice, SelectChoice);
                button.gameObject.SetActive(true);

                if (firstSelectable == null && choice.IsSelectable) {
                    firstSelectable = button;
                }
            }

            // Auto-select first selectable choice
            firstSelectable?.Select();
        }

        protected override void ClearChoiceDisplay() {
            ReturnAllChoiceButtons();

            if (choicesContainer != null) {
                choicesContainer.SetActive(false);
            }
        }

        public override void SetContinueIndicatorVisible(bool visible) {
            if (continueIndicator) {
                continueIndicator.SetActive(visible);
            }

            if (visible && continueIndicatorComponent) {
                continueIndicatorComponent.ResetAnimation();
            }
        }

        #endregion

        protected override void HandleExpressionChange(string expressionKey) {
            base.HandleExpressionChange(expressionKey);

            // Update portrait if we have access to character data
            // This would typically be handled by the controller
        }
    }
}