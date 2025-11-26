using System;
using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Null implementation of IConversationView for testing or headless operation.
    /// All operations complete immediately without visual output.
    /// </summary>
    public class NullConversationView : IConversationView {
        public bool IsVisible { get; private set; }
        public bool IsShowingText { get; private set; }
        public bool IsWaitingForInput { get; private set; }
        public bool IsTypewriterActive => false;

        public event Action OnAdvanceRequested;
        public event Action<string> OnChoiceSelected;
        public event Action OnTypewriterComplete;
        public event Action OnHidden;

        public void Show() {
            IsVisible = true;
        }

        public Task ShowAsync() {
            Show();
            return Task.CompletedTask;
        }

        public void Hide() {
            IsVisible = false;
            OnHidden?.Invoke();
        }

        public Task HideAsync() {
            Hide();
            return Task.CompletedTask;
        }

        public void ShowText(TextNodeViewData data) {
            IsShowingText = true;
            IsWaitingForInput = data?.RequiresInput ?? true;
            OnTypewriterComplete?.Invoke();
        }

        public Task ShowTextAsync(TextNodeViewData data) {
            ShowText(data);
            return Task.CompletedTask;
        }

        public void ShowChoices(ChoiceNodeViewData data) {
            IsShowingText = true;
        }

        public Task ShowChoicesAsync(ChoiceNodeViewData data) {
            ShowChoices(data);
            return Task.CompletedTask;
        }

        public void HideChoices() { }

        public void UpdateExpression(ExpressionUpdateData data) { }

        public void SkipTypewriter() { }

        public void Clear() {
            IsShowingText = false;
            IsWaitingForInput = false;
        }

        public void SetContinueIndicatorVisible(bool visible) { }

        public void SetTypewriterEnabled(bool enabled) { }

        /// <summary>
        /// Simulates player advancing dialogue.
        /// </summary>
        public void SimulateAdvance() {
            OnAdvanceRequested?.Invoke();
        }

        /// <summary>
        /// Simulates player selecting a choice.
        /// </summary>
        /// <param name="choiceId">The choice ID to select.</param>
        public void SimulateChoiceSelection(string choiceId) {
            OnChoiceSelected?.Invoke(choiceId);
        }
    }
}