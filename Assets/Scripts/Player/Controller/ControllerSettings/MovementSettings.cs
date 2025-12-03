using System;
using UnityEngine;

namespace GimGim.Player.Controller {
    
    [CreateAssetMenu(fileName = "Movement Settings", menuName = "GimGim/Player/Movement Settings")]
    [Serializable]
    public class MovementSettings : ScriptableObject {
        [Header("Movement")] 
        public float moveSpeed = 7f;
        public float acceleration = 50f;
        public float deceleration = 50f;
        public float airAcceleration = 30f;
        public float airDeceleration = 30f;
        
        [Header("Ground Detection")]
        public LayerMask groundLayer;
        public Vector2 groundCheckSize = new(0.9f, 0.1f);
        public float groundCheckDistance = 0.05f;
            
        [Header("Wall Detection")]
        [Tooltip("Prevents player from sticking to walls when moving into them")]
        public bool preventWallStick = true;
        [Tooltip("Distance to check for walls")]
        public float wallCheckDistance = 0.1f;
        [Tooltip("Size of the box used for wall detection (matches ground detection style)")]
        public Vector2 wallCheckSize = new Vector2(0.1f, 0.9f);
            
        [Header("Edge Assist")]
        [Tooltip("Helps player land on ledges when feet barely miss")]
        public bool enableLedgeAssist = true;
        [Tooltip("The height factor to use for edge assist")]
        public float feetLevelFactor = 0.1f;
        [Tooltip("Maximum horizontal distance to check for ledges")]
        public float ledgeAssistDistance = 0.15f;
        [Tooltip("Maximum vertical distance to check above player feet")]
        public float ledgeAssistHeight = 0.3f;
        [Tooltip("Speed at which player is pushed onto ledge")]
        public float ledgeAssistSpeed = 3f;
        [Tooltip("Edge detection horizontal movement threshold")]
        public float movementXThreshold = 0.1f;
        
        [Header("Ceiling Bump Guard")]
        [Tooltip("Pushes player sideways when head barely hits ceiling")]
        public bool enableCeilingBumpGuard = true;
        [Tooltip("The height factor to use for bump guard")]
        public float headLevelFactor = 0.8f;
        [Tooltip("Maximum horizontal distance to push player away from ceiling")]
        public float ceilingBumpDistance = 0.15f;
        [Tooltip("Maximum vertical distance to check below ceiling")]
        public float ceilingBumpCheckHeight = 0.2f;
        [Tooltip("Speed at which player is pushed away from ceiling")]
        public float ceilingBumpSpeed = 4f;
    }
}