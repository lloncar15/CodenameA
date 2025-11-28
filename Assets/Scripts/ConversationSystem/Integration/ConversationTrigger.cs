using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Triggers a conversation when the player enters a trigger zone or interacts.
    /// </summary>
    public class ConversationTrigger : MonoBehaviour {
        [Header("Conversation")]
        [SerializeField]
        private string conversationId;

        [SerializeField]
        private string startNodeId;

        [SerializeField]
        private TextAsset conversationJson;

        [Header("Trigger Settings")]
        [SerializeField]
        private TriggerMode triggerMode = TriggerMode.OnInteract;

        [SerializeField]
        private KeyCode interactKey = KeyCode.E;

        [SerializeField]
        private string playerTag = "Player";

        [SerializeField]
        private bool requiresLineOfSight = false;

        [SerializeField]
        private Transform lineOfSightOrigin;

        [SerializeField]
        private LayerMask lineOfSightBlockers;

        [Header("Behavior")]
        [SerializeField]
        private bool oneShot = false;

        [SerializeField]
        private bool disablePlayerMovement = true;

        [SerializeField]
        private float interactionCooldown = 0.5f;

        [Header("Visual Feedback")]
        [SerializeField]
        private GameObject interactionPrompt;

        [SerializeField]
        private bool showPromptWhenAvailable = true;

        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [Header("Events")]
        [SerializeField]
        private ConversationEvent onTriggerActivated;

        [SerializeField]
        private ConversationEvent onConversationComplete;

        // State
        private bool _isPlayerInRange;
        private bool _hasTriggered;
        private bool _isOnCooldown;
        private float _cooldownTimer;
        private Transform _playerTransform;

        private void Awake() {
            if (controller == null) {
                controller = FindAnyObjectByType<ConversationController>();
            }

            if (interactionPrompt != null) {
                interactionPrompt.SetActive(false);
            }
        }

        private void Update() {
            UpdateCooldown();
            UpdateInteractionPrompt();
            CheckForInteraction();
        }

        private void OnTriggerEnter(Collider other) {
            HandleTriggerEnter(other.gameObject, other.transform);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            HandleTriggerEnter(other.gameObject, other.transform);
        }

        private void OnTriggerExit(Collider other) {
            HandleTriggerExit(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other) {
            HandleTriggerExit(other.gameObject);
        }

        /// <summary>
        /// Handles trigger enter for both 2D and 3D.
        /// </summary>
        private void HandleTriggerEnter(GameObject obj, Transform objTransform) {
            if (!obj.CompareTag(playerTag)) {
                return;
            }

            _isPlayerInRange = true;
            _playerTransform = objTransform;

            if (triggerMode == TriggerMode.OnEnter) {
                TryStartConversation();
            }
        }

        /// <summary>
        /// Handles trigger exit for both 2D and 3D.
        /// </summary>
        private void HandleTriggerExit(GameObject obj) {
            if (!obj.CompareTag(playerTag)) {
                return;
            }

            _isPlayerInRange = false;
            _playerTransform = null;
        }

        /// <summary>
        /// Updates the cooldown timer.
        /// </summary>
        private void UpdateCooldown() {
            if (!_isOnCooldown) {
                return;
            }

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0) {
                _isOnCooldown = false;
            }
        }

        /// <summary>
        /// Updates the interaction prompt visibility.
        /// </summary>
        private void UpdateInteractionPrompt() {
            if (!interactionPrompt || !showPromptWhenAvailable) {
                return;
            }

            bool shouldShow = CanTrigger() && triggerMode == TriggerMode.OnInteract;
            interactionPrompt.SetActive(shouldShow);
        }

        /// <summary>
        /// Checks for player interaction input.
        /// </summary>
        private void CheckForInteraction() {
            if (triggerMode != TriggerMode.OnInteract) {
                return;
            }

            if (!_isPlayerInRange || !CanTrigger()) {
                return;
            }

            if (Input.GetKeyDown(interactKey)) {
                TryStartConversation();
            }
        }

        /// <summary>
        /// Checks if the trigger can be activated.
        /// </summary>
        private bool CanTrigger() {
            if (_isOnCooldown) {
                return false;
            }

            if (oneShot && _hasTriggered) {
                return false;
            }

            if (controller && controller.IsRunning) {
                return false;
            }

            if (requiresLineOfSight && !HasLineOfSight()) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if there's line of sight to the player.
        /// </summary>
        private bool HasLineOfSight() {
            if (!_playerTransform) {
                return false;
            }

            Transform origin = lineOfSightOrigin ? lineOfSightOrigin : transform;
            Vector3 direction = _playerTransform.position - origin.position;
            float distance = direction.magnitude;

            // 3D raycast
            if (Physics.Raycast(origin.position, direction.normalized, out RaycastHit hit, distance, lineOfSightBlockers)) {
                return hit.transform == _playerTransform;
            }

            // 2D raycast
            RaycastHit2D hit2D = Physics2D.Raycast(origin.position, direction.normalized, distance, lineOfSightBlockers);
            if (hit2D.collider) {
                return hit2D.transform == _playerTransform;
            }

            return true;
        }

        /// <summary>
        /// Attempts to start the conversation.
        /// </summary>
        private async void TryStartConversation() {
            if (!CanTrigger()) {
                return;
            }

            _hasTriggered = true;
            _isOnCooldown = true;
            _cooldownTimer = interactionCooldown;

            onTriggerActivated?.Invoke();

            if (disablePlayerMovement) {
                DisablePlayerMovement();
            }

            // Subscribe to completion
            if (controller) {
                controller.OnConversationEnded += HandleConversationEnded;
            }

            // Start conversation
            if (controller) {
                if (!string.IsNullOrEmpty(conversationId)) {
                    await controller.StartConversationAsync(conversationId, startNodeId);
                }
                else if (conversationJson) {
                    await controller.StartConversationFromJsonAsync(conversationJson.text, startNodeId);
                }
                else {
                    Debug.LogWarning("ConversationTrigger: No conversation ID or JSON assigned.");
                }
            }
        }

        /// <summary>
        /// Handles conversation ended.
        /// </summary>
        private void HandleConversationEnded() {
            if (controller) {
                controller.OnConversationEnded -= HandleConversationEnded;
            }

            if (disablePlayerMovement) {
                EnablePlayerMovement();
            }

            onConversationComplete?.Invoke();
        }

        /// <summary>
        /// Disables player movement.
        /// Override in subclass for custom implementation.
        /// </summary>
        protected virtual void DisablePlayerMovement() {
            // Default implementation - try to find common player controllers
            if (!_playerTransform) {
                return;
            }

            // Try Rigidbody
            Rigidbody rb = _playerTransform.GetComponent<Rigidbody>();
            if (rb) {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            Rigidbody2D rb2D = _playerTransform.GetComponent<Rigidbody2D>();
            if (rb2D) {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.bodyType = RigidbodyType2D.Kinematic;
            }
            
            //TODO: disable player controller
        }

        /// <summary>
        /// Enables player movement.
        /// Override in subclass for custom implementation.
        /// </summary>
        protected virtual void EnablePlayerMovement() {
            if (!_playerTransform) {
                return;
            }

            Rigidbody rb = _playerTransform.GetComponent<Rigidbody>();
            if (rb) {
                rb.isKinematic = false;
            }

            Rigidbody2D rb2D = _playerTransform.GetComponent<Rigidbody2D>();
            if (rb2D) {
                rb2D.bodyType = RigidbodyType2D.Kinematic;
            }
            
            //TODO: enable player controller
        }

        /// <summary>
        /// Resets the trigger so it can be activated again.
        /// </summary>
        public void ResetTrigger() {
            _hasTriggered = false;
            _isOnCooldown = false;
        }

        /// <summary>
        /// Manually triggers the conversation.
        /// </summary>
        public void TriggerManually() {
            if (controller != null && !controller.IsRunning) {
                TryStartConversation();
            }
        }

        /// <summary>
        /// Trigger activation modes.
        /// </summary>
        public enum TriggerMode {
            /// <summary>
            /// Triggers when player enters the zone.
            /// </summary>
            OnEnter,

            /// <summary>
            /// Triggers when player presses interact key while in zone.
            /// </summary>
            OnInteract,

            /// <summary>
            /// Manual trigger only via code.
            /// </summary>
            Manual
        }
    }
}