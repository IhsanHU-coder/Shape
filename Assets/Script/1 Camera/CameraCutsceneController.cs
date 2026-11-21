using UnityEngine;

namespace Papayakit.Ihsan
{
public class CameraCutsceneController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotateSpeed = 180f;

    private Transform _target;

    public bool IsPlaying { get; private set; }

    public void MoveTo(Transform target)
    {
        if (target == null)
            return;

        _target = target;
        IsPlaying = true;
    }

    private void Update()
    {
        if (!IsPlaying || _target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            moveSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _target.rotation,
            rotateSpeed * Time.deltaTime
        );
    }

    public void StopCutscene()
    {
        IsPlaying = false;
        _target = null;
    }
}
}