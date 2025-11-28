using System.Threading.Tasks;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Command that introduces a delay before continuing.
    /// Only effective when executed asynchronously.
    /// </summary>
    public class DelayCommand : ConversationCommandBase {
        public override string CommandType => "Delay";

        private readonly float _durationSeconds;

        /// <summary>
        /// Creates a new DelayCommand.
        /// </summary>
        /// <param name="durationSeconds">The delay duration in seconds.</param>
        public DelayCommand(float durationSeconds) {
            _durationSeconds = durationSeconds;
        }

        /// <summary>
        /// Creates a DelayCommand from CommandData.
        /// </summary>
        /// <param name="data">The command data.</param>
        /// <returns>A new DelayCommand instance.</returns>
        public static DelayCommand FromData(CommandData data) {
            float duration = data.GetFloat("duration");
            return new DelayCommand(duration) {
                SourceData = data
            };
        }

        /// <inheritdoc />
        public override void Execute(ICommandContext context) {
            // Synchronous execution ignores delay
            UnityEngine.Debug.LogWarning("DelayCommand: Synchronous execution ignores delay. Use ExecuteAsync for delays.");
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(ICommandContext context) {
            if (_durationSeconds > 0) {
                await Task.Delay((int)(_durationSeconds * 1000));
            }
        }

        /// <inheritdoc />
        public override bool Validate(out string errorMessage) {
            if (_durationSeconds < 0) {
                errorMessage = "DelayCommand: Duration cannot be negative.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public override string ToString() {
            return $"Delay({_durationSeconds}s)";
        }
    }
}