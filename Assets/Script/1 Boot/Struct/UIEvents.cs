public struct RetryButtonRequested { }

public struct SceneTransitionRequested
{
    public string SceneName;

    public SceneTransitionRequested(string sceneName)
    {
        SceneName = sceneName;
    }
}

public struct PauseButtonRequested { }

public struct PauseCloseRequested { }

public struct AboutOpenRequested { }

public struct AboutCloseRequested { }