using UnityEngine;

public class LevelNode : PathNode
{
    [Header("Level")]
    public int levelID;

    public string sceneName;

    [Header("State")]

    public bool completed = false;

    public Renderer cubeRenderer;

public Color lockedColor = Color.gray;
public Color unlockedColor = Color.red;
public Color completedColor = Color.green;

public void RefreshVisual()
{
    if (completed)
    {
        cubeRenderer.material.color = completedColor;
    }
    else if (unlocked)
    {
        cubeRenderer.material.color = unlockedColor;
    }
    else
    {
        cubeRenderer.material.color = lockedColor;
    }
}
}