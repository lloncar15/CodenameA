using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GimGim.ConversationSystem.Testing {
    /// <summary>
    /// Helper script to quickly set up a test scene for the conversation system.
    /// Attach to an empty GameObject and click the context menu to generate the UI.
    /// </summary>
    public class ConversationTestSetup : MonoBehaviour {
        [Header("Database References")]
        public ConversationDatabase conversationDatabase;
        public CharacterDatabase characterDatabase;

        [Header("Test Settings")]
        public string testConversationId = "test_conversation";
        public KeyCode startKey = KeyCode.T;

        private ConversationController _controller;
        private bool _isSetup = false;

        [ContextMenu("Generate Test UI")]
        public void GenerateTestUI() {
            // Create Canvas
            GameObject canvasObj = new GameObject("ConversationCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create dialogue panel
            GameObject panelObj = new GameObject("DialoguePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.05f);
            panelRect.anchorMax = new Vector2(0.9f, 0.35f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = panelObj.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Create speaker name
            GameObject nameObj = new GameObject("SpeakerName");
            nameObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(0.3f, 1);
            nameRect.pivot = new Vector2(0, 1);
            nameRect.anchoredPosition = new Vector2(20, -10);
            nameRect.sizeDelta = new Vector2(200, 30);

            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Speaker";
            nameText.fontSize = 18;
            nameText.fontStyle = FontStyles.Bold;

            // Create dialogue text
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(20, 20);
            textRect.offsetMax = new Vector2(-20, -50);

            TextMeshProUGUI dialogueText = textObj.AddComponent<TextMeshProUGUI>();
            dialogueText.text = "Dialogue text appears here...";
            dialogueText.fontSize = 16;

            // Create choices container
            GameObject choicesObj = new GameObject("ChoicesContainer");
            choicesObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform choicesRect = choicesObj.AddComponent<RectTransform>();
            choicesRect.anchorMin = new Vector2(0.1f, 0.4f);
            choicesRect.anchorMax = new Vector2(0.9f, 0.7f);
            choicesRect.offsetMin = Vector2.zero;
            choicesRect.offsetMax = Vector2.zero;

            VerticalLayoutGroup layout = choicesObj.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;

            // Create choice button prefab
            GameObject buttonPrefab = CreateChoiceButtonPrefab();
            buttonPrefab.transform.SetParent(canvasObj.transform, false);
            buttonPrefab.SetActive(false);

            // Add ClassicBoxView
            ClassicBoxView view = panelObj.AddComponent<ClassicBoxView>();
            
            // Use reflection or serialized object to set fields
            var viewType = typeof(ClassicBoxView);
            var baseType = typeof(ConversationViewBase);
            
            SetPrivateField(baseType, view, "canvas", canvas);
            SetPrivateField(baseType, view, "rootPanel", panelRect);
            SetPrivateField(viewType, view, "dialogueText", dialogueText);
            SetPrivateField(viewType, view, "speakerNameText", nameText);
            SetPrivateField(viewType, view, "choicesParent", choicesRect);
            SetPrivateField(viewType, view, "choiceButtonPrefab", buttonPrefab.GetComponent<ConversationChoiceButton>());

            // Add CanvasGroup
            CanvasGroup canvasGroup = panelObj.AddComponent<CanvasGroup>();
            SetPrivateField(baseType, view, "canvasGroup", canvasGroup);

            // Add TypewriterEffect
            TypewriterEffect typewriter = dialogueText.gameObject.AddComponent<TypewriterEffect>();
            SetPrivateField(typeof(TypewriterEffect), typewriter, "targetText", dialogueText);
            SetPrivateField(baseType, view, "typewriter", typewriter);

            // Create Controller
            GameObject controllerObj = new GameObject("ConversationController");
            _controller = controllerObj.AddComponent<ConversationController>();
            
            SetPrivateField(typeof(ConversationController), _controller, "conversationDatabase", conversationDatabase);
            SetPrivateField(typeof(ConversationController), _controller, "characterDatabase", characterDatabase);
            SetPrivateField(typeof(ConversationController), _controller, "view", view);

            _isSetup = true;
            Debug.Log("Test UI generated! Press " + startKey + " to start the conversation.");
        }

        private GameObject CreateChoiceButtonPrefab() {
            GameObject buttonObj = new GameObject("ChoiceButtonPrefab");
            
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 40);

            Image bg = buttonObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = bg;

            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Choice";
            text.fontSize = 14;
            text.alignment = TextAlignmentOptions.MidlineLeft;

            // Add ConversationChoiceButton
            ConversationChoiceButton choiceButton = buttonObj.AddComponent<ConversationChoiceButton>();
            SetPrivateField(typeof(ConversationChoiceButton), choiceButton, "button", button);
            SetPrivateField(typeof(ConversationChoiceButton), choiceButton, "choiceText", text);
            SetPrivateField(typeof(ConversationChoiceButton), choiceButton, "backgroundImage", bg);

            return buttonObj;
        }

        private void SetPrivateField(System.Type type, object obj, string fieldName, object value) {
            var field = type.GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            
            if (field != null) {
                field.SetValue(obj, value);
            }
            else {
                Debug.LogWarning($"Field '{fieldName}' not found on {type.Name}");
            }
        }

        private void Update() {
            if (!_isSetup) return;

            if (UnityEngine.Input.GetKeyDown(startKey) && _controller && !_controller.IsRunning) {
                StartTestConversation();
            }
        }

        private async void StartTestConversation() {
            Debug.Log("Starting test conversation...");
            await _controller.StartConversationAsync(testConversationId);
            Debug.Log("Conversation ended.");
        }
    }
}