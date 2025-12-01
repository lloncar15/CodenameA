using UnityEngine;

namespace GimGim.Player.Controller {
    
    [CreateAssetMenu(fileName = "DashSettings", menuName = "GimGim/Player/Dash Settings")]
    public class DashSettings : ScriptableObject {
        [Header("Dash")] public float dashSpeed = 20f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 1f;

        [Header("Dash Physics")] 
        public bool disableGravityDuringDash = true;
        public float onDashEndedVelocityFactor = 0.5f;
    }
}