using UnityEngine;

namespace GimGim.Player.Controller {
    
    [CreateAssetMenu(fileName = "JumpSettings", menuName = "GimGim/Player/Jump Settings")]
    public class JumpSettings : ScriptableObject {
        [Header("Jump")]
        public float jumpForce = 14f;
        public float minJumpForce = 7f;
        public float gravityScale = 3f;
        public float fallGravityMultiplier = 1.5f;
        public float lowJumpGravityMultiplier = 2f;
            
        [Header("Jump Apex (Hang Time)")]
        [Tooltip("Reduces gravity at the peak of the jump for more responsive feel")]
        public bool enableHangTime = true;
        [Tooltip("Vertical velocity threshold to activate hang time")]
        public float hangTimeThreshold = 2f;
        [Tooltip("Gravity multiplier during hang time (lower = more floaty)")]
        public float hangTimeGravityMultiplier = 0.5f;

        [Header("Jump Buffering")]
        public float coyoteTime = 0.15f;
        public float jumpBufferTime = 0.1f;

        [Header("Air Control")] 
        public int maxAirJumps;
    }
}