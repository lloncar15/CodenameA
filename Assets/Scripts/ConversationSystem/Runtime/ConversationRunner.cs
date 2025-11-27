using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Runs a conversation from start to finish.
    /// </summary>
    public class ConversationRunner {
        private readonly NodeProcessorRegistry _processorRegistry;
        private readonly ConversationContext _context;
        
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;
        private bool _isPaused;

        /// <summary>
        /// Gets whether the runner is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Gets whether the runner is paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets the conversation context.
        /// </summary>
        public ConversationContext Context => _context;

        /// <summary>
        /// Event raised when the conversation starts.
        /// </summary>
        public event Action OnConversationStarted;

        /// <summary>
        /// Event raised when the conversation ends.
        /// </summary>
        public event Action OnConversationEnded;

        /// <summary>
        /// Event raised when a node is entered.
        /// </summary>
        public event Action<ConversationNode> OnNodeEntered;

        /// <summary>
        /// Event raised when a node is exited.
        /// </summary>
        public event Action<ConversationNode, string> OnNodeExited;

        /// <summary>
        /// Event raised when an error occurs.
        /// </summary>
        public event Action<Exception> OnError;

        /// <summary>
        /// Creates a new conversation runner.
        /// </summary>
        /// <param name="context">The conversation context.</param>
        /// <param name="processorRegistry">The processor registry (null = default).</param>
        public ConversationRunner(ConversationContext context, NodeProcessorRegistry processorRegistry = null) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _processorRegistry = processorRegistry ?? NodeProcessorRegistry.CreateDefault();
        }

        /// <summary>
        /// Starts running a conversation.
        /// </summary>
        /// <param name="conversation">The conversation to run.</param>
        /// <param name="startNodeId">Optional starting node (null = use conversation default).</param>
        public async Task RunAsync(ConversationData conversation, string startNodeId = null) {
            if (_isRunning) {
                Debug.LogWarning("ConversationRunner: Already running a conversation.");
                return;
            }

            if (conversation == null) {
                Debug.LogError("ConversationRunner: Conversation is null.");
                return;
            }

            _context.Conversation = conversation;
            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;
            _isPaused = false;

            string currentNodeId = startNodeId ?? conversation.StartNodeId;

            try {
                OnConversationStarted?.Invoke();

                // Show view
                if (_context.View != null) {
                    await _context.View.ShowAsync();
                }

                // Main loop
                while (!string.IsNullOrEmpty(currentNodeId) && _isRunning) {
                    // Check for cancellation
                    if (_cancellationTokenSource.Token.IsCancellationRequested) {
                        break;
                    }

                    // Wait while paused
                    while (_isPaused && _isRunning) {
                        await Task.Delay(100);
                    }

                    // Get node
                    ConversationNode node = _context.Conversation.GetNode(currentNodeId);
                    if (node == null) {
                        Debug.LogError($"ConversationRunner: Node '{currentNodeId}' not found.");
                        break;
                    }

                    _context.CurrentNode = node;
                    OnNodeEntered?.Invoke(node);

                    // Process node
                    string nextNodeId = await ProcessNodeAsync(node);

                    OnNodeExited?.Invoke(node, nextNodeId);

                    // Check for conversation return
                    if (string.IsNullOrEmpty(nextNodeId) && _context.ConversationStack.Count > 0) {
                        ConversationStackFrame frame = _context.PopState();
                        
                        // Load the previous conversation
                        // This would need the conversation loader
                        nextNodeId = frame.NodeId;
                        
                        Debug.Log($"ConversationRunner: Returning to conversation '{frame.ConversationId}' at node '{nextNodeId}'.");
                    }

                    currentNodeId = nextNodeId;
                }

                // Hide view
                if (_context.View != null) {
                    await _context.View.HideAsync();
                }
            }
            catch (Exception ex) {
                Debug.LogError($"ConversationRunner: Error - {ex.Message}");
                OnError?.Invoke(ex);
            }
            finally {
                _isRunning = false;
                _context.CurrentNode = null;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                OnConversationEnded?.Invoke();
            }
        }

        /// <summary>
        /// Processes a single node.
        /// </summary>
        private async Task<string> ProcessNodeAsync(ConversationNode node) {
            INodeProcessor processor = _processorRegistry.GetProcessor(node);
            if (processor == null) {
                Debug.LogError($"ConversationRunner: No processor for node type '{node.NodeType}'.");
                return null;
            }

            return await processor.ProcessAsync(node, _context);
        }

        /// <summary>
        /// Pauses the conversation.
        /// </summary>
        public void Pause() {
            _isPaused = true;
        }

        /// <summary>
        /// Resumes the conversation.
        /// </summary>
        public void Resume() {
            _isPaused = false;
        }

        /// <summary>
        /// Stops the conversation.
        /// </summary>
        public void Stop() {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Skips the current typewriter effect.
        /// </summary>
        public void SkipTypewriter() {
            _context.View?.SkipTypewriter();
        }
    }
}