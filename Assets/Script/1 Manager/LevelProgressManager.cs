using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class LevelProgressManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        GameContext.SceneEvents.Publish(new GameWinRequested());
    }
}
}