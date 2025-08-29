using UnityEngine;
using NUnit.Framework;
using TMPro;

public class TypingManagerTests
{
    private TypingManager manager;
    private GameObject managerObject;
    private GameObject panel;
    private TextMeshProUGUI targetText;
    private TextMeshProUGUI inputText;

    // This method with the [SetUp] attribute runs automatically before every single test.
    // It's perfect for creating a clean environment for each test.
    [SetUp]
    public void Setup()
    {
        // ARRANGE - Part 1: Create all the necessary objects and components

        // Create a GameObject to hold the TypingManager
        managerObject = new GameObject();
        manager = managerObject.AddComponent<TypingManager>();

        // --- START OF CORRECTIONS ---
        // 1. Create a parent panel object.
        panel = new GameObject("TestTypingPanel");

        // 2. Create SEPARATE GameObjects for each text element and parent them to the panel.
        GameObject targetTextObject = new GameObject("TargetText");
        targetTextObject.transform.SetParent(panel.transform);
        targetText = targetTextObject.AddComponent<TextMeshProUGUI>();

        GameObject inputTextObject = new GameObject("InputText");
        inputTextObject.transform.SetParent(panel.transform);
        inputText = inputTextObject.AddComponent<TextMeshProUGUI>();

        // 3. Create a simple "mock" Player object for the test.
        // It only needs the Player component so the LockMovement() method exists.
        GameObject mockPlayerObject = new GameObject();
        mockPlayerObject.AddComponent<Rigidbody2D>(); // Add Rigidbody2D to avoid errors if LockMovement accesses it
        mockPlayerObject.AddComponent<Player>();
        // --- END OF CORRECTIONS ---


        // Link the mock UI components to the manager's fields
        manager.Initialize(panel, targetText, inputText);
    }

    [Test]
    public void StartTypingSession_InitializesStateCorrectly()
    {
        // ARRANGE - Part 2: Get the mock player created in Setup
        Player mockPlayer = Object.FindObjectOfType<Player>();
        string testSentence = "hello world";
        panel.SetActive(false);

        // ACT: Call the method with the mock player instead of null
        manager.StartTypingSession(testSentence, mockPlayer);

        // ASSERT: Check if the results are what we expect
        Assert.IsTrue(panel.activeSelf, "Panel should be active after session starts.");
        Assert.AreEqual(testSentence, targetText.text, "Target text was not set correctly.");
        Assert.AreEqual("", inputText.text, "Player input text should be cleared.");
    }

    [Test]
    public void HandleCorrectInput_UpdatesStringAndVisuals()
    {
        // ARRANGE
        string testSentence = "Test";
        manager.StartTypingSession(testSentence, Object.FindObjectOfType<Player>());

        // ACT
        // We call our new public method to simulate typing the 'T' character.
        manager.ProcessCharacter('T');

        // ASSERT
        // 1. Check if the internal string was updated correctly.
        Assert.AreEqual("T", manager.playerInputString);

        // 2. Check if the rich text visuals are correct.
        Assert.AreEqual("<color=green>T</color>", inputText.text);
    }
    [Test]
    public void HandleIncorrectInput_ShowsErrorInVisuals()
    {
        // ARRANGE
        string testSentence = "Test";
        manager.StartTypingSession(testSentence, Object.FindObjectOfType<Player>());

        // ACT
        // Simulate typing a wrong character.
        manager.ProcessCharacter('x');

        // ASSERT
        Assert.AreEqual("x", manager.playerInputString);
        Assert.AreEqual("<color=red>x</color>", inputText.text, "Incorrect character should be displayed in red.");
    }

    [Test]
    public void HandleBackspace_RemovesLastCharacter()
    {
        // ARRANGE
        string testSentence = "Test";
        manager.StartTypingSession(testSentence, Object.FindObjectOfType<Player>());
        manager.ProcessCharacter('T');
        manager.ProcessCharacter('e'); // Current input is "Te"

        // ACT
        // Simulate a backspace press.
        manager.ProcessBackspace();

        // ASSERT
        Assert.AreEqual("T", manager.playerInputString, "Player input string should be 'T' after backspace.");
        Assert.AreEqual("<color=green>T</color>", inputText.text, "Visuals should be updated after backspace.");
    }

    [Test]
    public void HandleEnter_WithCorrectFullInput_EndsSessionSuccessfully()
    {
        // ARRANGE
        Player mockPlayer = Object.FindObjectOfType<Player>();
        string testSentence = "win";
        manager.StartTypingSession(testSentence, mockPlayer);

        // Simulate typing the correct sentence
        manager.ProcessCharacter('w');
        manager.ProcessCharacter('i');
        manager.ProcessCharacter('n');

        // ACT
        manager.ProcessEnter();

        // ASSERT
        Assert.IsFalse(panel.activeSelf, "Panel should be inactive after a successful session.");
        Assert.IsTrue(mockPlayer.UnlockMovementWasCalled, "Player should be unlocked after a successful session.");
    }

    [Test]
    public void HandleEnter_WithIncorrectInput_EndsSessionAndUnlocksPlayer()
    {
        // ARRANGE
        Player mockPlayer = Object.FindObjectOfType<Player>();
        string testSentence = "win";
        manager.StartTypingSession(testSentence, mockPlayer);

        // Simulate typing an incorrect sentence
        manager.ProcessCharacter('f');
        manager.ProcessCharacter('a');
        manager.ProcessCharacter('i');
        manager.ProcessCharacter('l');

        // ACT
        manager.ProcessEnter();

        // ASSERT
        Assert.IsFalse(panel.activeSelf, "Panel should be inactive after a failed session.");
        Assert.IsTrue(mockPlayer.UnlockMovementWasCalled, "Player should still be unlocked after a failed session.");
    }
}