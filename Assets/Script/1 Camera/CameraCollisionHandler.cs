using UnityEngine;

namespace Papayakit.Ihsan
{
public class CameraCollisionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.15f;

    public Vector3 GetSafePosition(
        Vector3 targetPosition,
        Vector3 desiredPosition)
    {
        Vector3 direction =
            (desiredPosition - targetPosition).normalized;

        float distance = Vector3.Distance(
            targetPosition,
            desiredPosition
        );

        if (Physics.SphereCast(
            targetPosition,
            cameraRadius,
            direction,
            out RaycastHit hit,
            distance,
            obstacleLayer))
        {
            return hit.point - direction * collisionOffset;
        }

        return desiredPosition;
    }
}
}