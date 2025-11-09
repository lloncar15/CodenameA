using System;
using UnityEngine;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerPlatformHandler : MonoBehaviour {
        private Transform _currentPlatform;
        private BoxCollider2D _collider;

        private void Awake() {
            _collider = GetComponent<BoxCollider2D>();
        }

        public void HandlePlatform(bool isGrounded, LayerMask groundLayer, 
            Vector2 groundCheckSize, float groundCheckDistance) {
            if (!isGrounded) {
                DetachFromPlatform();
                return;
            }

            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + _collider.offset,
                groundCheckSize,
                0f,
                Vector2.down,
                groundCheckDistance,
                groundLayer);

            if (!hit.collider) 
                return;
            
            Transform platformTransform = hit.collider.transform;

            if (platformTransform != transform.parent && platformTransform.CompareTag("MovingPlatform")) {
                if (_currentPlatform != platformTransform) {
                    _currentPlatform = platformTransform;
                    transform.SetParent(_currentPlatform);
                }
            }
            else if (_currentPlatform && !platformTransform.CompareTag("MovingPlatform")) {
                DetachFromPlatform();
            }
        }

        public void DetachFromPlatform() {
            if (!_currentPlatform)
                return;
            
            transform.SetParent(null);
            _currentPlatform = null;
        }
    }
}