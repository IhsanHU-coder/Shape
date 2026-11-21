using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class FinishTrigger : MonoBehaviour
{
    private bool _hasFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasFinished)
            return;

        if (!other.CompareTag("Player"))
            return;

        _hasFinished = true;

        GameContext.SceneEvents.Publish(new GameWinRequested());
    }
}
}