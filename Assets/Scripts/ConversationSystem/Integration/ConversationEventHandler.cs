using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Handles specific dialogue events by name and routes them to UnityEvents.
    /// </summary>
    public class ConversationEventHandler : MonoBehaviour {
        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [SerializeField]
        private bool findControllerAutomatically = true;

        [Header("Event Mappings")]
        [SerializeField]
        private List<ConversationEventMapping> eventMappings = new List<ConversationEventMapping>();

        private Dictionary<string, ConversationEventMapping> _eventLookup;

        private void Awake() {
            if (controller == null && findControllerAutomatically) {
                controller = FindAnyObjectByType<ConversationController>();
            }

            BuildEventLookup();
        }

        private void OnEnable() {
            if (controller != null) {
                controller.OnConversationEvent += HandleConversationEvent;
            }
        }

        private void OnDisable() {
            if (controller != null) {
                controller.OnConversationEvent -= HandleConversationEvent;
            }
        }

        /// <summary>
        /// Builds the event lookup dictionary.
        /// </summary>
        private void BuildEventLookup() {
            _eventLookup = new Dictionary<string, ConversationEventMapping>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var mapping in eventMappings) {
                if (!string.IsNullOrEmpty(mapping.eventName)) {
                    _eventLookup[mapping.eventName] = mapping;
                }
            }
        }

        /// <summary>
        /// Handles a dialogue event.
        /// </summary>
        private void HandleConversationEvent(string eventName, SerializableDictionary<string, string> parameters) {
            if (_eventLookup.TryGetValue(eventName, out ConversationEventMapping mapping)) {
                mapping.response?.Invoke();
                mapping.responseWithParams?.Invoke(parameters);

                if (mapping.logEvent) {
                    Debug.Log($"ConversationEventHandler: Event '{eventName}' triggered.");
                }
            }
            else {
                Debug.LogWarning($"ConversationEventHandler: No mapping for event '{eventName}'.");
            }
        }

        /// <summary>
        /// Registers an event handler at runtime.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="handler">The handler action.</param>
        public void RegisterHandler(string eventName, Action handler) {
            if (!_eventLookup.TryGetValue(eventName, out ConversationEventMapping mapping)) {
                mapping = new ConversationEventMapping { eventName = eventName };
                eventMappings.Add(mapping);
                _eventLookup[eventName] = mapping;
            }

            mapping.response.AddListener(() => handler());
        }

        /// <summary>
        /// Unregisters all handlers for an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        public void UnregisterHandlers(string eventName) {
            if (_eventLookup.TryGetValue(eventName, out ConversationEventMapping mapping)) {
                mapping.response.RemoveAllListeners();
                mapping.responseWithParams.RemoveAllListeners();
            }
        }
    }

    /// <summary>
    /// Maps a dialogue event name to UnityEvents.
    /// </summary>
    [Serializable]
    public class ConversationEventMapping {
        [Tooltip("The event name to listen for.")]
        public string eventName;

        [Tooltip("Response when event is triggered.")]
        public UnityEvent response;

        [Tooltip("Response with parameters when event is triggered.")]
        public ConversationEventParamsResponse responseWithParams;

        [Tooltip("Log when this event is triggered.")]
        public bool logEvent = false;
    }

    /// <summary>
    /// UnityEvent with SerializableDictionary parameter.
    /// </summary>
    [Serializable]
    public class ConversationEventParamsResponse : UnityEvent<SerializableDictionary<string, string>> { }
}