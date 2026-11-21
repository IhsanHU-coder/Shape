using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class SceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private ButtonAnimationController buttonAnimation;

    public void OnClick()
    {
        if (buttonAnimation != null)
            buttonAnimation.PlayClickAnimation();

        GameContext.SceneEvents.Publish(
            new SceneTransitionRequested(sceneName)
        );
    }
}
}