using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TypingManager : MonoBehaviour
{
    [field: Header("UI References"), SerializeField]
    public GameObject TypingUIPanel { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI TargetSentenceText { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI PlayerInputText { get; private set; }

    private string sentenceToType;
    private int currentIndex;
    public string playerInputString { get; private set; }
    private Player currentPlayer;
    private PlayerController playerController;

    public void Initialize(GameObject uiPanel, TextMeshProUGUI target, TextMeshProUGUI input)
    {
        this.TypingUIPanel = uiPanel;
        this.TargetSentenceText = target;
        this.PlayerInputText = input;
    }

    private void Awake()
    {
        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Player.Enable();
        Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
        Keyboard.current.onTextInput -= OnTextInput;
    }

    void Start()
    {
        // CHANGED: lowercase t -> uppercase T
        TypingUIPanel.SetActive(false);
    }

    public void StartTypingSession(string sentence, Player player)
    {
        sentenceToType = sentence;
        currentIndex = 0;
        playerInputString = "";

        // CHANGED: lowercase t -> uppercase T
        TargetSentenceText.text = sentenceToType;
        // CHANGED: lowercase p -> uppercase P
        PlayerInputText.text = "";

        // CHANGED: lowercase t -> uppercase T
        TypingUIPanel.SetActive(true);

        currentPlayer = player;
        currentPlayer.LockMovement();
    }

    private void Update()
    {
        if (playerController.Player.TestTyping.WasPressedThisFrame())
        {
            // We need a Player reference to test this now. Find it in the scene.
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                StartTypingSession("The quick brown fox jumps over the lazy dog.", player);
            }
        }

        // CHANGED: lowercase t -> uppercase T
        if (!TypingUIPanel.activeInHierarchy)
        {
            return;
        }

        HandleBackspaceInput();
        HandleEnterInput();
    }

    private void OnTextInput(char character)
    {
        // It calls the public method that contains the actual logic
        ProcessCharacter(character);
    }

    public void ProcessCharacter(char character)
    {
        if (!TypingUIPanel.activeInHierarchy) return;
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
        if (playerController.Player.Backspace.WasPressedThisFrame())
        {
            ProcessBackspace();
        }
    }

    private void HandleEnterInput()
    {
        if (playerController.Player.Enter.WasPressedThisFrame())
        {
            ProcessEnter();
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
        PlayerInputText.text = richText;
    }

    public void ProcessBackspace()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            playerInputString = playerInputString.Substring(0, playerInputString.Length - 1);
            Debug.Log("Backspace used! Applying time penalty.");
            UpdateVisuals();
        }
    }

    public void ProcessEnter()
    {
        if (playerInputString.Equals(sentenceToType))
        {
            Debug.Log("Success! Package Delivered.");
        }
        else
        {
            Debug.Log("Failed! Input does not match.");
        }

        // This happens on both success and failure
        TypingUIPanel.SetActive(false);
        currentPlayer.UnlockMovement();
    }
}