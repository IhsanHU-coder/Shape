using System;
using PapayaKit.Core;
using UnityEngine;


namespace Papayakit.Ihsan
{
public class CubeGridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cubeSize = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float scrollForceMultiplier = 4.5f;
    [SerializeField] private float maxAngularVelocity = 45f;
    [SerializeField] private float stopThreshold = 3f;

    [Header("References")]
    [SerializeField] private CubeGridRollController rollController;
    [SerializeField] private CameraBoost cameraBoost;

    public bool IsMoving { get; private set; }
    public float CurrentVelocity { get; private set; }
    public Vector3 CurrentDirection { get; private set; }

    private float _frictionPerBlock = 3f;
    private bool _isPaused;
    private bool _isDead;

    public void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<PlayerScrollInput>(OnScrollInput);
        GameContext.SceneEvents.Subscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Subscribe<PlayerDeathRequested>(OnDeath);
        GameContext.SceneEvents.Subscribe<PlayerRespawnRequested>(OnRespawn);
    }

    private void Update()
    {
        if (_isPaused || _isDead)
            return;

        UpdateCameraBoost();

        if (CurrentVelocity > 0.01f &&
            !IsMoving &&
            CurrentDirection != Vector3.zero)
        {
            if (CurrentVelocity < stopThreshold)
            {
                StopMovement();
                rollController.SnapToGrid(true);
                return;
            }

            if (rollController.IsWallAhead(CurrentDirection))
            {
                StopMovement();
                return;
            }

            rollController.Roll(CurrentDirection);
        }
    }

    private void OnScrollInput(PlayerScrollInput evt)
    {
        if (_isPaused || _isDead || IsMoving)
            return;

        Vector3 targetDirection;

        if (evt.IsLeftCtrlPressed)
        {
            targetDirection = evt.ScrollValue > 0
                ? Vector3.right
                : Vector3.left;
        }
        else
        {
            targetDirection = evt.ScrollValue > 0
                ? Vector3.forward
                : Vector3.back;
        }

        float inputForce =
            Mathf.Abs(evt.ScrollValue) / 120f * scrollForceMultiplier;

        if (CurrentDirection == Vector3.zero ||
            (CurrentDirection != targetDirection &&
             CurrentDirection != -targetDirection))
        {
            CurrentDirection = targetDirection;

            if (CurrentVelocity > 0f)
                CurrentVelocity = Mathf.Max(CurrentVelocity * 0.6f + inputForce, 6f);
            else
                CurrentVelocity = 5f + inputForce;
        }
        else if (CurrentDirection == targetDirection)
        {
            CurrentVelocity += inputForce;
        }
        else
        {
            CurrentVelocity -= 10f;

            if (CurrentVelocity <= 0f)
            {
                CurrentVelocity = 0f;
                CurrentDirection = Vector3.zero;
            }
        }

        CurrentVelocity = Mathf.Clamp(
            CurrentVelocity,
            0f,
            maxAngularVelocity
        );

        PublishMovement();
    }

    private void UpdateCameraBoost()
    {
        bool shouldBoost = CurrentVelocity >= 10f;

        if (cameraBoost != null)
            GameContext.SceneEvents.Publish(
                new CameraBoostChanged(shouldBoost)
            );
    }

    public void SetMoving(bool value)
    {
        IsMoving = value;
        PublishMovement();
    }

    public void ApplyFriction(float friction)
    {
        CurrentVelocity = Mathf.Max(0f, CurrentVelocity - friction);

        if (CurrentVelocity <= 0f)
            CurrentDirection = Vector3.zero;

        PublishMovement();
    }

    public void SetFriction(float value)
    {
        _frictionPerBlock = value;
    }

    public float GetFriction()
    {
        return _frictionPerBlock;
    }

    public void StopMovement()
    {
        CurrentVelocity = 0f;
        CurrentDirection = Vector3.zero;
        IsMoving = false;

        PublishMovement();
    }

    private void OnPauseChanged(GamePausedChanged evt)
    {
        _isPaused = evt.IsPaused;
    }

    private void OnDeath(PlayerDeathRequested evt)
    {
        _isDead = true;
        StopMovement();
    }

    private void OnRespawn(PlayerRespawnRequested evt)
    {
        _isDead = false;
        StopMovement();
    }

    private void PublishMovement()
    {
        GameContext.SceneEvents.Publish(
            new PlayerMovementChanged(
                CurrentDirection,
                CurrentVelocity,
                IsMoving
            )
        );
    }

    public void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<PlayerScrollInput>(OnScrollInput);
        GameContext.SceneEvents.Unsubscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Unsubscribe<PlayerDeathRequested>(OnDeath);
        GameContext.SceneEvents.Unsubscribe<PlayerRespawnRequested>(OnRespawn);
    }
}
}