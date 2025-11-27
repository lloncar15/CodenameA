// Assets/Scripts/ConversationSystem/Integration/ConversationCameraController.cs
using System.Collections;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Controls camera behavior during conversations.
    /// Supports focus on speakers, cinematic shots, and transitions.
    /// </summary>
    public class ConversationCameraController : MonoBehaviour {
        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [Header("Camera")]
        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private Transform defaultCameraPosition;

        [Header("Focus Settings")]
        [SerializeField]
        private float focusTransitionDuration = 0.5f;

        [SerializeField]
        private float focusDistance = 3f;

        [SerializeField]
        private float focusHeight = 1.5f;

        [Header("Dialogue Settings")]
        [SerializeField]
        private bool autoFocusOnSpeaker = false;

        [SerializeField]
        private float dialogueFieldOfView = 40f;

        [SerializeField]
        private float normalFieldOfView = 60f;

        // State
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private float _originalFov;
        private Coroutine _transitionCoroutine;
        private Transform _currentFocusTarget;

        private void Awake() {
            if (targetCamera == null) {
                targetCamera = Camera.main;
            }

            if (controller == null) {
                controller = FindAnyObjectByType<ConversationController>();
            }
        }

        private void OnEnable() {
            if (controller != null) {
                controller.OnConversationStarted += HandleConversationStarted;
                controller.OnConversationEnded += HandleConversationEnded;
                controller.OnConversationEvent += HandleConversationEvent;
            }
        }

        private void OnDisable() {
            if (controller != null) {
                controller.OnConversationStarted -= HandleConversationStarted;
                controller.OnConversationEnded -= HandleConversationEnded;
                controller.OnConversationEvent -= HandleConversationEvent;
            }
        }

        /// <summary>
        /// Handles conversation started.
        /// </summary>
        private void HandleConversationStarted() {
            if (!targetCamera) {
                return;
            }

            // Store original camera state
            _originalPosition = targetCamera.transform.position;
            _originalRotation = targetCamera.transform.rotation;
            _originalFov = targetCamera.fieldOfView;

            // Optionally adjust FOV for dialogue
            if (Mathf.Abs(dialogueFieldOfView - normalFieldOfView) > 0.1f) {
                StartTransition(targetCamera.transform.position, targetCamera.transform.rotation, dialogueFieldOfView);
            }
        }

        /// <summary>
        /// Handles conversation ended.
        /// </summary>
        private void HandleConversationEnded() {
            // Return to original camera state
            if (targetCamera) {
                StartTransition(_originalPosition, _originalRotation, _originalFov);
            }

            _currentFocusTarget = null;
        }

        /// <summary>
        /// Handles dialogue events for camera commands.
        /// </summary>
        private void HandleConversationEvent(string eventName, SerializableDictionary<string, string> parameters) {
            switch (eventName.ToLower()) {
                case "camerafocus":
                    HandleCameraFocus(parameters);
                    break;
                case "camerareset":
                    HandleCameraReset(parameters);
                    break;
                case "camerashake":
                    HandleCameraShake(parameters);
                    break;
                case "camerazoom":
                    HandleCameraZoom(parameters);
                    break;
            }
        }

        /// <summary>
        /// Handles camera focus event.
        /// </summary>
        private void HandleCameraFocus(SerializableDictionary<string, string> parameters) {
            string targetName = parameters.GetOrDefault("target", "");
            float duration = float.TryParse(parameters.GetOrDefault("duration", "0.5"), out float d) ? d : 0.5f;

            if (string.IsNullOrEmpty(targetName)) {
                return;
            }

            // Find target by name
            GameObject targetObj = GameObject.Find(targetName);
            if (!targetObj) {
                Debug.LogWarning($"ConversationCameraController: Target '{targetName}' not found.");
                return;
            }

            FocusOn(targetObj.transform, duration);
        }

        /// <summary>
        /// Handles camera reset event.
        /// </summary>
        private void HandleCameraReset(SerializableDictionary<string, string> parameters) {
            float duration = float.TryParse(parameters.GetOrDefault("duration", "0.5"), out float d) ? d : 0.5f;

            if (defaultCameraPosition) {
                StartTransition(defaultCameraPosition.position, defaultCameraPosition.rotation, _originalFov, duration);
            }
            else {
                StartTransition(_originalPosition, _originalRotation, _originalFov, duration);
            }
        }

        /// <summary>
        /// Handles camera shake event.
        /// </summary>
        private void HandleCameraShake(SerializableDictionary<string, string> parameters) {
            float intensity = float.TryParse(parameters.GetOrDefault("intensity", "0.5"), out float i) ? i : 0.5f;
            float duration = float.TryParse(parameters.GetOrDefault("duration", "0.3"), out float d) ? d : 0.3f;

            StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        /// <summary>
        /// Handles camera zoom event.
        /// </summary>
        private void HandleCameraZoom(SerializableDictionary<string, string> parameters) {
            float fov = float.TryParse(parameters.GetOrDefault("fov", "40"), out float f) ? f : 40f;
            float duration = float.TryParse(parameters.GetOrDefault("duration", "0.5"), out float d) ? d : 0.5f;

            if (targetCamera) {
                StartTransition(targetCamera.transform.position, targetCamera.transform.rotation, fov, duration);
            }
        }

        /// <summary>
        /// Focuses the camera on a target.
        /// </summary>
        public void FocusOn(Transform target, float duration = 0.5f) {
            if (!target || !targetCamera) {
                return;
            }

            _currentFocusTarget = target;

            // Calculate focus position
            Vector3 focusPosition = target.position + target.forward * -focusDistance + Vector3.up * focusHeight;
            Quaternion focusRotation = Quaternion.LookRotation(target.position + Vector3.up * focusHeight - focusPosition);

            StartTransition(focusPosition, focusRotation, dialogueFieldOfView, duration);
        }

        /// <summary>
        /// Starts a camera transition.
        /// </summary>
        private void StartTransition(Vector3 position, Quaternion rotation, float fov, float duration = 0.5f) {
            if (_transitionCoroutine != null) {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(TransitionCoroutine(position, rotation, fov, duration));
        }

        /// <summary>
        /// Camera transition coroutine.
        /// </summary>
        private IEnumerator TransitionCoroutine(Vector3 targetPosition, Quaternion targetRotation, float targetFov, float duration) {
            if (!targetCamera) {
                yield break;
            }

            Vector3 startPosition = targetCamera.transform.position;
            Quaternion startRotation = targetCamera.transform.rotation;
            float startFov = targetCamera.fieldOfView;

            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

                targetCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                targetCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                targetCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, t);

                yield return null;
            }

            targetCamera.transform.position = targetPosition;
            targetCamera.transform.rotation = targetRotation;
            targetCamera.fieldOfView = targetFov;

            _transitionCoroutine = null;
        }

        /// <summary>
        /// Camera shake coroutine.
        /// </summary>
        private IEnumerator ShakeCoroutine(float intensity, float duration) {
            if (!targetCamera) {
                yield break;
            }

            Vector3 originalPosition = targetCamera.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                float dampening = 1f - (elapsed / duration);

                float x = Random.Range(-1f, 1f) * intensity * dampening;
                float y = Random.Range(-1f, 1f) * intensity * dampening;

                targetCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

                yield return null;
            }

            targetCamera.transform.localPosition = originalPosition;
        }
    }
}