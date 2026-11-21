using System.Collections;
using UnityEngine;

namespace Papayakit.Ihsan
{
public class CubeGridRollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CubeGridMovement movement;

    [Header("Grid Settings")]
    [SerializeField] private float cubeSize = 1f;

    [Header("Physics")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkDistance = 1f;

    private bool _isRolling;

    public void Roll(Vector3 direction)
    {
        if (_isRolling)
            return;

        Vector3 nextGridPos = transform.position + direction;

        if (IsWallAhead(direction))
        {
            movement.StopMovement();
            return;
        }

        Vector3 uphillCheckPos = nextGridPos + Vector3.up;
        bool isUphill = Physics.Raycast(
            uphillCheckPos,
            Vector3.down,
            cubeSize * 0.5f,
            groundLayer
        );

        bool groundAhead = Physics.Raycast(
            nextGridPos,
            Vector3.down,
            cubeSize * 1.1f,
            groundLayer
        );

        StartCoroutine(RollRoutine(
            direction,
            !groundAhead && !isUphill,
            isUphill
        ));
    }

    private IEnumerator RollRoutine(
        Vector3 direction,
        bool isAirborne,
        bool isUphill)
    {
        _isRolling = true;
        movement.SetMoving(true);

        Vector3 pivot = transform.position
            + Vector3.down * cubeSize * 0.5f
            + direction * cubeSize * 0.5f;

        if (isUphill)
            pivot += Vector3.up * cubeSize;

        Vector3 rotationAxis =
            Vector3.Cross(Vector3.up, direction).normalized;

        float currentAngle = 0f;

        while (currentAngle < 90f)
        {
            float safeVelocity = Mathf.Max(
                movement.CurrentVelocity,
                3f
            );

            float duration = 1f / safeVelocity;

            float step = (90f / duration) * Time.deltaTime;

            step = Mathf.Min(step, 30f);
            step = Mathf.Min(step, 90f - currentAngle);

            transform.RotateAround(pivot, rotationAxis, step);

            currentAngle += step;

            yield return null;
        }

        SnapToGrid(!isAirborne || isUphill);

        if (isAirborne)
        {
            movement.SetFriction(0f);
        }
        else if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            1f,
            groundLayer))
        {
            movement.SetFriction(2f);
        }

        movement.ApplyFriction(movement.GetFriction());

        _isRolling = false;
        movement.SetMoving(false);
    }

    public void SnapToGrid(bool snapY)
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Round(pos.x / cubeSize) * cubeSize;
        pos.z = Mathf.Round(pos.z / cubeSize) * cubeSize;

        if (snapY)
            pos.y = Mathf.Round(pos.y / cubeSize) * cubeSize;

        transform.position = pos;

        Vector3 forward = GetClosestAxis(transform.forward);
        Vector3 up = GetClosestAxis(transform.up);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    private Vector3 GetClosestAxis(Vector3 direction)
    {
        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);
        float z = Mathf.Abs(direction.z);

        if (x > y && x > z)
            return new Vector3(Mathf.Sign(direction.x), 0f, 0f);

        if (y > x && y > z)
            return new Vector3(0f, Mathf.Sign(direction.y), 0f);

        return new Vector3(0f, 0f, Mathf.Sign(direction.z));
    }

    public bool IsWallAhead(Vector3 direction)
    {
        Vector3 origin = transform.position + Vector3.up * 0.4f;

        float distance = movement.CurrentVelocity > 0f
            ? cubeSize * 2f
            : checkDistance;

        return Physics.BoxCast(
            origin,
            Vector3.one * 0.5f,
            direction,
            Quaternion.identity,
            distance,
            wallLayer
        );
    }
}
}