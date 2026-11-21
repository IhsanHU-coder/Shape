using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class WorldMapButton : MonoBehaviour
{
    [SerializeField] private ButtonAnimationController buttonAnimation;

    public void WorldMap()
    {
        if (buttonAnimation != null)
            buttonAnimation.PlayClickAnimation();

        int currentLevel = SaveManager.LoadCurrentLevel();
        SaveManager.SaveCurrentLevel(currentLevel);

        PlayerPrefs.Save();

        GameFlow.FromMainMenu = false;

        GameContext.SceneEvents.Publish(
            new SceneTransitionRequested("WorldMap")
        );
    }

    public void MainWorldMap()
    {
        if (buttonAnimation != null)
            buttonAnimation.PlayClickAnimation();

        int currentLevel = SaveManager.LoadCurrentLevel();
        SaveManager.SaveCurrentLevel(currentLevel);

        PlayerPrefs.Save();

        GameFlow.FromMainMenu = true;

        GameContext.SceneEvents.Publish(
            new SceneTransitionRequested("WorldMap")
        );
    }
}
}