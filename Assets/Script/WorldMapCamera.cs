using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private CubeMapMovement player;

    [Header("Follow")]
    [SerializeField] private Vector3 followOffset = new Vector3(0, 12, -10);
    [SerializeField] private float followSpeed = 5f;

    [Header("Drag")]
    [SerializeField] private float dragSpeed = 0.25f;


    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0f;
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 18f;

    private Vector3 dragOffset;

    [SerializeField] private float centerSpeed = 5f;


[Header("Map Bounds")]
[SerializeField] private bool useBounds = true;
[SerializeField] private BoxCollider cameraBounds;

    void Start()
    {
        // float scroll = Mouse.current.scroll.ReadValue().y;

        // if (Mathf.Abs(scroll) < 0.01f)
        //     return;

        followOffset += followOffset.normalized * (zoomSpeed * Time.deltaTime * 30f);

        float distance = followOffset.magnitude;

        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        followOffset = followOffset.normalized * distance;
    }

    private Vector3 ClampCamera(Vector3 position)
{
    if (!useBounds || cameraBounds == null)
        return position;

    Bounds bounds = cameraBounds.bounds;

    position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
    position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);

    return position;
}

    void LateUpdate()
    {
        if (Mouse.current == null)
            return;

        HandleZoom();

        // Saat cube jalan, kamera follow
        if (player.IsMoving)
        {
            // Perlahan hilangkan drag supaya player kembali ke tengah
            dragOffset = Vector3.Lerp(
                dragOffset,
                Vector3.zero,
                centerSpeed * Time.deltaTime);

            Vector3 desired =
                target.position +
                followOffset +
                dragOffset;

            desired = ClampCamera(desired);

            transform.position = Vector3.Lerp(
                transform.position,
                desired,
                followSpeed * Time.deltaTime);

            return;
        }

        // Saat cube diam, kamera bisa digeser
        HandleDrag();

        Vector3 pos = target.position + followOffset + dragOffset;
        pos = ClampCamera(pos);

        transform.position = Vector3.Lerp(
            transform.position,
            pos,
            followSpeed * Time.deltaTime);
    }

    void HandleDrag()
    {
        if (!Mouse.current.middleButton.isPressed)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        right.y = 0;
        forward.y = 0;

        right.Normalize();
        forward.Normalize();

        dragOffset -= right * delta.x * dragSpeed;
        dragOffset -= forward * delta.y * dragSpeed;
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        followOffset += followOffset.normalized * (-scroll * zoomSpeed * Time.deltaTime * 30f);

        float distance = followOffset.magnitude;

        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        followOffset = followOffset.normalized * distance;
    }
}