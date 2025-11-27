using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Processes TextNode instances.
    /// </summary>
    public class TextNodeProcessor : INodeProcessor {
        public ConversationNodeType NodeType => ConversationNodeType.Text;

        public async Task<string> ProcessAsync(ConversationNode node, ConversationContext context) {
            if (node is not TextNode textNode) {
                Debug.LogError("TextNodeProcessor: Invalid node type.");
                return null;
            }

            // Execute OnEnter commands
            if (textNode.OnEnterCommands != null && textNode.OnEnterCommands.Count > 0) {
                await ExecuteCommands(textNode.OnEnterCommands, context);
            }

            // Record node visit
            context.StateManager?.RecordNodeVisit(context.CurrentConversationId, textNode.Id);

            // Prepare view data
            TextNodeViewData viewData = CreateViewData(textNode, context);

            // Show text
            if (context.View != null) {
                await context.View.ShowTextAsync(viewData);

                // Wait for player input if required
                if (textNode.RequiresInput) {
                    await WaitForAdvance(context);
                }
                else if (textNode.AutoAdvanceDelay > 0) {
                    await Task.Delay((int)(textNode.AutoAdvanceDelay * 1000));
                }
            }

            // Execute OnExit commands
            if (textNode.OnExitCommands != null && textNode.OnExitCommands.Count > 0) {
                await ExecuteCommands(textNode.OnExitCommands, context);
            }

            return textNode.NextNodeId;
        }

        /// <summary>
        /// Creates view data from the text node.
        /// </summary>
        private TextNodeViewData CreateViewData(TextNode node, ConversationContext context) {
            var viewData = new TextNodeViewData {
                Text = node.Text,
                Expression = node.Expression,
                RequiresInput = node.RequiresInput,
                AutoAdvanceDelay = node.AutoAdvanceDelay,
                UseTypewriter = true
            };

            // Get character info
            if (!string.IsNullOrEmpty(node.SpeakerId) && context.CharacterProvider != null) {
                CharacterDefinition character = context.CharacterProvider.GetCharacter(node.SpeakerId);
                if (character != null) {
                    viewData.SpeakerName = character.DisplayName;
                    viewData.Portrait = character.GetPortrait(node.Expression);
                    viewData.SpeakerColor = character.CharacterColor;
                    viewData.VoiceSettings = character.VoiceSettings;

                    // Get emotion modifiers
                    CharacterEmotion emotion = character.GetEmotion(node.Expression);
                    if (emotion != null) {
                        viewData.EmotionPitchModifier = emotion.VoicePitchModifier;
                        viewData.EmotionSpeedModifier = emotion.VoiceSpeedModifier;
                    }
                }
            }

            return viewData;
        }

        /// <summary>
        /// Waits for the player to advance.
        /// </summary>
        private Task WaitForAdvance(ConversationContext context) {
            TaskCompletionSource<bool> tcs = new();

            void OnAdvance() {
                context.View.OnAdvanceRequested -= OnAdvance;
                tcs.TrySetResult(true);
            }

            context.View.OnAdvanceRequested += OnAdvance;

            return tcs.Task;
        }

        /// <summary>
        /// Executes a list of commands.
        /// </summary>
        private async Task ExecuteCommands(System.Collections.Generic.List<CommandData> commands, ConversationContext context) {
            CommandExecutor executor = new(context.CommandFactory);
            await executor.ExecuteAllAsync(commands, context.CommandContext);
        }
    }
}