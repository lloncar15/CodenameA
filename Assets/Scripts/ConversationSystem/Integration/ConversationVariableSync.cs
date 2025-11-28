using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Syncs conversation variables with game state.
    /// Allows bidirectional updates between dialogue and game systems.
    /// </summary>
    public class ConversationVariableSync : MonoBehaviour, IConversationVariableProvider {
        [Header("Controller Reference")]
        [SerializeField]
        private ConversationController controller;

        [Header("Variable Bindings")]
        [SerializeField]
        private List<BoolVariableBinding> boolBindings = new List<BoolVariableBinding>();

        [SerializeField]
        private List<IntVariableBinding> intBindings = new List<IntVariableBinding>();

        [SerializeField]
        private List<FloatVariableBinding> floatBindings = new List<FloatVariableBinding>();

        [SerializeField]
        private List<StringVariableBinding> stringBindings = new List<StringVariableBinding>();

        [Header("Settings")]
        [SerializeField]
        private int priority = 100;

        [SerializeField]
        private bool registerOnAwake = true;

        // Lookup caches
        private Dictionary<string, BoolVariableBinding> _boolLookup;
        private Dictionary<string, IntVariableBinding> _intLookup;
        private Dictionary<string, FloatVariableBinding> _floatLookup;
        private Dictionary<string, StringVariableBinding> _stringLookup;

        public int Priority => priority;

        private void Awake() {
            BuildLookups();

            if (controller == null) {
                controller = FindAnyObjectByType<ConversationController>();
            }

            if (registerOnAwake && controller != null) {
                controller.RegisterVariableProvider(this);
            }
        }

        /// <summary>
        /// Builds lookup dictionaries for fast access.
        /// </summary>
        private void BuildLookups() {
            _boolLookup = new Dictionary<string, BoolVariableBinding>(StringComparer.OrdinalIgnoreCase);
            _intLookup = new Dictionary<string, IntVariableBinding>(StringComparer.OrdinalIgnoreCase);
            _floatLookup = new Dictionary<string, FloatVariableBinding>(StringComparer.OrdinalIgnoreCase);
            _stringLookup = new Dictionary<string, StringVariableBinding>(StringComparer.OrdinalIgnoreCase);

            foreach (var binding in boolBindings) {
                if (!string.IsNullOrEmpty(binding.variableName)) {
                    _boolLookup[binding.variableName] = binding;
                }
            }

            foreach (var binding in intBindings) {
                if (!string.IsNullOrEmpty(binding.variableName)) {
                    _intLookup[binding.variableName] = binding;
                }
            }

            foreach (var binding in floatBindings) {
                if (!string.IsNullOrEmpty(binding.variableName)) {
                    _floatLookup[binding.variableName] = binding;
                }
            }

            foreach (var binding in stringBindings) {
                if (!string.IsNullOrEmpty(binding.variableName)) {
                    _stringLookup[binding.variableName] = binding;
                }
            }
        }

        #region IConversationVariableProvider

        public bool TryGetBool(string variableName, out bool value) {
            value = false;
            
            if (_boolLookup.TryGetValue(variableName, out BoolVariableBinding binding)) {
                value = binding.getValue?.Invoke() ?? binding.defaultValue;
                return true;
            }
            
            return false;
        }

        public bool TryGetInt(string variableName, out int value) {
            value = 0;
            
            if (_intLookup.TryGetValue(variableName, out IntVariableBinding binding)) {
                value = binding.getValue?.Invoke() ?? binding.defaultValue;
                return true;
            }
            
            return false;
        }

        public bool TryGetFloat(string variableName, out float value) {
            value = 0;
            
            if (_floatLookup.TryGetValue(variableName, out FloatVariableBinding binding)) {
                value = binding.getValue?.Invoke() ?? binding.defaultValue;
                return true;
            }
            
            return false;
        }

        public bool TryGetString(string variableName, out string value) {
            value = null;
            
            if (_stringLookup.TryGetValue(variableName, out StringVariableBinding binding)) {
                value = binding.getValue?.Invoke() ?? binding.defaultValue;
                return true;
            }
            
            return false;
        }

        public bool HasVariable(string variableName) {
            return _boolLookup.ContainsKey(variableName) ||
                   _intLookup.ContainsKey(variableName) ||
                   _floatLookup.ContainsKey(variableName) ||
                   _stringLookup.ContainsKey(variableName);
        }

        #endregion

        /// <summary>
        /// Registers a bool variable binding at runtime.
        /// </summary>
        public void RegisterBool(string variableName, Func<bool> getter, Action<bool> setter = null) {
            var binding = new BoolVariableBinding {
                variableName = variableName,
                getValue = getter,
                setValue = setter
            };
            
            boolBindings.Add(binding);
            _boolLookup[variableName] = binding;
        }

        /// <summary>
        /// Registers an int variable binding at runtime.
        /// </summary>
        public void RegisterInt(string variableName, Func<int> getter, Action<int> setter = null) {
            var binding = new IntVariableBinding {
                variableName = variableName,
                getValue = getter,
                setValue = setter
            };
            
            intBindings.Add(binding);
            _intLookup[variableName] = binding;
        }

        /// <summary>
        /// Registers a float variable binding at runtime.
        /// </summary>
        public void RegisterFloat(string variableName, Func<float> getter, Action<float> setter = null) {
            var binding = new FloatVariableBinding {
                variableName = variableName,
                getValue = getter,
                setValue = setter
            };
            
            floatBindings.Add(binding);
            _floatLookup[variableName] = binding;
        }

        /// <summary>
        /// Registers a string variable binding at runtime.
        /// </summary>
        public void RegisterString(string variableName, Func<string> getter, Action<string> setter = null) {
            var binding = new StringVariableBinding {
                variableName = variableName,
                getValue = getter,
                setValue = setter
            };
            
            stringBindings.Add(binding);
            _stringLookup[variableName] = binding;
        }
    }

    /// <summary>
    /// Base class for variable bindings.
    /// </summary>
    [Serializable]
    public abstract class VariableBindingBase {
        [Tooltip("The variable name used in conversations.")]
        public string variableName;
    }

    /// <summary>
    /// Bool variable binding.
    /// </summary>
    [Serializable]
    public class BoolVariableBinding : VariableBindingBase {
        public bool defaultValue;
        
        [NonSerialized]
        public Func<bool> getValue;
        
        [NonSerialized]
        public Action<bool> setValue;

        public UnityEvent<bool> onValueChanged;
    }

    /// <summary>
    /// Int variable binding.
    /// </summary>
    [Serializable]
    public class IntVariableBinding : VariableBindingBase {
        public int defaultValue;
        
        [NonSerialized]
        public Func<int> getValue;
        
        [NonSerialized]
        public Action<int> setValue;

        public UnityEvent<int> onValueChanged;
    }

    /// <summary>
    /// Float variable binding.
    /// </summary>
    [Serializable]
    public class FloatVariableBinding : VariableBindingBase {
        public float defaultValue;
        
        [NonSerialized]
        public Func<float> getValue;
        
        [NonSerialized]
        public Action<float> setValue;

        public UnityEvent<float> onValueChanged;
    }

    /// <summary>
    /// String variable binding.
    /// </summary>
    [Serializable]
    public class StringVariableBinding : VariableBindingBase {
        public string defaultValue;
        
        [NonSerialized]
        public Func<string> getValue;
        
        [NonSerialized]
        public Action<string> setValue;

        public UnityEvent<string> onValueChanged;
    }
}