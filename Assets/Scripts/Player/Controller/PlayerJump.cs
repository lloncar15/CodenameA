using System;
using UnityEngine;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerJump : MonoBehaviour {
        [Serializable]
        public class JumpSettings {
            [Header("Jump")]
            public float jumpForce = 14f;
            public float minJumpForce = 7f;
            public float gravityScale = 3f;
            public float fallGravityMultiplier = 1.5f;
            public float lowJumpGravityMultiplier = 2f;

            [Header("Jump Buffering")]
            public float coyoteTime = 0.15f;
            public float jumpBufferTime = 0.1f;

            [Header("Air Control")] 
            public int maxAirJumps;
        }
        
        [Header("Jump Settings")]
        public JumpSettings settings;

        private Rigidbody2D _rb;
        private float _originalGravityScale;
        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;
        private int _airJumpsRemaining;
        private bool _isJumping;
        private bool _isJumpHeld;
        
        public bool IsJumping => _isJumping;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _originalGravityScale = settings.gravityScale;
            _rb.gravityScale = _originalGravityScale;
        }

        public void OnJumpPressed() {
            _isJumpHeld = true;
            _jumpBufferCounter = settings.jumpBufferTime;
        }

        public void OnJumpReleased() {
            _isJumpHeld = false;
        }

        public void UpdateTimers(bool isGrounded, bool wasGrounded) {
            if (isGrounded) {
                _coyoteTimeCounter = settings.coyoteTime;
            }
            else {
                _coyoteTimeCounter -= Time.deltaTime;
            }

            if (_jumpBufferCounter > 0) {
                _jumpBufferCounter -= Time.deltaTime;
            }

            if (isGrounded && !wasGrounded) {
                _airJumpsRemaining = settings.maxAirJumps;
                _isJumping = false;
            }
        }

        public bool TryJump(bool isGrounded, Action onPlatformDetach = null) {
            if (_jumpBufferCounter <= 0)
                return false;

            if (_coyoteTimeCounter > 0 && !_isJumping) {
                PerformJump(settings.jumpForce, onPlatformDetach);
                _jumpBufferCounter = 0;
                _coyoteTimeCounter = 0;
                return true;
            }
            else if (!isGrounded && _airJumpsRemaining > 0) {
                PerformJump(settings.jumpForce, onPlatformDetach);
                _airJumpsRemaining--;
                _jumpBufferCounter = 0;
                return true;
            }

            return false;
        }

        private void PerformJump(float force, Action onPlatformDetach) {
            onPlatformDetach?.Invoke();
            _rb.linearVelocityY = force;
            _isJumping = true;
        }

        public void ApplyGravityModifiers(bool isDashing) {
            if (isDashing)
                return;

            if (_rb.linearVelocityY < 0) {
                _rb.gravityScale = _originalGravityScale * settings.fallGravityMultiplier;
            }
            else if (_rb.linearVelocityY > 0 && !_isJumpHeld) {
                _rb.gravityScale = _originalGravityScale * settings.lowJumpGravityMultiplier;
            }
            else {
                _rb.gravityScale = _originalGravityScale;
            }
        }

        public void ApplyVariableJumpHeight() {
            if (!_isJumpHeld && _rb.linearVelocityY > settings.minJumpForce && _isJumping) {
                _rb.linearVelocityY = settings.minJumpForce;
            }
        }
        
        public void ResetGravity() {
            _rb.gravityScale = _originalGravityScale;
        }
    }
}