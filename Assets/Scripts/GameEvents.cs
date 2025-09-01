using System;

public static class GameEvents
{
    // This event will be broadcast when the typing minigame starts.
    public static Action OnTypingSessionStart;

    // This event will be broadcast when the typing minigame ends (success or fail).
    public static Action OnTypingSessionEnd;
}