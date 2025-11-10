using System;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace GimGim.Objects {
    public class MovingPlatform : MonoBehaviour {
        public enum MovementType {
            Loop,
            PingPong
        }

        [Serializable]
        public class MovingPlatformSettings {
            [Header("Movement")] 
            public float moveSpeed = 2f;
            public MovementType movementType = MovementType.PingPong;
            public Ease ease = Ease.InOutSine;

            [Header("Waypoints")]
            [Tooltip("Array of transforms representing the waypoints the platform will move through.")]
            public Transform[] waypoints;
            
            [Header("Wait Time")]
            [Tooltip("Time to wait at each waypoint before moving to the next (in seconds)")]
            public float waitTimeAtWaypoint;
            
            [Header("Start Settings")]
            [Tooltip("Should the platform start moving immediately?")]
            public bool startMovingOnAwake = true;
            [Tooltip("Starting waypoint index (0-based)")]
            public int startingWaypointIndex;
        }

        [Header("Platform Settings")] 
        public MovingPlatformSettings settings;

        private int _currentWaypointIndex;
        private int _direction = 1;
        private bool _isMoving;
        private Tween _currentTween;
        
        private const float GIZMOS_WAYPOINT_RADIUS = 0.3f;
        private const float GIZMOS_WAYPOINT_FACTOR = 0.5f;

        private void Start() {
            if (settings.waypoints == null || settings.waypoints.Length == 0) {
                enabled = false;
                return;
            }

            if (!ValidateWaypoints())
                return;
            
            _currentWaypointIndex = Mathf.Clamp(settings.startingWaypointIndex, 0, settings.waypoints.Length - 1);
            
            if (settings.waypoints[_currentWaypointIndex])
                transform.position = settings.waypoints[_currentWaypointIndex].position;

            if (settings.waypoints.Length > 1 && settings.startMovingOnAwake) {
                _isMoving = true;
                MoveToNextWaypoint();
            }
        }

        /// <summary>
        /// Called when the platform reaches a waypoint
        /// </summary>
        private void OnWaypointReached() {
            if (_isMoving)
                MoveToNextWaypoint();
        }

        /// <summary>
        /// Moves the platform to the next waypoint using PrimeTween
        /// </summary>
        private void MoveToNextWaypoint() {
            if (!_isMoving || settings.waypoints == null || settings.waypoints.Length <= 1)
                return;

            int nextWaypointIndex = GetNextWaypointIndex();

            if (!settings.waypoints[nextWaypointIndex])
                return;
            
            Vector3 targetPosition = settings.waypoints[nextWaypointIndex].position;
            float distance =  Vector3.Distance(transform.position, targetPosition);
            float duration = distance / settings.moveSpeed;
            
            _currentTween = Tween.Position(transform,
                targetPosition,
                duration,
                ease: settings.ease,
                startDelay: settings.waitTimeAtWaypoint
                ).OnComplete(this, target => target.OnWaypointReached());
            
            _currentWaypointIndex = nextWaypointIndex;
        }

        /// <summary>
        /// Calculates the next waypoint index based on the movement type
        /// </summary>
        /// <returns>Next waypoint index</returns>
        private int GetNextWaypointIndex() {
            int nextIndex;
            
            if (settings.movementType == MovementType.Loop) {
                nextIndex = (_currentWaypointIndex + 1) % settings.waypoints.Length;
            }
            else {
                nextIndex = _currentWaypointIndex + _direction;

                if (nextIndex >= settings.waypoints.Length) {
                    nextIndex = settings.waypoints.Length - 2;
                    _direction = -1;
                }
                else if (nextIndex < 0) {
                    nextIndex = 1;
                    _direction = 1;
                }
            }
            
            return Mathf.Clamp(nextIndex, 0, settings.waypoints.Length - 1);
        }

        /// <summary>
        /// Start or resume platform movement
        /// </summary>
        public void StartMoving() {
            if (_isMoving) 
                return;
            
            _isMoving = true;
            if (!_currentTween.isAlive) {
                MoveToNextWaypoint();
            }
        }

        /// <summary>
        /// Stop platform movement
        /// </summary>
        public void StopMoving() {
            _isMoving = false;
            if (_currentTween.isAlive) {
                _currentTween.Stop();
            }
        }

        /// <summary>
        /// Toggle platform movement on/off
        /// </summary>
        public void ToggleMovement() {
            if (_isMoving)
                StopMoving();
            else
                StartMoving();
        }

        /// <summary>
        /// Set the movement speed at runtime (useful for sound-based modifications)
        /// Restarts the current tween with new speed if platform is moving
        /// </summary>
        /// <param name="speed">New movement speed</param>
        public void SetMoveSpeed(float speed) {
            settings.moveSpeed = Mathf.Max(0, speed);

            if (_isMoving && _currentTween.isAlive) {
                _currentTween.Stop();
                MoveToNextWaypoint();
            }
        }

        /// <summary>
        /// Get the current movement speed
        /// </summary>
        /// <returns>Current move speed</returns>
        public float GetMoveSpeed() {
            return settings.moveSpeed;
        }
        
        /// <summary>
        /// Sets the easing function for platform movement
        /// </summary>
        /// <param name="ease">Ease function to use</param>
        public void SetEase(Ease ease) {
            settings.ease = ease;
        
            if (_isMoving && _currentTween.isAlive) {
                _currentTween.Stop();
                MoveToNextWaypoint();
            }
        }

        private bool ValidateWaypoints() {
            return settings.waypoints.All(t => t);
        }

        private void OnDrawGizmos() {
            if (settings.waypoints == null)
                return;
            
            int waypointsLength = settings.waypoints.Length;
            if (settings.waypoints == null || waypointsLength == 0)
                return;
            
            Gizmos.color = Color.cyan;

            for (int i = 0; i < waypointsLength; i++) {
                if (!settings.waypoints[i])
                    continue;
                
                Vector3 waypointPosition = settings.waypoints[i].position;
                
                Gizmos.DrawWireSphere(waypointPosition, GIZMOS_WAYPOINT_RADIUS);
                
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(waypointPosition + Vector3.up * GIZMOS_WAYPOINT_FACTOR, $"WP {name}");
                #endif

                if (i < waypointsLength - 1 && settings.waypoints[i + 1]) {
                    Gizmos.DrawLine(waypointPosition, settings.waypoints[i + 1].position);
                }
            }

            if (settings.movementType == MovementType.Loop 
                && waypointsLength > 1 
                && settings.waypoints[0]
                && settings.waypoints[waypointsLength - 1]) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(settings.waypoints[0].position, settings.waypoints[waypointsLength - 1].position);
            }

            if (Application.isPlaying) {
                return;
            }
            
            int startIndex = Mathf.Clamp(settings.startingWaypointIndex, 0, waypointsLength - 1);
            if (settings.waypoints[startIndex]) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(settings.waypoints[startIndex].position, Vector3.one * GIZMOS_WAYPOINT_FACTOR);
            }
        }

        private void OnDrawGizmosSelected() {
            if (settings.waypoints == null)
                return;
            
            Gizmos.color = Color.green;

            foreach (Transform waypoint in settings.waypoints) {
                if (!waypoint)
                    continue;
                
                Vector3 waypointPosition = waypoint.position;
                Gizmos.DrawSphere(waypointPosition, GIZMOS_WAYPOINT_RADIUS);
            }
        }
    }
}
