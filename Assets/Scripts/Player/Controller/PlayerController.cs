using System;
using GimGim.Input;
using UnityEngine;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerDash))]
    [RequireComponent(typeof(PlayerPlatformHandler))]
    public class PlayerController : MonoBehaviour {
        private PlayerMovement _movement;
        private PlayerJump _jump;
        private PlayerDash _dash;
        private PlayerPlatformHandler _platformHandler;

        private Vector2 _currentMoveInput;

        private void Awake() {
            _movement  = GetComponent<PlayerMovement>();
            _jump  = GetComponent<PlayerJump>();
            _dash  = GetComponent<PlayerDash>();
            _platformHandler = GetComponent<PlayerPlatformHandler>();
        }

        private void OnEnable() {
            InputController controller = InputController.Instance;
            
            controller.OnMove += HandleMove;
            controller.OnJumpPressed += HandleJumpPressed;
            controller.OnJumpReleased += HandleJumpReleased;
            controller.OnDashPressed += HandleDashPressed;
            controller.OnInteractPressed += HandleInteract;
        }

        private void OnDisable() {
            InputController controller = InputController.Instance;
            
            controller.OnMove -= HandleMove;
            controller.OnJumpPressed -= HandleJumpPressed;
            controller.OnJumpReleased -= HandleJumpReleased;
            controller.OnDashPressed -= HandleDashPressed;
            controller.OnInteractPressed -= HandleInteract;
        }

        private void Update() {
            _movement.CheckGround();
            _jump.UpdateTimers(_movement.IsGrounded, _movement.WasGrounded);
            _dash.UpdateCooldown();
        }

        private void FixedUpdate() {
            _platformHandler.HandlePlatform(_movement.IsGrounded,
                _movement.settings.groundLayer,
                _movement.settings.groundCheckSize,
                _movement.settings.groundCheckDistance);

            if (_dash.IsDashing) {
                _dash.ApplyDash();
            }
            else {
                _movement.ApplyMovement();

                _jump.TryJump(_movement.IsGrounded, _platformHandler.DetachFromPlatform);
                _jump.ApplyVariableJumpHeight();
            }
            
            _jump.ApplyGravityModifiers(_dash.IsDashing);

            if (!_dash.IsDashing) {
                _dash.ResetGravity(_jump.settings.gravityScale);
            }
        }

        private void HandleMove(Vector2 input) {
            _currentMoveInput = input;
            _movement.SetMoveInput(input);
        }

        private void HandleJumpPressed() {
            _jump.OnJumpPressed();
        }

        private void HandleJumpReleased() {
            _jump.OnJumpReleased();
        }

        private void HandleDashPressed() {
            _dash.StartDash(_currentMoveInput, _movement.FacingDirection, _platformHandler.DetachFromPlatform);
        }

        private void HandleInteract() {
            Debug.Log("Interact pressed");
        }
    }
}