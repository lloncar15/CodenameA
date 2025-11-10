using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GimGim.Singleton;

namespace GimGim.Input {
    public class InputController : Singleton<InputController> {
        private PlayerInputActions _inputActions;

        public event Action<Vector2> OnMove;
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnDashPressed;
        public event Action OnInteractPressed;
        public event Action OnOpenMenuPressed;
        
        public Vector2 MoveInput { get; private set; }
        public bool IsJumpHeld { get; private set; }

        protected override void Awake() {
            base.Awake();
            _inputActions = new PlayerInputActions();
        }

        private void OnEnable() {
            EnableGameplayInput();
        }

        private void OnDisable() {
            DisableAllInput();
        }

        public void EnableGameplayInput() {
            DisableAllInput();
        
            _inputActions.Gameplay.Enable();
        
            _inputActions.Gameplay.Move.performed += HandleMove;
            _inputActions.Gameplay.Move.canceled += HandleMove;
        
            _inputActions.Gameplay.Jump.performed += HandleJumpPressed;
            _inputActions.Gameplay.Jump.canceled += HandleJumpReleased;
        
            _inputActions.Gameplay.Dash.performed += HandleDashPressed;
            _inputActions.Gameplay.Interact.performed += HandleInteractPressed;
        }

        public void DisableGameplayInput() {
            _inputActions.Gameplay.Move.performed -= HandleMove;
            _inputActions.Gameplay.Move.canceled -= HandleMove;
        
            _inputActions.Gameplay.Jump.performed -= HandleJumpPressed;
            _inputActions.Gameplay.Jump.canceled -= HandleJumpReleased;
        
            _inputActions.Gameplay.Dash.performed -= HandleDashPressed;
            _inputActions.Gameplay.Interact.performed -= HandleInteractPressed;
        
            _inputActions.Gameplay.Disable();
        
            MoveInput = Vector2.zero;
            IsJumpHeld = false;
        }

        public void DisableAllInput() {
            DisableGameplayInput();
        }

        private void HandleMove(InputAction.CallbackContext context) {
            MoveInput = context.ReadValue<Vector2>();
            OnMove?.Invoke(MoveInput);
        }

        private void HandleJumpPressed(InputAction.CallbackContext context) {
            IsJumpHeld = true;
            OnJumpPressed?.Invoke();
        }

        private void HandleJumpReleased(InputAction.CallbackContext context) {
            IsJumpHeld = false;
            OnJumpReleased?.Invoke();
        }

        private void HandleDashPressed(InputAction.CallbackContext context) {
            OnDashPressed?.Invoke();
        }

        private void HandleInteractPressed(InputAction.CallbackContext context) {
            OnInteractPressed?.Invoke();
        }
    }
}