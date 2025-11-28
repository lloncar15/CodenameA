using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Visual novel style dialogue with large character portraits.
    /// </summary>
    public class VisualNovelView : ConversationViewBase {
        [Header("Visual Novel Components")]
        [SerializeField]
        private TMP_Text dialogueText;

        [SerializeField]
        private TMP_Text speakerNameText;

        [SerializeField]
        private Image speakerNameBackground;

        [Header("Portrait Display")]
        [SerializeField]
        private Image leftPortrait;

        [SerializeField]
        private Image rightPortrait;

        [SerializeField]
        private Image centerPortrait;

        [SerializeField]
        private PortraitPosition defaultPortraitPosition = PortraitPosition.Left;

        [Header("Portrait Animation")]
        [SerializeField]
        private float portraitFadeDuration = 0.2f;

        [SerializeField]
        private Color inactivePortraitColor = new Color(0.5f, 0.5f, 0.5f, 1f);

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
        private PortraitPosition _currentPortraitPosition;
        private List<ConversationChoiceButton> _choiceButtons;
        private Dictionary<string, PortraitPosition> _characterPositions;

        protected override void Awake() {
            base.Awake();
            _choiceButtons = new List<ConversationChoiceButton>();
            _characterPositions = new Dictionary<string, PortraitPosition>();
        }

        /// <summary>
        /// Sets the portrait position for a character.
        /// </summary>
        /// <param name="characterId">The character ID.</param>
        /// <param name="position">The portrait position.</param>
        public void SetCharacterPosition(string characterId, PortraitPosition position) {
            _characterPositions[characterId] = position;
        }

        /// <summary>
        /// Gets the portrait image for a position.
        /// </summary>
        private Image GetPortraitImage(PortraitPosition position) {
            return position switch {
                PortraitPosition.Left => leftPortrait,
                PortraitPosition.Right => rightPortrait,
                PortraitPosition.Center => centerPortrait,
                _ => leftPortrait
            };
        }

        /// <summary>
        /// Gets the position for a character.
        /// </summary>
        private PortraitPosition GetCharacterPosition(string characterId) {
            if (!string.IsNullOrEmpty(characterId) && _characterPositions.TryGetValue(characterId, out var position)) {
                return position;
            }
            return defaultPortraitPosition;
        }

        #region Abstract Implementation

        protected override void UpdateSpeakerDisplay(string speakerName, Sprite portrait, Color color) {
            // Update name
            bool hasName = !string.IsNullOrEmpty(speakerName);
            
            if (speakerNameText != null) {
                speakerNameText.gameObject.SetActive(hasName);
                speakerNameText.text = speakerName ?? string.Empty;
                speakerNameText.color = color;
            }

            if (speakerNameBackground != null) {
                speakerNameBackground.gameObject.SetActive(hasName);
                speakerNameBackground.color = new Color(color.r, color.g, color.b, 0.9f);
            }

            // Update portrait
            if (portrait != null) {
                // Determine position (this would normally use character ID)
                _currentPortraitPosition = defaultPortraitPosition;
                
                // Dim all portraits first
                SetPortraitActive(leftPortrait, false);
                SetPortraitActive(rightPortrait, false);
                SetPortraitActive(centerPortrait, false);

                // Show and activate current speaker's portrait
                Image targetPortrait = GetPortraitImage(_currentPortraitPosition);
                if (targetPortrait != null) {
                    targetPortrait.sprite = portrait;
                    targetPortrait.gameObject.SetActive(true);
                    SetPortraitActive(targetPortrait, true);
                }
            }
        }

        /// <summary>
        /// Sets a portrait as active or inactive.
        /// </summary>
        private void SetPortraitActive(Image portrait, bool active) {
            if (portrait == null) {
                return;
            }

            portrait.color = active ? Color.white : inactivePortraitColor;
        }

        protected override void UpdatePortrait(Sprite portrait) {
            Image targetPortrait = GetPortraitImage(_currentPortraitPosition);
            if (targetPortrait != null && portrait != null) {
                targetPortrait.sprite = portrait;
            }
        }

        protected override void ClearSpeakerDisplay() {
            if (speakerNameText != null) {
                speakerNameText.gameObject.SetActive(false);
            }

            if (speakerNameBackground != null) {
                speakerNameBackground.gameObject.SetActive(false);
            }

            // Hide all portraits
            if (leftPortrait != null) {
                leftPortrait.gameObject.SetActive(false);
            }
            if (rightPortrait != null) {
                rightPortrait.gameObject.SetActive(false);
            }
            if (centerPortrait != null) {
                centerPortrait.gameObject.SetActive(false);
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
        /// Shows a character at a specific position.
        /// </summary>
        public void ShowCharacter(Sprite portrait, PortraitPosition position, bool isActive = true) {
            Image targetPortrait = GetPortraitImage(position);
            if (targetPortrait == null) {
                return;
            }

            targetPortrait.sprite = portrait;
            targetPortrait.gameObject.SetActive(true);
            SetPortraitActive(targetPortrait, isActive);
        }

        /// <summary>
        /// Hides a character at a specific position.
        /// </summary>
        public void HideCharacter(PortraitPosition position) {
            Image targetPortrait = GetPortraitImage(position);
            if (targetPortrait != null) {
                targetPortrait.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Portrait positions for visual novel layout.
        /// </summary>
        public enum PortraitPosition {
            Left,
            Right,
            Center
        }
    }
}