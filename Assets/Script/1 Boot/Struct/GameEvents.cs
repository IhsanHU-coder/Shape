public struct GamePausedChanged
{
    public bool IsPaused;

    public GamePausedChanged(bool isPaused)
    {
        IsPaused = isPaused;
    }
}

public struct GameWinRequested { }

public struct GameFinished { }

public struct GameRetryRequested { }

public struct GameLevelLoaded
{
    public string SceneName;

    public GameLevelLoaded(string sceneName)
    {
        SceneName = sceneName;
    }
}

public struct GameTimerStopped { }

public struct GameTimerStarted { }