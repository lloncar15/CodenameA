using GimGim.Utility.Logger;
using UnityEngine;
using ColorPalette = GimGim.Utility.ColorPalette;

namespace GimGim.Player.Controller {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerMovement : MonoBehaviour {
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

            if (settings.preventWallStick && !IsGrounded && IsAgainstWall()) {
                targetSpeed = 0;
            }
                
            float accelRate;
            float deAccelRate;

            if (IsGrounded) {
                accelRate = settings.acceleration;
                deAccelRate = settings.deceleration;
            }
            else {
                accelRate = settings.airAcceleration;
                deAccelRate = settings.airDeceleration;
            }

            float speedDif = targetSpeed - _rb.linearVelocityX;
            float rate = Mathf.Abs(targetSpeed) > 0.01f ? accelRate : deAccelRate;
            float movement = speedDif * rate * Time.fixedDeltaTime;

            _rb.linearVelocity = new Vector2(_rb.linearVelocityX + movement, _rb.linearVelocityY);
        }

        /// <summary>
        /// Applies corner assist to help player land on ledges when feet barely miss
        /// and to help player dodge ceilings when head barely hits.
        /// Checks at head and feet level, works only when rising
        /// </summary>
        public void ApplyCornerAssist() {
            if (!settings.enableLedgeAssist)
                return;
            
            if (IsGrounded || _rb.linearVelocityY <= 0 || Mathf.Abs(_moveInput.x) < settings.movementXThreshold)
                return;

            Vector2 direction = GetFacingDirection();
            
            if (DetectLedgeAtFeet(direction, out float feetPushDistance))
            {
                float push = feetPushDistance * settings.ledgeAssistSpeed * Time.fixedDeltaTime;
                _rb.linearVelocityX += push;
                Debug.Log($"Pushed the player for {push}");
                return;
            }
            
            if (DetectCornerAtHead(direction, out float headPushDistance)) {
                float push = headPushDistance * settings.ceilingBumpSpeed * Time.fixedDeltaTime;
                _rb.linearVelocityX += push;
                Debug.Log($"Pushed the player for {push}");
            }
        }

        /// <summary>
        /// Detects if there's a ledge nearby that the player should be assisted onto or away from
        /// </summary>
        /// <param name="direction">Direction to check (left or right)</param>
        /// <param name="pushDistance">How far to push player toward ledge</param>
        /// <returns>True if ledge detected</returns>
        private bool DetectLedgeAtFeet(Vector2 direction, out float pushDistance) {
            pushDistance = 0f;
            
            Vector2 playerCenter = (Vector2)transform.position + _boxCollider.offset;
            Vector2 playerSize = _boxCollider.size;

            float playerBottom = playerCenter.y - playerSize.y * 0.5f;
            float checkHeight = playerBottom + settings.feetLevelFactor;
            Vector2 horizontalCheckStart = new(playerCenter.x, checkHeight);
            
            RaycastHit2D horizontalHit = Physics2D.Raycast(
                horizontalCheckStart,
                direction,
                settings.ledgeAssistDistance,
                settings.groundLayer);

            if (!horizontalHit.collider)
                return false;
            
            Vector2 topCheckPos = horizontalHit.point + Vector2.up * settings.ledgeAssistHeight;
            RaycastHit2D topHit = Physics2D.Raycast(
                topCheckPos,
                Vector2.down,
                settings.ledgeAssistHeight,
                settings.groundLayer);

            if (!topHit.collider)
                return false;
            
            float targetX = topHit.point.x;
            pushDistance = (targetX - playerCenter.x) * direction.x;
            
            // Only push if we're close enough and the ledge is above us
            if (!(pushDistance > 0) || !(pushDistance < settings.ledgeAssistDistance)) 
                return false;
            
            float ledgeHeight = topHit.point.y;

            return ledgeHeight > playerBottom && ledgeHeight < playerBottom + settings.ledgeAssistHeight;
        }
        
        /// <summary>
        /// Detects if there's a collider nearby that the player should be assisted clearing while jumping
        /// </summary>
        /// <param name="direction">Direction to check (left or right)</param>
        /// <param name="pushDistance">How far to push player away from the corner</param>
        /// <returns>True if ledge detected</returns>
        private bool DetectCornerAtHead(Vector2 direction, out float pushDistance) {
            pushDistance = 0f;
            
            Vector2 playerCenter = (Vector2)transform.position + _boxCollider.offset;
            Vector2 playerSize = _boxCollider.size;

            float headHeight = playerCenter.y + playerSize.y * settings.headLevelFactor;
            Vector2 horizontalCheckStart = new(playerCenter.x, headHeight);
            
            RaycastHit2D horizontalHit = Physics2D.Raycast(
                horizontalCheckStart,
                direction,
                settings.ceilingBumpDistance,
                settings.groundLayer);

            if (!horizontalHit.collider)
                return false;
            
            Vector2 ceilingCheckPos = horizontalHit.point + Vector2.down * settings.ceilingBumpCheckHeight;
            RaycastHit2D ceilingHit = Physics2D.Raycast(
                ceilingCheckPos,
                Vector2.up,
                settings.ceilingBumpCheckHeight,
                settings.groundLayer);

            if (!ceilingHit.collider)
                return false;

            float ceilingHeight = ceilingHit.point.y;
            float playerTop = playerCenter.y + playerSize.y * 0.5f;

            if (!(ceilingHeight > playerTop) || !(ceilingHeight < playerTop + settings.ceilingBumpCheckHeight))
                return false;

            pushDistance = settings.ceilingBumpDistance * direction.x;

            return true;
        }
        
        /// <summary>
        /// Checks if the player is against a wall in the direction they're moving
        /// </summary>
        /// <returns>True if player is touching a wall</returns>
        private bool IsAgainstWall() {
            if (Mathf.Abs(_moveInput.x) < settings.movementXThreshold)
                return false;
            
            Vector2 facingDirection = GetFacingDirection();
            Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset;

            Vector2 wallCheckCenter = boxCenter + facingDirection * settings.wallCheckDistance;

            bool isAgainstWall = Physics2D.OverlapBox(
                wallCheckCenter,
                settings.wallCheckSize,
                0f,
                settings.groundLayer);

            return isAgainstWall;
        }

        private void UpdateFacingDirection() {
            if (_moveInput.x > 0.1f)
                _facingDirection = 1;
            else if (_moveInput.x < -0.1f)
                _facingDirection = -1;
        }

        private Vector2 GetFacingDirection() {
            return _moveInput.x > 0 ? Vector2.right : Vector2.left;
        }

        private void OnDrawGizmosSelected() {
            if (!_boxCollider)
                _boxCollider = GetComponent<BoxCollider2D>();
            
            // Ground check visualization
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset + Vector2.down * settings.groundCheckDistance;
            Gizmos.DrawWireCube(boxCenter, settings.groundCheckSize);
            
            Vector2 playerCenter = (Vector2)transform.position + _boxCollider.offset;
            Vector2 playerSize = _boxCollider.size;
            
            // Wall check visualization
            if (settings.preventWallStick) {
                Gizmos.color = Color.yellow;
            
                // Draw wall check boxes on both sides
                Vector2 rightCheckCenter = playerCenter + Vector2.right * settings.wallCheckDistance;
                Gizmos.DrawWireCube(rightCheckCenter, settings.wallCheckSize);
            
                Vector2 leftCheckCenter = playerCenter + Vector2.left * settings.wallCheckDistance;
                Gizmos.DrawWireCube(leftCheckCenter, settings.wallCheckSize);
            }
            
            // Corner correction visualization
            if (settings.enableLedgeAssist) {
                Gizmos.color = Color.cyan;
            
                float playerBottom = playerCenter.y - playerSize.y * 0.5f;
                float feetHeight = playerBottom + settings.feetLevelFactor;
            
                // Draw horizontal detection rays
                Gizmos.DrawRay(new Vector2(playerCenter.x, feetHeight), Vector2.right * settings.ledgeAssistDistance);
                Gizmos.DrawRay(new Vector2(playerCenter.x, feetHeight), Vector2.left * settings.ledgeAssistDistance);
            
                // Draw vertical detection range
                Gizmos.color = Color.magenta;
                Vector2 rightPoint = new(playerCenter.x + settings.ledgeAssistDistance, feetHeight);
                Vector2 leftPoint = new(playerCenter.x - settings.ledgeAssistDistance, feetHeight);
                Gizmos.DrawRay(rightPoint, Vector2.up * settings.ledgeAssistHeight);
                Gizmos.DrawRay(leftPoint, Vector2.up * settings.ledgeAssistHeight);
            }

            // Ceiling bump guard visualization
            if (settings.enableCeilingBumpGuard) {
                Gizmos.color = Color.cyan;
            
                float headHeight = playerCenter.y - playerSize.y * settings.headLevelFactor;
            
                // Draw horizontal detection rays
                Gizmos.DrawRay(new Vector2(playerCenter.x, headHeight), Vector2.right * settings.ceilingBumpDistance);
                Gizmos.DrawRay(new Vector2(playerCenter.x, headHeight), Vector2.left * settings.ceilingBumpDistance);
            
                // Draw vertical detection range
                Gizmos.color = Color.magenta;
                Vector2 rightPoint = new(playerCenter.x + settings.ceilingBumpDistance, headHeight);
                Vector2 leftPoint = new(playerCenter.x - settings.ceilingBumpDistance, headHeight);
                Gizmos.DrawRay(rightPoint, Vector2.up * settings.ceilingBumpCheckHeight);
                Gizmos.DrawRay(leftPoint, Vector2.up * settings.ceilingBumpCheckHeight);
            }
        }
    }
}
