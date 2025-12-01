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
        public float wallCheckDistance = 0.1f;
            
        [Header("Corner Correction / Edge Assist")]
        [Tooltip("Helps player land on ledges when they're slightly off")]
        public bool enableCornerCorrection = true;
        [Tooltip("Maximum horizontal distance to check for ledges")]
        public float cornerCorrectionDistance = 0.15f;
        [Tooltip("Maximum vertical distance to check above player")]
        public float cornerCorrectionHeight = 0.3f;
        [Tooltip("Speed at which player is pushed onto ledge")]
        public float cornerCorrectionSpeed = 3f;
            
        [Header("Detection factors")]
        [Tooltip("Factor used for ledge detection (approx. head height)")]
        public float headLevelFactor = 0.3f;
        [Tooltip("Edge detection horizontal movement threshold")]
        public float movementXThreshold = 0.1f;
        [Tooltip("Box  factor to check for wall detection")]
        public float boxColliderSizeFactor = 0.9f;
    }
}