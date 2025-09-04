using NUnit.Framework;
using UnityEngine;
using TMPro;

public class TypingManagerTests
{
    private TypingManager manager;
    private GameObject managerObject;
    private GameObject panel;
    private TextMeshProUGUI targetText;
    private TextMeshProUGUI inputText;

    [SetUp]
    public void Setup()
    {
        managerObject = new GameObject();
        manager = managerObject.AddComponent<TypingManager>();

        panel = new GameObject("TestTypingPanel");

        GameObject targetTextObject = new GameObject("TargetText");
        targetTextObject.transform.SetParent(panel.transform);
        targetText = targetTextObject.AddComponent<TextMeshProUGUI>();

        GameObject inputTextObject = new GameObject("InputText");
        inputTextObject.transform.SetParent(panel.transform);
        inputText = inputTextObject.AddComponent<TextMeshProUGUI>();

        manager.Initialize(panel, targetText, inputText);
    }

    // This method with [TearDown] runs after each test to clean up.
    // It's vital for static events to prevent tests from interfering with each other.
    [TearDown]
    public void Teardown()
    {
        // Unsubscribe from all static events to ensure test isolation.
        GameEvents.OnTypingSessionStart = null;
        GameEvents.OnTypingSessionEnd = null;

        // Destroy the GameObjects created during the test.
        Object.DestroyImmediate(managerObject);
        Object.DestroyImmediate(panel);
    }

    [Test]
    public void StartTypingSession_InitializesStateCorrectly()
    {
        // ARRANGE
        string testSentence = "hello world";
        panel.SetActive(false);

        // ACT
        // CHANGED: The Player reference is no longer needed.
        manager.StartTypingSession(testSentence);

        // ASSERT
        Assert.IsTrue(panel.activeSelf, "Panel should be active after session starts.");
        Assert.AreEqual(testSentence, targetText.text, "Target text was not set correctly.");
        Assert.AreEqual("", inputText.text, "Player input text should be cleared.");
    }

    [Test]
    public void HandleCorrectInput_UpdatesStringAndVisuals()
    {
        // ARRANGE
        manager.StartTypingSession("Test");

        // ACT
        manager.ProcessCharacter('T');

        // ASSERT
        Assert.AreEqual("T", manager.playerInputString);
        Assert.AreEqual("<color=green>T</color>", inputText.text);
    }

    [Test]
    public void HandleIncorrectInput_ShowsErrorInVisuals()
    {
        // ARRANGE
        manager.StartTypingSession("Test");

        // ACT
        manager.ProcessCharacter('x');

        // ASSERT
        Assert.AreEqual("x", manager.playerInputString);
        Assert.AreEqual("<color=red>x</color>", inputText.text);
    }

    [Test]
    public void HandleBackspace_RemovesLastCharacter()
    {
        // ARRANGE
        manager.StartTypingSession("Test");
        manager.ProcessCharacter('T');
        manager.ProcessCharacter('e');

        // ACT
        manager.ProcessBackspace();

        // ASSERT
        Assert.AreEqual("T", manager.playerInputString);
        Assert.AreEqual("<color=green>T</color>", inputText.text);
    }

    [Test]
    public void HandleEnter_WithCorrectFullInput_FiresEndSessionEvent()
    {
        // ARRANGE
        bool sessionEndedEventFired = false;
        // Subscribe to the event to listen for the broadcast.
        GameEvents.OnTypingSessionEnd += () => { sessionEndedEventFired = true; };

        manager.StartTypingSession("win");
        manager.ProcessCharacter('w');
        manager.ProcessCharacter('i');
        manager.ProcessCharacter('n');

        // ACT
        manager.ProcessEnter();

        // ASSERT
        Assert.IsFalse(panel.activeSelf, "Panel should be inactive after a successful session.");
        Assert.IsTrue(sessionEndedEventFired, "OnTypingSessionEnd event was not fired on success.");
    }

    [Test]
    public void HandleEnter_WithIncorrectInput_FiresEndSessionEvent()
    {
        // ARRANGE
        bool sessionEndedEventFired = false;
        // Subscribe to the event to listen for the broadcast.
        GameEvents.OnTypingSessionEnd += () => { sessionEndedEventFired = true; };

        manager.StartTypingSession("win");
        manager.ProcessCharacter('f');
        manager.ProcessCharacter('a');
        manager.ProcessCharacter('i');
        manager.ProcessCharacter('l');

        // ACT
        manager.ProcessEnter();

        // ASSERT
        Assert.IsFalse(panel.activeSelf, "Panel should be inactive after a failed session.");
        Assert.IsTrue(sessionEndedEventFired, "OnTypingSessionEnd event was not fired on failure.");
    }
}