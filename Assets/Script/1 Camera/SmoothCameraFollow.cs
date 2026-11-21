using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{ 
public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -5f);
    [SerializeField] private Vector3 finishOffset = new Vector3(0f, 2.5f, -3.5f);
    [SerializeField] private float positionSmoothSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSmoothSpeed = 4f;

    [Header("References")]
    [SerializeField] private CameraCollisionHandler collisionHandler;
    [SerializeField] private CameraCutsceneController cutsceneController;

    private CubeGridMovement _movement;
    private Quaternion _desiredRotation;
    private bool _isFrozen;
    private bool _isFinished;

    private void Start()
    {
        if (target != null)
            _movement = target.GetComponent<CubeGridMovement>();

        _desiredRotation = Quaternion.Euler(25f, 0f, 0f);
    }

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<CameraFreezeRequested>(OnFreeze);
        GameContext.SceneEvents.Subscribe<CameraUnfreezeRequested>(OnUnfreeze);
        GameContext.SceneEvents.Subscribe<CameraFinishRequested>(OnFinish);
        GameContext.SceneEvents.Subscribe<CameraCutsceneRequested>(OnCutscene);
    }

    private void LateUpdate()
    {
        if (cutsceneController != null && cutsceneController.IsPlaying)
            return;

        if (_isFrozen || target == null)
            return;

        if (_movement != null && !_movement.enabled)
            _isFinished = true;

        if (!_isFinished &&
            _movement != null &&
            _movement.CurrentDirection != Vector3.zero)
        {
            _desiredRotation = Quaternion.LookRotation(
                _movement.CurrentDirection
            );
        }

        Vector3 currentOffset = _isFinished
            ? finishOffset
            : offset;

        Vector3 desiredPosition =
            target.position + _desiredRotation * currentOffset;

        if (collisionHandler != null)
        {
            desiredPosition = collisionHandler.GetSafePosition(
                target.position,
                desiredPosition
            );
        }

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmoothSpeed * Time.deltaTime
        );

        Quaternion lookAtTarget = Quaternion.LookRotation(
            target.position - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookAtTarget,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    private void OnFreeze(CameraFreezeRequested evt)
    {
        _isFrozen = true;
    }

    private void OnUnfreeze(CameraUnfreezeRequested evt)
    {
        _isFrozen = false;
    }

    private void OnFinish(CameraFinishRequested evt)
    {
        _isFinished = true;
    }

    private void OnCutscene(CameraCutsceneRequested evt)
    {
        if (cutsceneController != null)
            cutsceneController.MoveTo(evt.Target);
    }

    private void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<CameraFreezeRequested>(OnFreeze);
        GameContext.SceneEvents.Unsubscribe<CameraUnfreezeRequested>(OnUnfreeze);
        GameContext.SceneEvents.Unsubscribe<CameraFinishRequested>(OnFinish);
        GameContext.SceneEvents.Unsubscribe<CameraCutsceneRequested>(OnCutscene);
    }
}
}