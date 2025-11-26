using System;
using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Interface for conversation UI views.
    /// Implement this to create custom dialogue UI layouts.
    /// </summary>
    public interface IConversationView {
        /// <summary>
        /// Gets whether the view is currently visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Gets whether the view is currently showing text.
        /// </summary>
        bool IsShowingText { get; }

        /// <summary>
        /// Gets whether the view is waiting for player input.
        /// </summary>
        bool IsWaitingForInput { get; }

        /// <summary>
        /// Gets whether the typewriter effect is currently active.
        /// </summary>
        bool IsTypewriterActive { get; }

        /// <summary>
        /// Event raised when player requests to advance/continue.
        /// </summary>
        event Action OnAdvanceRequested;

        /// <summary>
        /// Event raised when player selects a choice.
        /// </summary>
        event Action<string> OnChoiceSelected;

        /// <summary>
        /// Event raised when typewriter completes.
        /// </summary>
        event Action OnTypewriterComplete;

        /// <summary>
        /// Event raised when the view is hidden.
        /// </summary>
        event Action OnHidden;

        /// <summary>
        /// Shows the conversation view.
        /// </summary>
        void Show();

        /// <summary>
        /// Shows the view with animation.
        /// </summary>
        /// <returns>Task that completes when animation finishes.</returns>
        Task ShowAsync();

        /// <summary>
        /// Hides the conversation view.
        /// </summary>
        void Hide();

        /// <summary>
        /// Hides the view with animation.
        /// </summary>
        /// <returns>Task that completes when animation finishes.</returns>
        Task HideAsync();

        /// <summary>
        /// Displays a text node.
        /// </summary>
        /// <param name="data">The text node data to display.</param>
        void ShowText(TextNodeViewData data);

        /// <summary>
        /// Displays a text node asynchronously.
        /// </summary>
        /// <param name="data">The text node data to display.</param>
        /// <returns>Task that completes when text is fully displayed.</returns>
        Task ShowTextAsync(TextNodeViewData data);

        /// <summary>
        /// Displays choices for player selection.
        /// </summary>
        /// <param name="data">The choice node data to display.</param>
        void ShowChoices(ChoiceNodeViewData data);

        /// <summary>
        /// Displays choices asynchronously.
        /// </summary>
        /// <param name="data">The choice node data to display.</param>
        /// <returns>Task that completes when prompt is displayed and choices are ready.</returns>
        Task ShowChoicesAsync(ChoiceNodeViewData data);

        /// <summary>
        /// Hides the choice buttons.
        /// </summary>
        void HideChoices();

        /// <summary>
        /// Updates the speaker's expression.
        /// </summary>
        /// <param name="data">The expression update data.</param>
        void UpdateExpression(ExpressionUpdateData data);

        /// <summary>
        /// Skips the typewriter effect to show all text immediately.
        /// </summary>
        void SkipTypewriter();

        /// <summary>
        /// Clears all displayed content.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets whether the continue indicator is visible.
        /// </summary>
        /// <param name="visible">Whether to show the indicator.</param>
        void SetContinueIndicatorVisible(bool visible);

        /// <summary>
        /// Enables or disables the typewriter effect.
        /// </summary>
        /// <param name="isEnabled">Whether typewriter is isEnabled.</param>
        void SetTypewriterEnabled(bool isEnabled);
    }
}