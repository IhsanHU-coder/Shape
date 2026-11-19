using UnityEngine;

public static class SaveManager
{
    public static void SaveCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentLevel()
{
    return PlayerPrefs.GetInt("CurrentLevel", 0);
}

public static int GetHighestCompletedLevel(int totalLevel)
{
    int highest = 0;

    for (int i = 1; i <= totalLevel; i++)
    {
        if (PlayerPrefs.GetInt("Level " + i + "_Completed", 0) == 1)
            highest = i;
    }

    return highest;
}
}