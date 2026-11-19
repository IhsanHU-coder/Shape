using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -5f);
    [SerializeField] private float positionSmoothSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothSpeed = 4f;

    [Header("Finish Cinematic")]
    [Tooltip("Jarak kamera saat mendekat ke kubus pas finish (Zoom In)")]
    [SerializeField] private Vector3 finishOffset = new Vector3(0f, 2.5f, -3.5f);

    private CubeGridMovement movementScript;
    private Quaternion desiredCameraRotation;
    private bool isPlayerFinished = false;

    private bool freezeCamera = false;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.15f;

    private bool useCutscene = false;
private Transform cutsceneTarget;
[Header("Cutscene")]
[SerializeField] private float cutsceneMoveSpeed = 6f;
[SerializeField] private float cutsceneRotateSpeed = 180f;

public void MoveToCutscene(Transform targetPos)
{
    if (targetPos == null) return;

    useCutscene = true;
    cutsceneTarget = targetPos;
}

    private void Start()
    {
        if (target != null)
        {
            movementScript = target.GetComponent<CubeGridMovement>();
        }


        desiredCameraRotation = Quaternion.Euler(25f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (useCutscene)
{
    transform.position = Vector3.MoveTowards(
        transform.position,
        cutsceneTarget.position,
        cutsceneMoveSpeed * Time.deltaTime);

    transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        cutsceneTarget.rotation,
        cutsceneRotateSpeed * Time.deltaTime);

    return;
}
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // if (player != null)
            // {
            //     target = player.transform;
            //     movementScript = target.GetComponent<CubeGridMovement>();
            // }
            // else
            // {
            //     return;
            // }
        }

        if (freezeCamera)
            return;
        if (target == null) return;

        // Cek apakah script movement player dinonaktifkan (tanda sudah finish)
        if (movementScript != null && !movementScript.enabled)
        {
            isPlayerFinished = true;
        }

        // Jika BELUM finish, baca input scroll seperti biasa
        if (!isPlayerFinished && Mouse.current != null)
        {
            Vector2 scrollInput = Mouse.current.scroll.ReadValue();

            if (Mathf.Abs(scrollInput.y) > 0.1f && movementScript != null && movementScript.currentDirection != Vector3.zero)
            {
                Vector3 moveDir = movementScript.currentDirection;
                desiredCameraRotation = Quaternion.LookRotation(moveDir);
            }
        }

        // HITUNG POSISI KAMERA
        // Jika sudah finish, gunakan finishOffset (kamera otomatis maju mendekat / zoom-in)
        // Vector3 currentOffset = isPlayerFinished ? finishOffset : offset;
        // Vector3 targetPosition = target.position + (desiredCameraRotation * currentOffset);
        Vector3 currentOffset = isPlayerFinished ? finishOffset : offset;

        Vector3 desiredPosition =
            target.position + (desiredCameraRotation * currentOffset);

        // arah dari player ke kamera
        Vector3 direction = (desiredPosition - target.position).normalized;

        float distance = Vector3.Distance(target.position, desiredPosition);

        // cek apakah ada tembok
        if (Physics.SphereCast(
            target.position,
            cameraRadius,
            direction,
            out RaycastHit hit,
            distance,
            obstacleLayer))
        {
            desiredPosition =
                hit.point - direction * collisionOffset;
        }

        Vector3 targetPosition = desiredPosition;

        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmoothSpeed * Time.deltaTime);

        // ROTASI KAMERA
        Quaternion lookAtTarget = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, rotationSmoothSpeed * Time.deltaTime);
    }

    public void FreezeCamera()
    {
        freezeCamera = true;
    }
    public void UnFreezeCamera()
    {
        freezeCamera = false;
    }


}