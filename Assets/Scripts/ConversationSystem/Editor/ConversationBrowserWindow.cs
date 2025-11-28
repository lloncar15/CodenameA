#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GimGim.ConversationSystem.Editor {
    /// <summary>
    /// Editor window for browsing and managing conversations.
    /// </summary>
    public class ConversationBrowserWindow : EditorWindow {
        // References
        private ConversationDatabase _database;
        private List<ConversationData> _loadedConversations;
        private ConversationData _selectedConversation;
        private ConversationNode _selectedNode;

        // UI State
        private Vector2 _conversationListScroll;
        private Vector2 _nodeListScroll;
        private Vector2 _detailsScroll;
        private string _searchFilter = "";
        private int _selectedConversationIndex = -1;
        private int _selectedNodeIndex = -1;

        // Validation
        private ConversationValidator _validator;
        private bool _showValidationResults = false;

        // Styles
        private GUIStyle _headerStyle;
        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _errorStyle;
        private GUIStyle _warningStyle;
        private bool _stylesInitialized = false;

        [MenuItem("GimGim/Conversation System/Conversation Browser")]
        public static void ShowWindow() {
            ConversationBrowserWindow window = GetWindow<ConversationBrowserWindow>();
            window.titleContent = new GUIContent("Conversation Browser");
            window.minSize = new Vector2(800, 500);
            window.Show();
        }

        private void OnEnable() {
            _validator = new ConversationValidator();
            _loadedConversations = new List<ConversationData>();
            LoadDatabase();
        }

        private void InitStyles() {
            if (_stylesInitialized) {
                return;
            }

            _headerStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 14,
                margin = new RectOffset(5, 5, 10, 10)
            };

            _nodeStyle = new GUIStyle(EditorStyles.label) {
                padding = new RectOffset(5, 5, 3, 3),
                margin = new RectOffset(0, 0, 1, 1)
            };

            _selectedNodeStyle = new GUIStyle(_nodeStyle) {
                normal = { background = MakeTexture(1, 1, new Color(0.24f, 0.49f, 0.91f, 0.5f)) }
            };

            _errorStyle = new GUIStyle(EditorStyles.label) {
                normal = { textColor = new Color(1f, 0.3f, 0.3f) }
            };

            _warningStyle = new GUIStyle(EditorStyles.label) {
                normal = { textColor = new Color(1f, 0.8f, 0.2f) }
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeTexture(int width, int height, Color color) {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void LoadDatabase() {
            // Try to find a ConversationDatabase in the project
            string[] guids = AssetDatabase.FindAssets("t:ConversationDatabase");
            if (guids.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _database = AssetDatabase.LoadAssetAtPath<ConversationDatabase>(path);
                
                if (_database != null) {
                    _database.Initialize();
                    RefreshConversationList();
                }
            }
        }

        private void RefreshConversationList() {
            _loadedConversations.Clear();

            if (!_database) {
                return;
            }

            foreach (string id in _database.GetAllConversationIds()) {
                ConversationData conversation = _database.GetConversation(id);
                if (conversation != null) {
                    _loadedConversations.Add(conversation);
                }
            }
        }

        private void OnGUI() {
            InitStyles();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawToolbar();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            // Left panel - Conversation list
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            DrawConversationList();
            EditorGUILayout.EndVertical();

            // Middle panel - Node list
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawNodeList();
            EditorGUILayout.EndVertical();

            // Right panel - Details
            EditorGUILayout.BeginVertical();
            DrawDetailsPanel();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            // Bottom panel - Validation results
            if (_showValidationResults) {
                DrawValidationResults();
            }
        }

        private void DrawToolbar() {
            // Database selection
            EditorGUI.BeginChangeCheck();
            _database = (ConversationDatabase)EditorGUILayout.ObjectField(_database, typeof(ConversationDatabase), false, GUILayout.Width(200));
            if (EditorGUI.EndChangeCheck()) {
                if (_database) {
                    _database.Initialize();
                }
                RefreshConversationList();
                _selectedConversation = null;
                _selectedNode = null;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                RefreshConversationList();
            }

            if (GUILayout.Button("Validate All", EditorStyles.toolbarButton, GUILayout.Width(80))) {
                ValidateAllConversations();
            }

            GUILayout.FlexibleSpace();

            // Search
            GUILayout.Label("Search:", GUILayout.Width(50));
            _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(150));

            if (GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(20))) {
                _searchFilter = "";
            }
        }

        private void DrawConversationList() {
            EditorGUILayout.LabelField("Conversations", _headerStyle);

            _conversationListScroll = EditorGUILayout.BeginScrollView(_conversationListScroll);

            for (int i = 0; i < _loadedConversations.Count; i++) {
                ConversationData conversation = _loadedConversations[i];

                // Apply search filter
                if (!string.IsNullOrEmpty(_searchFilter)) {
                    if (!conversation.Id.ToLower().Contains(_searchFilter.ToLower()) &&
                        !conversation.Name.ToLower().Contains(_searchFilter.ToLower())) {
                        continue;
                    }
                }

                bool isSelected = _selectedConversationIndex == i;
                GUIStyle style = isSelected ? _selectedNodeStyle : _nodeStyle;

                EditorGUILayout.BeginHorizontal(style);
                
                if (GUILayout.Button(conversation.Name ?? conversation.Id, EditorStyles.label)) {
                    SelectConversation(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Add from JSON button
            EditorGUILayout.Space();
            if (GUILayout.Button("Load from JSON...")) {
                LoadConversationFromJson();
            }
        }

        private void DrawNodeList() {
            EditorGUILayout.LabelField("Nodes", _headerStyle);

            if (_selectedConversation == null) {
                EditorGUILayout.LabelField("Select a conversation", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            // Quick stats
            EditorGUILayout.LabelField($"Total: {_selectedConversation.Nodes.Count} nodes", EditorStyles.miniLabel);
            EditorGUILayout.Space();

            _nodeListScroll = EditorGUILayout.BeginScrollView(_nodeListScroll);

            int index = 0;
            foreach (var kvp in _selectedConversation.Nodes) {
                bool isSelected = _selectedNodeIndex == index;
                bool isStart = kvp.Key == _selectedConversation.StartNodeId;
                GUIStyle style = isSelected ? _selectedNodeStyle : _nodeStyle;

                EditorGUILayout.BeginHorizontal(style);

                // Node type icon/label
                string typeLabel = GetNodeTypeLabel(kvp.Value.NodeType);
                GUILayout.Label(typeLabel, GUILayout.Width(20));

                // Node ID
                string displayName = kvp.Key;
                if (isStart) {
                    displayName += " [START]";
                }

                if (GUILayout.Button(displayName, EditorStyles.label)) {
                    SelectNode(index, kvp.Value);
                }

                EditorGUILayout.EndHorizontal();
                index++;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDetailsPanel() {
            EditorGUILayout.LabelField("Details", _headerStyle);

            _detailsScroll = EditorGUILayout.BeginScrollView(_detailsScroll);

            if (_selectedNode != null) {
                DrawNodeDetails(_selectedNode);
            }
            else if (_selectedConversation != null) {
                DrawConversationDetails(_selectedConversation);
            }
            else {
                EditorGUILayout.LabelField("Select a conversation or node", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawConversationDetails(ConversationData conversation) {
            EditorGUILayout.LabelField("Conversation Info", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("ID:", conversation.Id);
            EditorGUILayout.LabelField("Name:", conversation.Name ?? "(none)");
            EditorGUILayout.LabelField("Start Node:", conversation.StartNodeId ?? "(none)");
            EditorGUILayout.LabelField("Node Count:", conversation.Nodes.Count.ToString());

            if (!string.IsNullOrEmpty(conversation.Description)) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Description:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(conversation.Description, EditorStyles.wordWrappedLabel);
            }

            if (conversation.ParticipantIds != null && conversation.ParticipantIds.Count > 0) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Participants:", EditorStyles.boldLabel);
                foreach (var participant in conversation.ParticipantIds) {
                    EditorGUILayout.LabelField($"  • {participant}");
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Validate")) {
                ValidateConversation(conversation);
            }

            if (GUILayout.Button("Export JSON")) {
                ExportConversationJson(conversation);
            }
        }

        private void DrawNodeDetails(ConversationNode node) {
            EditorGUILayout.LabelField($"{node.NodeType} Node", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("ID:", node.Id);
            EditorGUILayout.LabelField("Type:", node.NodeType.ToString());

            EditorGUILayout.Space();

            switch (node) {
                case TextNode textNode:
                    DrawTextNodeDetails(textNode);
                    break;
                case ChoiceNode choiceNode:
                    DrawChoiceNodeDetails(choiceNode);
                    break;
                case BranchNode branchNode:
                    DrawBranchNodeDetails(branchNode);
                    break;
                case EventNode eventNode:
                    DrawEventNodeDetails(eventNode);
                    break;
                case RandomNode randomNode:
                    DrawRandomNodeDetails(randomNode);
                    break;
                case WaitNode waitNode:
                    DrawWaitNodeDetails(waitNode);
                    break;
                case JumpNode jumpNode:
                    DrawJumpNodeDetails(jumpNode);
                    break;
            }
        }

        private void DrawTextNodeDetails(TextNode node) {
            EditorGUILayout.LabelField("Speaker:", node.SpeakerId ?? "(narrator)");
            
            if (!string.IsNullOrEmpty(node.Expression)) {
                EditorGUILayout.LabelField("Expression:", node.Expression);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(node.Text ?? "(empty)", EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Next Node:", node.NextNodeId ?? "(end)");
            EditorGUILayout.LabelField("Requires Input:", node.RequiresInput.ToString());
            
            if (node.AutoAdvanceDelay > 0) {
                EditorGUILayout.LabelField("Auto Advance:", $"{node.AutoAdvanceDelay}s");
            }

            if (node.OnEnterCommands != null && node.OnEnterCommands.Count > 0) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"OnEnter Commands: {node.OnEnterCommands.Count}");
            }

            if (node.OnExitCommands != null && node.OnExitCommands.Count > 0) {
                EditorGUILayout.LabelField($"OnExit Commands: {node.OnExitCommands.Count}");
            }
        }

        private void DrawChoiceNodeDetails(ChoiceNode node) {
            if (!string.IsNullOrEmpty(node.PromptText)) {
                EditorGUILayout.LabelField("Prompt:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(node.PromptText, EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField($"Choices ({node.Choices?.Count ?? 0}):", EditorStyles.boldLabel);

            if (node.Choices != null) {
                foreach (var choice in node.Choices) {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"• {choice.Text}");
                    EditorGUILayout.LabelField($"  ID: {choice.Id}, Next: {choice.NextNodeId ?? "(end)"}");
                    EditorGUILayout.EndVertical();
                }
            }

            if (node.TimeLimit > 0) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Time Limit: {node.TimeLimit}s");
                EditorGUILayout.LabelField($"Timeout Node: {node.TimeoutNodeId ?? "(none)"}");
            }
        }

        private void DrawBranchNodeDetails(BranchNode node) {
            EditorGUILayout.LabelField($"Branches ({node.Branches?.Count ?? 0}):", EditorStyles.boldLabel);

            if (node.Branches != null) {
                foreach (BranchOption branch in node.Branches) {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"Target: {branch.NextNodeId}");
                    EditorGUILayout.LabelField($"Priority: {branch.Priority}");
                    if (branch.Condition != null) {
                        EditorGUILayout.LabelField($"Condition: {branch.Condition.Type}");
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Node:", node.DefaultNodeId ?? "(none)");
        }

        private void DrawEventNodeDetails(EventNode node) {
            EditorGUILayout.LabelField($"Commands ({node.Commands?.Count ?? 0}):", EditorStyles.boldLabel);

            if (node.Commands != null) {
                foreach (CommandData cmd in node.Commands) {
                    EditorGUILayout.LabelField($"• {cmd.CommandType}");
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Next Node:", node.NextNodeId ?? "(end)");
        }

        private void DrawRandomNodeDetails(RandomNode node) {
            EditorGUILayout.LabelField($"Options ({node.Options?.Count ?? 0}):", EditorStyles.boldLabel);

            if (node.Options != null) {
                float totalWeight = 0f;
                foreach (RandomOption opt in node.Options) {
                    totalWeight += opt.Weight;
                }

                foreach (RandomOption opt in node.Options) {
                    float percent = totalWeight > 0 ? (opt.Weight / totalWeight * 100f) : 0f;
                    EditorGUILayout.LabelField($"• {opt.NextNodeId} (Weight: {opt.Weight}, {percent:F1}%)");
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Avoid Repeat:", node.AvoidRepeat.ToString());
        }

        private void DrawWaitNodeDetails(WaitNode node) {
            EditorGUILayout.LabelField("Wait Type:", node.WaitType.ToString());

            switch (node.WaitType) {
                case WaitType.Time:
                    EditorGUILayout.LabelField("Duration:", $"{node.Duration}s");
                    break;
                case WaitType.Condition:
                    EditorGUILayout.LabelField("Condition:", node.WaitCondition?.Type ?? "(none)");
                    break;
                case WaitType.Event:
                    EditorGUILayout.LabelField("Event Name:", node.WaitEventName ?? "(none)");
                    break;
            }

            if (node.Timeout > 0) {
                EditorGUILayout.LabelField("Timeout:", $"{node.Timeout}s");
                EditorGUILayout.LabelField("Timeout Node:", node.TimeoutNodeId ?? "(none)");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Next Node:", node.NextNodeId ?? "(end)");
        }

        private void DrawJumpNodeDetails(JumpNode node) {
            if (!string.IsNullOrEmpty(node.TargetConversationId)) {
                EditorGUILayout.LabelField("Target Conversation:", node.TargetConversationId);
                EditorGUILayout.LabelField("Start Node:", node.TargetConversationStartNodeId ?? "(default)");
            }
            else {
                EditorGUILayout.LabelField("Target Node:", node.TargetNodeId ?? "(none)");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Return After:", node.ReturnAfterTarget.ToString());
            
            if (node.ReturnAfterTarget) {
                EditorGUILayout.LabelField("Return Node:", node.ReturnNodeId ?? "(none)");
            }
        }

        private void DrawValidationResults() {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Validation Results", _headerStyle);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(150));

            if (_validator.Results.Count == 0) {
                EditorGUILayout.LabelField("No validation results", EditorStyles.centeredGreyMiniLabel);
            }
            else {
                EditorGUILayout.LabelField(_validator.GetSummary());
                EditorGUILayout.Space();

                foreach (ValidationResult result in _validator.Results) {
                    GUIStyle style = result.Severity switch {
                        ValidationSeverity.Error => _errorStyle,
                        ValidationSeverity.Warning => _warningStyle,
                        _ => EditorStyles.label
                    };

                    EditorGUILayout.LabelField($"[{result.NodeId}] {result.Message}", style);
                }
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Hide")) {
                _showValidationResults = false;
            }
        }

        private string GetNodeTypeLabel(ConversationNodeType nodeType) {
            return nodeType switch {
                ConversationNodeType.Text => "T",
                ConversationNodeType.Choice => "C",
                ConversationNodeType.Branch => "B",
                ConversationNodeType.Event => "E",
                ConversationNodeType.Random => "R",
                ConversationNodeType.Wait => "W",
                ConversationNodeType.Jump => "J",
                _ => "?"
            };
        }

        private void SelectConversation(int index) {
            _selectedConversationIndex = index;
            _selectedConversation = _loadedConversations[index];
            _selectedNode = null;
            _selectedNodeIndex = -1;
            _showValidationResults = false;
        }

        private void SelectNode(int index, ConversationNode node) {
            _selectedNodeIndex = index;
            _selectedNode = node;
        }

        private void LoadConversationFromJson() {
            string path = EditorUtility.OpenFilePanel("Load Conversation JSON", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            try {
                string json = System.IO.File.ReadAllText(path);
                ConversationData conversation = ConversationJsonUtility.FromJson(json);
                
                if (conversation != null) {
                    _loadedConversations.Add(conversation);
                    SelectConversation(_loadedConversations.Count - 1);
                    Debug.Log($"Loaded conversation: {conversation.Id}");
                }
            }
            catch (System.Exception ex) {
                EditorUtility.DisplayDialog("Error", $"Failed to load conversation: {ex.Message}", "OK");
            }
        }

        private void ValidateConversation(ConversationData conversation) {
            _validator.Validate(conversation);
            _showValidationResults = true;
        }

        private void ValidateAllConversations() {
            int errors = 0;
            int warnings = 0;

            foreach (ConversationData conversation in _loadedConversations) {
                _validator.Validate(conversation);
                
                foreach (ValidationResult result in _validator.Results) {
                    if (result.Severity == ValidationSeverity.Error) {
                        errors++;
                    }
                    else if (result.Severity == ValidationSeverity.Warning) {
                        warnings++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Validation Complete", 
                $"Validated {_loadedConversations.Count} conversations.\n{errors} errors, {warnings} warnings.", 
                "OK");

            _showValidationResults = true;
        }

        private void ExportConversationJson(ConversationData conversation) {
            string defaultName = $"{conversation.Id}.json";
            string path = EditorUtility.SaveFilePanel("Export Conversation JSON", Application.dataPath, defaultName, "json");
            
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            try {
                string json = ConversationJsonUtility.ToJson(conversation, true);
                System.IO.File.WriteAllText(path, json);
                Debug.Log($"Exported conversation to: {path}");
            }
            catch (System.Exception ex) {
                EditorUtility.DisplayDialog("Error", $"Failed to export conversation: {ex.Message}", "OK");
            }
        }
    }
}
#endif