using System;
using UnityEngine;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDash : MonoBehaviour {
        [Serializable]
        public class DashSettings {
            [Header("Dash")] public float dashSpeed = 20f;
            public float dashDuration = 0.15f;
            public float dashCooldown = 1f;

            [Header("Dash Physics")] 
            public bool disableGravityDuringDash = true;
            public float onDashEndedVelocityFactor = 0.5f;
        }
        
        [Header("Dash Settings")]
        public DashSettings settings;

        private Rigidbody2D _rb;
        private float _dashTimeRemaining;
        private float _dashCooldownRemaining;
        private Vector2 _dashDirection;
        
        public bool IsDashing { get; private set; }
        public bool CanDash => _dashCooldownRemaining <= 0 && !IsDashing;

        private void Awake() {
            _rb =  GetComponent<Rigidbody2D>();
        }

        public void UpdateCooldown() {
            if (_dashCooldownRemaining <= 0)
                return;
            
            _dashCooldownRemaining -= Time.deltaTime;
        }

        public void StartDash(Vector2 moveInput, int facingDirection, Action onPlatformDetach = null) {
            if (!CanDash)
                return;

            IsDashing = true;
            _dashTimeRemaining = settings.dashDuration;
            _dashCooldownRemaining = settings.dashCooldown;

            _dashDirection = moveInput.magnitude > 0.1f ? moveInput.normalized : new Vector2(facingDirection, 0);

            if (settings.disableGravityDuringDash)
                _rb.gravityScale = 0;
            
            onPlatformDetach?.Invoke();
        }

        public void ApplyDash() {
            if (!IsDashing)
                return;

            _dashTimeRemaining -= Time.fixedDeltaTime;

            if (_dashTimeRemaining > 0) {
                _rb.linearVelocity = _dashDirection * settings.dashSpeed;
            }
            else {
                EndDash();
            }
        }

        private void EndDash() {
            IsDashing = false;

            _rb.linearVelocity = _dashDirection * settings.dashSpeed * settings.onDashEndedVelocityFactor;
        }

        public void ResetGravity(float gravityScale) {
            if (!IsDashing) {
                _rb.gravityScale = gravityScale;
            }
        }
    }
}