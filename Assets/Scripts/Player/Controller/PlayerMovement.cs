using System;
using UnityEngine;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerMovement : MonoBehaviour {
        [Serializable]
        public class MovementSettings {
            [Header("Movement")] 
            public float moveSpeed = 7f;
            public float acceleration = 50f;
            public float deceleration = 50f;
            public float airAcceleration = 30f;
            public float airDeceleration = 30f;
        
            [Header("Ground Detection")]
            public LayerMask groundLayer;
            public Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);
            public float groundCheckDistance = 0.05f;
        }

        [Header("Movement Settings")] 
        public MovementSettings settings;

        private Rigidbody2D _rb;
        private BoxCollider2D _boxCollider;
        private Vector2 _moveInput;
        private int _facingDirection = 1;
        
        public bool IsGrounded { get; private set; }
        public bool WasGrounded { get; private set; }
        
        public int FacingDirection => _facingDirection;
        public Rigidbody2D Rigidbody => _rb;
        public BoxCollider2D BoxCollider => _boxCollider;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        public void SetMoveInput(Vector2 moveInput) {
            _moveInput = moveInput;
            UpdateFacingDirection();
        }

        public void CheckGround() {
            WasGrounded = IsGrounded;

            Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset +
                                Vector2.down * settings.groundCheckDistance;
            IsGrounded = Physics2D.OverlapBox(boxCenter, 
                settings.groundCheckSize, 0f, settings.groundLayer);
        }

        public void ApplyMovement(bool canMove = true) {
            if (!canMove)
                return;
            
            float targetSpeed = _moveInput.x * settings.moveSpeed;
            float accelRate;
            float decelRate;

            if (IsGrounded) {
                accelRate = settings.acceleration;
                decelRate = settings.deceleration;
            }
            else {
                accelRate = settings.airAcceleration;
                decelRate = settings.airDeceleration;
            }

            float speedDif = targetSpeed - _rb.linearVelocityX;
            float rate = Mathf.Abs(targetSpeed) > 0.01f ? accelRate : decelRate;
            float movement = speedDif * rate * Time.fixedDeltaTime;

            _rb.linearVelocity = new Vector2(_rb.linearVelocityX + movement, _rb.linearVelocityY);
        }

        private void UpdateFacingDirection() {
            if (_moveInput.x > 0.1f)
                _facingDirection = 1;
            else if (_moveInput.x < -0.1f)
                _facingDirection = -1;
        }

        private void OnDrawGizmosSelected() {
            if (!_boxCollider)
                _boxCollider = GetComponent<BoxCollider2D>();
            
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset + Vector2.down * settings.groundCheckDistance;
            Gizmos.DrawWireCube(boxCenter, settings.groundCheckSize);
        }
    }
}
