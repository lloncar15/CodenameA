// Assets/Scripts/ConversationSystem/Core/Variables/ConversationStateManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Manages conversation-specific state including variables, visited nodes, and choices.
    /// Also implements IConversationVariableProvider to provide state variables to conditions.
    /// </summary>
    public class ConversationStateManager : IConversationStateManager, IConversationVariableProvider {
        private ConversationStateData _data;

        /// <summary>
        /// Event raised when a variable is changed.
        /// </summary>
        public event Action<string, object> OnVariableChanged;

        /// <summary>
        /// Event raised when a node is visited.
        /// </summary>
        public event Action<string, string, int> OnNodeVisited; // conversationId, nodeId, visitCount

        /// <summary>
        /// Event raised when a choice is made.
        /// </summary>
        public event Action<string, string, string> OnChoiceMade; // conversationId, nodeId, choiceId

        /// <inheritdoc />
        public int Priority { get; set; } = 100; // High priority for conversation state

        /// <summary>
        /// Creates a new state manager with empty state.
        /// </summary>
        public ConversationStateManager() {
            _data = new ConversationStateData();
        }

        /// <summary>
        /// Creates a state manager with existing state data.
        /// </summary>
        /// <param name="data">The state data to use.</param>
        public ConversationStateManager(ConversationStateData data) {
            _data = data ?? new ConversationStateData();
        }

        /// <summary>
        /// Gets the underlying state data for serialization.
        /// </summary>
        /// <returns>The current state data.</returns>
        public ConversationStateData GetStateData() {
            _data.UpdateTimestamp();
            return _data;
        }

        /// <summary>
        /// Loads state from data.
        /// </summary>
        /// <param name="data">The state data to load.</param>
        public void LoadStateData(ConversationStateData data) {
            _data = data ?? new ConversationStateData();
        }

        /// <summary>
        /// Resets all state to empty.
        /// </summary>
        public void Reset() {
            _data = new ConversationStateData();
        }

        #region Variable Operations

        /// <inheritdoc />
        public void SetBool(string key, bool value) {
            _data.BoolVariables[key] = value;
            OnVariableChanged?.Invoke(key, value);
        }

        /// <inheritdoc />
        public bool GetBool(string key, bool defaultValue = false) {
            return _data.BoolVariables.GetValueOrDefault(key, defaultValue);
        }

        /// <inheritdoc />
        public void SetInt(string key, int value) {
            _data.IntVariables[key] = value;
            OnVariableChanged?.Invoke(key, value);
        }

        /// <inheritdoc />
        public int GetInt(string key, int defaultValue = 0) {
            return _data.IntVariables.GetValueOrDefault(key, defaultValue);
        }

        /// <inheritdoc />
        public void SetFloat(string key, float value) {
            _data.FloatVariables[key] = value;
            OnVariableChanged?.Invoke(key, value);
        }

        /// <inheritdoc />
        public float GetFloat(string key, float defaultValue = 0f) {
            return _data.FloatVariables.GetValueOrDefault(key, defaultValue);
        }

        /// <inheritdoc />
        public void SetString(string key, string value) {
            _data.StringVariables[key] = value;
            OnVariableChanged?.Invoke(key, value);
        }

        /// <inheritdoc />
        public string GetString(string key, string defaultValue = "") {
            return _data.StringVariables.GetValueOrDefault(key, defaultValue);
        }

        /// <inheritdoc cref="IConversationVariableProvider.HasVariable" />
        public bool HasVariable(string key) {
            return _data.BoolVariables.ContainsKey(key) ||
                   _data.IntVariables.ContainsKey(key) ||
                   _data.FloatVariables.ContainsKey(key) ||
                   _data.StringVariables.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool RemoveVariable(string key) {
            bool removed = false;
            removed |= _data.BoolVariables.Remove(key);
            removed |= _data.IntVariables.Remove(key);
            removed |= _data.FloatVariables.Remove(key);
            removed |= _data.StringVariables.Remove(key);
            return removed;
        }

        #endregion

        #region Node Visit Tracking

        /// <inheritdoc />
        public void RecordNodeVisit(string conversationId, string nodeId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            
            int count = _data.NodeVisitCounts.GetValueOrDefault(key, 0);

            count++;
            _data.NodeVisitCounts[key] = count;
            
            OnNodeVisited?.Invoke(conversationId, nodeId, count);
        }

        /// <inheritdoc />
        public int GetNodeVisitCount(string conversationId, string nodeId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            return _data.NodeVisitCounts.GetValueOrDefault(key, 0);
        }

        /// <inheritdoc />
        public bool WasNodeVisited(string conversationId, string nodeId) {
            return GetNodeVisitCount(conversationId, nodeId) > 0;
        }

        #endregion

        #region Choice Tracking

        /// <inheritdoc />
        public void RecordChoice(string conversationId, string nodeId, string choiceId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);

            if (!_data.ChoiceHistory.TryGetValue(key, out List<string> choices)) {
                choices = new List<string>();
                _data.ChoiceHistory[key] = choices;
            }

            choices.Add(choiceId);
            
            OnChoiceMade?.Invoke(conversationId, nodeId, choiceId);
        }

        /// <inheritdoc />
        public string GetLastChoice(string conversationId, string nodeId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            
            if (_data.ChoiceHistory.TryGetValue(key, out List<string> choices) && choices.Count > 0) {
                return choices[^1];
            }

            return null;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetAllChoices(string conversationId, string nodeId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            
            if (_data.ChoiceHistory.TryGetValue(key, out List<string> choices)) {
                return choices.AsReadOnly();
            }

            return Array.Empty<string>();
        }

        #endregion

        #region Random Selection Tracking

        /// <summary>
        /// Records the last random selection for a node.
        /// Used for AvoidRepeat functionality.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <param name="nodeId">The node ID.</param>
        /// <param name="selectionIndex">The index that was selected.</param>
        public void RecordRandomSelection(string conversationId, string nodeId, int selectionIndex) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            _data.LastRandomSelections[key] = selectionIndex;
        }

        /// <summary>
        /// Gets the last random selection for a node.
        /// </summary>
        /// <param name="conversationId">The conversation ID.</param>
        /// <param name="nodeId">The node ID.</param>
        /// <returns>The last selection index, or -1 if none.</returns>
        public int GetLastRandomSelection(string conversationId, string nodeId) {
            string key = ConversationStateData.MakeNodeKey(conversationId, nodeId);
            return _data.LastRandomSelections.GetValueOrDefault(key, -1);
        }

        #endregion

        #region IConversationVariableProvider Implementation

        /// <inheritdoc />
        bool IConversationVariableProvider.TryGetBool(string variableName, out bool value) {
            return _data.BoolVariables.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        bool IConversationVariableProvider.TryGetInt(string variableName, out int value) {
            return _data.IntVariables.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        bool IConversationVariableProvider.TryGetFloat(string variableName, out float value) {
            return _data.FloatVariables.TryGetValue(variableName, out value);
        }

        /// <inheritdoc />
        bool IConversationVariableProvider.TryGetString(string variableName, out string value) {
            return _data.StringVariables.TryGetValue(variableName, out value);
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Sets a metadata value.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The value to set.</param>
        public void SetMetadata(string key, string value) {
            _data.Metadata[key] = value;
        }

        /// <summary>
        /// Gets a metadata value.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">Default if not found.</param>
        /// <returns>The metadata value or default.</returns>
        public string GetMetadata(string key, string defaultValue = "") {
            return _data.Metadata.GetValueOrDefault(key, defaultValue);
        }

        #endregion
    }
}