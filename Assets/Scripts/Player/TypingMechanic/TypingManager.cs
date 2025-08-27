using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Using the new Input System

public class TypingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject typingUIPanel;
    [SerializeField] private TextMeshProUGUI targetSentenceText;
    [SerializeField] private TextMeshProUGUI playerInputText;

    private string sentenceToType;
    private int currentIndex;
    private string playerInputString;

    private PlayerController playerController;

    // --- SETUP METHODS ---
    private void Awake()
    {
        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Player.Enable();
        // CHANGED: Subscribe to the text input event
        Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
        // CHANGED: Unsubscribe to prevent memory leaks
        Keyboard.current.onTextInput -= OnTextInput;
    }

    void Start()
    {
        typingUIPanel.SetActive(false);
    }

    // --- PUBLIC API ---
    public void StartTypingSession(string sentence)
    {
        sentenceToType = sentence;
        currentIndex = 0;
        playerInputString = "";

        targetSentenceText.text = sentenceToType;
        playerInputText.text = "";

        typingUIPanel.SetActive(true);
    }

    // --- CORE LOGIC ---
    private void Update()
    {
        // TEMPORARY FOR TESTING
        if (playerController.Player.TestTyping.WasPressedThisFrame())
        {
            StartTypingSession("he quick brown fox jumps over the lazy dog.");
        }

        if (!typingUIPanel.activeInHierarchy)
        {
            return;
        }

        // REMOVED: HandleCharacterInput() is no longer called here.
        HandleBackspaceInput();
        HandleEnterInput();
    }

    // NEW METHOD: This is called automatically by the Input System for each typed character.
    private void OnTextInput(char character)
    {
        // Don't process input if the panel isn't active.
        if (!typingUIPanel.activeInHierarchy) return;

        // Ignore control characters.
        if (character == '\b' || character == '\n' || character == '\r') return;

        if (currentIndex < sentenceToType.Length)
        {
            playerInputString += character;
            currentIndex++;
            UpdateVisuals();
        }
    }

    private void HandleBackspaceInput()
    {
        if (playerController.Player.Backspace.WasPressedThisFrame() && currentIndex > 0)
        {
            currentIndex--;
            playerInputString = playerInputString.Substring(0, playerInputString.Length - 1);

            Debug.Log("Backspace used! Applying time penalty.");
            UpdateVisuals();
        }
    }

    private void HandleEnterInput()
    {
        if (playerController.Player.Enter.WasPressedThisFrame())
        {
            if (playerInputString.Equals(sentenceToType))
            {
                Debug.Log("Success! Package Delivered.");
                typingUIPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Failed! Input does not match.");
            }
        }
    }

    private void UpdateVisuals()
    {
        string richText = "";
        for (int i = 0; i < playerInputString.Length; i++)
        {
            if (i < sentenceToType.Length && playerInputString[i] == sentenceToType[i])
            {
                richText += $"<color=green>{playerInputString[i]}</color>";
            }
            else
            {
                richText += $"<color=red>{playerInputString[i]}</color>";
            }
        }
        playerInputText.text = richText;
    }
}