using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Abstract base class for conversation views.
    /// Provides common functionality for all view implementations.
    /// </summary>
    public abstract class ConversationViewBase : MonoBehaviour, IConversationView {
        [Header("Base Components")]
        [SerializeField]
        protected Canvas canvas;

        [SerializeField]
        protected CanvasGroup canvasGroup;

        [SerializeField]
        protected GameObject rootPanel;

        [Header("Typewriter")]
        [SerializeField]
        protected TypewriterEffect typewriter;

        [SerializeField]
        protected bool typewriterEnabled = true;

        [Header("Animation")]
        [SerializeField]
        protected float showAnimationDuration = 0.3f;

        [SerializeField]
        protected float hideAnimationDuration = 0.2f;

        [Header("Input")]
        [SerializeField]
        protected KeyCode advanceKey = KeyCode.Space;

        [SerializeField]
        protected KeyCode skipKey = KeyCode.Return;

        [SerializeField]
        protected bool allowMouseAdvance = true;

        // State
        protected bool isVisible;
        protected bool isShowingText;
        protected bool isWaitingForInput;
        protected bool isShowingChoices;
        protected TextNodeViewData currentTextData;
        protected ChoiceNodeViewData currentChoiceData;

        // Task completion sources for async operations
        protected TaskCompletionSource<bool> showTcs;
        protected TaskCompletionSource<bool> hideTcs;
        protected TaskCompletionSource<bool> textTcs;

        // Events
        public event Action OnAdvanceRequested;
        public event Action<string> OnChoiceSelected;
        public event Action OnTypewriterComplete;
        public event Action OnHidden;

        // Properties
        public bool IsVisible => isVisible;
        public bool IsShowingText => isShowingText;
        public bool IsWaitingForInput => isWaitingForInput;
        public bool IsTypewriterActive => typewriter != null && typewriter.IsActive;

        protected virtual void Awake() {
            InitializeComponents();
            SetupTypewriter();
        }

        protected virtual void OnDestroy() {
            CleanupTypewriter();
        }

        protected virtual void Update() {
            HandleInput();
        }

        /// <summary>
        /// Initializes UI components.
        /// </summary>
        protected virtual void InitializeComponents() {
            if (canvas == null) {
                canvas = GetComponentInParent<Canvas>();
            }

            if (canvasGroup == null) {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null && rootPanel != null) {
                    canvasGroup = rootPanel.GetComponent<CanvasGroup>();
                }
            }

            // Start hidden
            SetVisibility(false);
        }

        /// <summary>
        /// Sets up typewriter event handlers.
        /// </summary>
        protected virtual void SetupTypewriter() {
            if (typewriter == null) {
                return;
            }

            typewriter.OnComplete += HandleTypewriterComplete;
            typewriter.OnExpressionChange += HandleExpressionChange;
        }

        /// <summary>
        /// Cleans up typewriter event handlers.
        /// </summary>
        protected virtual void CleanupTypewriter() {
            if (typewriter == null) {
                return;
            }

            typewriter.OnComplete -= HandleTypewriterComplete;
            typewriter.OnExpressionChange -= HandleExpressionChange;
        }

        /// <summary>
        /// Handles input for advancing dialogue.
        /// </summary>
        protected virtual void HandleInput() {
            if (!isVisible) {
                return;
            }

            bool advancePressed = Input.GetKeyDown(advanceKey) || 
                                  (allowMouseAdvance && Input.GetMouseButtonDown(0));
            bool skipPressed = Input.GetKeyDown(skipKey);

            if (IsTypewriterActive) {
                if (advancePressed || skipPressed) {
                    SkipTypewriter();
                }
            }
            else if (isWaitingForInput && !isShowingChoices) {
                if (advancePressed) {
                    RequestAdvance();
                }
            }
        }

        /// <summary>
        /// Requests to advance the dialogue.
        /// </summary>
        protected virtual void RequestAdvance() {
            OnAdvanceRequested?.Invoke();
        }

        /// <summary>
        /// Handles choice selection.
        /// </summary>
        /// <param name="choiceId">The selected choice ID.</param>
        protected virtual void SelectChoice(string choiceId) {
            if (!isShowingChoices) {
                return;
            }

            isShowingChoices = false;
            OnChoiceSelected?.Invoke(choiceId);
        }

        /// <summary>
        /// Handles typewriter completion.
        /// </summary>
        protected virtual void HandleTypewriterComplete() {
            isWaitingForInput = currentTextData?.RequiresInput ?? true;
            SetContinueIndicatorVisible(isWaitingForInput && !isShowingChoices);
            
            OnTypewriterComplete?.Invoke();
            textTcs?.TrySetResult(true);
        }

        /// <summary>
        /// Handles expression change from text commands.
        /// </summary>
        protected virtual void HandleExpressionChange(string expressionKey) {
            // Subclasses should override to update portrait
        }

        #region IConversationView Implementation

        /// <inheritdoc />
        public virtual void Show() {
            SetVisibility(true);
            OnShowStarted();
        }

        /// <inheritdoc />
        public virtual async Task ShowAsync() {
            showTcs = new TaskCompletionSource<bool>();
            
            SetVisibility(true);
            OnShowStarted();
            
            await AnimateShow();
            
            showTcs.TrySetResult(true);
        }

        /// <inheritdoc />
        public virtual void Hide() {
            SetVisibility(false);
            OnHideCompleted();
        }

        /// <inheritdoc />
        public virtual async Task HideAsync() {
            hideTcs = new TaskCompletionSource<bool>();
            
            await AnimateHide();
            
            SetVisibility(false);
            OnHideCompleted();
            
            hideTcs.TrySetResult(true);
        }

        /// <inheritdoc />
        public virtual void ShowText(TextNodeViewData data) {
            currentTextData = data;
            isShowingText = true;
            isWaitingForInput = false;
            
            SetContinueIndicatorVisible(false);
            HideChoices();
            
            UpdateSpeakerDisplay(data.SpeakerName, data.Portrait, data.SpeakerColor);
            
            if (typewriter != null && typewriterEnabled && data.UseTypewriter) {
                SetupVoice(data);
                typewriter.ShowText(data.Text);
            }
            else {
                DisplayTextImmediate(data.Text);
                HandleTypewriterComplete();
            }
        }

        /// <inheritdoc />
        public virtual async Task ShowTextAsync(TextNodeViewData data) {
            textTcs = new TaskCompletionSource<bool>();
            
            ShowText(data);
            
            await textTcs.Task;
        }

        /// <inheritdoc />
        public virtual void ShowChoices(ChoiceNodeViewData data) {
            currentChoiceData = data;
            isShowingChoices = true;
            isWaitingForInput = false;
            
            SetContinueIndicatorVisible(false);
            
            if (!string.IsNullOrEmpty(data.PromptText)) {
                UpdateSpeakerDisplay(data.SpeakerName, data.Portrait, data.SpeakerColor);
                
                if (typewriter != null && typewriterEnabled && data.UseTypewriter) {
                    typewriter.ShowText(data.PromptText);
                }
                else {
                    DisplayTextImmediate(data.PromptText);
                }
            }
            
            DisplayChoices(data.Choices);
        }

        /// <inheritdoc />
        public virtual async Task ShowChoicesAsync(ChoiceNodeViewData data) {
            textTcs = new TaskCompletionSource<bool>();
            
            ShowChoices(data);
            
            if (!string.IsNullOrEmpty(data.PromptText) && typewriterEnabled) {
                await textTcs.Task;
            }
        }

        /// <inheritdoc />
        public virtual void HideChoices() {
            isShowingChoices = false;
            ClearChoiceDisplay();
        }

        /// <inheritdoc />
        public virtual void UpdateExpression(ExpressionUpdateData data) {
            if (data.Portrait != null) {
                UpdatePortrait(data.Portrait);
            }

            if (typewriter != null) {
                typewriter.SetVoice(null, data.PitchModifier, data.SpeedModifier);
            }
        }

        /// <inheritdoc />
        public virtual void SkipTypewriter() {
            typewriter?.Skip();
        }

        /// <inheritdoc />
        public virtual void Clear() {
            typewriter?.Clear();
            HideChoices();
            ClearSpeakerDisplay();
            SetContinueIndicatorVisible(false);
            
            isShowingText = false;
            isWaitingForInput = false;
            currentTextData = null;
            currentChoiceData = null;
        }

        /// <inheritdoc />
        public abstract void SetContinueIndicatorVisible(bool visible);

        /// <inheritdoc />
        public virtual void SetTypewriterEnabled(bool isEnabled) {
            typewriterEnabled = isEnabled;
        }

        #endregion

        #region Abstract Methods (Must be implemented by subclasses)

        /// <summary>
        /// Updates the speaker name and portrait display.
        /// </summary>
        protected abstract void UpdateSpeakerDisplay(string speakerName, Sprite portrait, Color color);

        /// <summary>
        /// Updates the portrait image.
        /// </summary>
        protected abstract void UpdatePortrait(Sprite portrait);

        /// <summary>
        /// Clears the speaker display.
        /// </summary>
        protected abstract void ClearSpeakerDisplay();

        /// <summary>
        /// Displays text immediately without typewriter.
        /// </summary>
        protected abstract void DisplayTextImmediate(string text);

        /// <summary>
        /// Displays choice buttons.
        /// </summary>
        protected abstract void DisplayChoices(System.Collections.Generic.List<ChoiceOptionViewData> choices);

        /// <summary>
        /// Clears the choice display.
        /// </summary>
        protected abstract void ClearChoiceDisplay();

        #endregion

        #region Protected Helpers

        /// <summary>
        /// Sets up voice for the current speaker.
        /// </summary>
        protected virtual void SetupVoice(TextNodeViewData data) {
            if (typewriter == null) {
                return;
            }

            typewriter.SetVoice(data.VoiceSettings, data.EmotionPitchModifier, data.EmotionSpeedModifier);
        }

        /// <summary>
        /// Sets the visibility of the view.
        /// </summary>
        protected virtual void SetVisibility(bool visible) {
            isVisible = visible;

            if (rootPanel != null) {
                rootPanel.SetActive(visible);
            }

            if (canvasGroup != null) {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
        }

        /// <summary>
        /// Called when show animation starts.
        /// </summary>
        protected virtual void OnShowStarted() {
            // Override in subclasses
        }

        /// <summary>
        /// Called when hide animation completes.
        /// </summary>
        protected virtual void OnHideCompleted() {
            Clear();
            OnHidden?.Invoke();
        }

        /// <summary>
        /// Animates showing the view.
        /// </summary>
        protected virtual async Task AnimateShow() {
            if (canvasGroup == null || showAnimationDuration <= 0) {
                return;
            }

            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < showAnimationDuration) {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / showAnimationDuration);
                await Task.Yield();
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Animates hiding the view.
        /// </summary>
        protected virtual async Task AnimateHide() {
            if (canvasGroup == null || hideAnimationDuration <= 0) {
                return;
            }

            float elapsed = 0f;

            while (elapsed < hideAnimationDuration) {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / hideAnimationDuration);
                await Task.Yield();
            }

            canvasGroup.alpha = 0f;
        }

        #endregion
    }
}