using UnityEngine;

namespace Papayakit.Ihsan
{
public class AboutButton : MonoBehaviour
{
    [SerializeField] private ButtonAnimationController buttonAnimation;

    public void Open()
    {
        buttonAnimation.AboutOpen();
    }

    public void Close()
    {
        buttonAnimation.AboutClose();
    }
}
}