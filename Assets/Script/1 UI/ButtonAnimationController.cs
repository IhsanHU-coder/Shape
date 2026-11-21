using UnityEngine;
using UnityEngine.EventSystems;

namespace Papayakit.Ihsan
{
public class ButtonAnimationController : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Animator animator;

    private bool _isClicked;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isClicked || animator == null)
            return;

        animator.Play("FlipBtn");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hover exit.
    }

    public void PlayClickAnimation()
    {
        _isClicked = true;

        if (animator != null)
            animator.Play("ShakeBtn");
    }

    public void ResetButton()
    {
        _isClicked = false;
    }

    public void AboutOpen()
    {
        if (animator != null)
            animator.Play("AboutAnim");
    }

    public void AboutClose()
    {
        if (animator != null)
            animator.Play("Close");
    }
}
}