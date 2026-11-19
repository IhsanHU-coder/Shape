using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeGridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] public float cubeSize = 1f;

    [Header("Drift & Wheel Momentum Settings")]
    [Tooltip("Seberapa sensitif scroll menghasilkan energi gelinding")]
    [SerializeField] private float scrollForceMultiplier = 4.5f;
    [Tooltip("Seberapa banyak energi berkurang setiap kali menyelesaikan satu kotak grid")]
    [SerializeField] private float frictionPerBlock = 3f; // Dikurangi sedikit agar luncuran lebih jauh & asyik buat belok
    [Tooltip("Kecepatan maksimum menggelinding")]
    [SerializeField] private float maxAngularVelocity = 45f;
    [Tooltip("Kecepatan minimum sebelum roda kubus memutuskan untuk berhenti")]
    [SerializeField] private float stopThreshold = 3f;

    [Header("Physics Settings")]
    [SerializeField] private LayerMask groundLayer;

    public bool isMoving;
    public float currentVelocity = 0f;
    public Vector3 currentDirection = Vector3.zero;
    public bool landed = true;
    [SerializeField] BtnAnim btnAnim;
    [SerializeField] SmoothCameraFollow smoothCameraFollow;

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject winCanvas;
    public PauseManager pauseManager;
    public GameObject waterSplashParticle;
    [SerializeField] private Transform player;
    public CameraBoost cameraBoost;

    [Header("Wall Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkDistance = 1f;

    [Header("Death Effects")]
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private float rockDeathDelay = 2f;

    private bool isDead = false;


    void Start()
    {
        landed = true;
        awalSpawn();
    }

    public void awalSpawn()
    {
        // this.transform.position = new Vector3(0,10,0);
    }

    private void Update()
    {
        if (Mouse.current == null || Keyboard.current == null) return;



        if (pauseManager.isPaused)
            return;
        if (currentVelocity >= 10)
        {
            cameraBoost.boosting = true;
        }
        else if (currentVelocity >= 35)
        {
            // cameraBoost.boosting2 = true;
        }
        else
        {
            cameraBoost.boosting = false;
            // cameraBoost.boosting2 = false;
        }
        Vector2 scroll = Mouse.current.scroll.ReadValue();

        if (pauseManager.reverseScroll)
            scroll.y = -scroll.y;



        // 1. INPUT ACCELERATION & INSTANT DIRECTION SWAP (Belok Instan)
        if (Mathf.Abs(scroll.y) > 0.1f)
        {
            Vector3 targetDir = Vector3.zero;
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                targetDir = scroll.y > 0 ? Vector3.right : Vector3.left;
            }
            else
            {
                targetDir = scroll.y > 0 ? Vector3.forward : Vector3.back;
            }

            float inputForce = (Mathf.Abs(scroll.y) / 120f) * scrollForceMultiplier;

            // PERBAIKAN UTAMA 1: Jika kubus sedang diam, atau arahnya berubah (BELOK)
            // Kita langsung izinkan ganti arah saat itu juga!
            if (currentDirection == Vector3.zero || currentDirection != targetDir && currentDirection != -targetDir)
            {
                currentDirection = targetDir;

                // Jika belok saat sedang kencang, pertahankan setengah kecepatan lama sebagai momentum drift
                if (currentVelocity > 0f)
                    currentVelocity = Mathf.Max(currentVelocity * 0.6f + inputForce, 6f);
                else
                    currentVelocity = 5f + inputForce;
            }
            // Jika searah lurus, tambahkan kecepatannya (Akselerasi)
            else if (currentDirection == targetDir)
            {
                currentVelocity += inputForce;
            }
            // Jika berlawanan arah (Mengerem)
            else if (currentDirection == -targetDir)
            {
                currentVelocity -= 10f;
                if (currentVelocity <= 0)
                {
                    currentVelocity = 0;
                    currentDirection = Vector3.zero;
                }
            }

            currentVelocity = Mathf.Clamp(currentVelocity, 0f, maxAngularVelocity);
        }

        // 2. MOMENTUM TRIGGER
        // GANTI bagian MOMENTUM TRIGGER di Update() kamu dengan logika ini:

if (currentVelocity > 0.01f && !isMoving && currentDirection != Vector3.zero)
{
    if (currentVelocity < stopThreshold)
    {
        currentVelocity = 0f;
        currentDirection = Vector3.zero;
        SnapToGrid(true);
    }
    else
    {
        // 1. Cek Dinding / Rintangan Mutlak dulu
        if (IsWallAhead(currentDirection))
        {
            currentVelocity = 0f; 
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 nextGridPos = transform.position + currentDirection;
        
        // 2. DETEKSI TANJAKAN: Cek apakah ada tanah di posisi depan-atas (+1 Y)
        Vector3 uphillCheckPos = nextGridPos + Vector3.up;
        bool isUphill = Physics.Raycast(uphillCheckPos, Vector3.down, cubeSize * 0.5f, groundLayer);

        if (isUphill)
        {
            // Jika menanjak, jalankan Coroutine khusus menanjak atau beri tahu RollOneStep
            StartCoroutine(RollOneStep(currentDirection, isAirborne: false, isUphill: true));
        }
        else
        {
            // Cek tanah normal di bawah
            bool groundAhead = Physics.Raycast(nextGridPos, Vector3.down, cubeSize * 1.1f, groundLayer);
            if (groundAhead)
            {
                StartCoroutine(RollOneStep(currentDirection, isAirborne: false, isUphill: false));
            }
            else
            {
                StartCoroutine(RollOneStep(currentDirection, isAirborne: true, isUphill: false));
            }
        }
    }
}
    }
    private IEnumerator RollOneStep(Vector3 direction, bool isAirborne, bool isUphill = false)
{
    isMoving = true;

    // Tentukan posisi pivot dasar
    Vector3 pivot = transform.position + (Vector3.down * cubeSize * 0.5f) + (direction * cubeSize * 0.5f);
    
    // PERBAIKAN: Jika menanjak, naikkan pivot ke atas agar kubus terangkat melompati rintangan/tanjakan
    if (isUphill)
    {
        pivot += Vector3.up * cubeSize;
    }

    Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction).normalized;
    float currentAngle = 0f;

    while (currentAngle < 90f)
    {
        float safeVelocity = Mathf.Max(currentVelocity, 3f);
        float dynamicDuration = 1f / safeVelocity;

        float step = (90f / dynamicDuration) * Time.deltaTime;

        // Cegah "Over-rotate" (pembatas agar tidak melompat terlalu jauh dalam 1 frame saat sangat cepat)
        if (step > 30f) step = 30f; 

        if (currentAngle + step > 90f) step = 90f - currentAngle;

        transform.RotateAround(pivot, rotationAxis, step);
        currentAngle += step;

        yield return null;
    }

    // Jika menanjak, kita PASTI ingin snap posisi Y ke atas
    SnapToGrid(snapY: (!isAirborne || isUphill));

    if (isAirborne)
    {
        frictionPerBlock = 0f;
        landed = false;
    }
    else
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, groundLayer))
        {
            landed = true;
            frictionPerBlock = 2f;
        }
    }

    currentVelocity -= frictionPerBlock;
    isMoving = false;
}

    private void SnapToGrid(bool snapY)
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / cubeSize) * cubeSize;
        if (snapY) pos.y = Mathf.Round(pos.y / cubeSize) * cubeSize;
        pos.z = Mathf.Round(pos.z / cubeSize) * cubeSize;
        transform.position = pos;

        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        Vector3 snappedForward = GetClosestAxis(forward);
        Vector3 snappedUp = GetClosestAxis(up);

        transform.rotation = Quaternion.LookRotation(snappedForward, snappedUp);
    }

    private Vector3 GetClosestAxis(Vector3 v)
    {
        float x = Mathf.Abs(v.x);
        float y = Mathf.Abs(v.y);
        float z = Mathf.Abs(v.z);

        if (x > y && x > z) return new Vector3(Mathf.Sign(v.x), 0, 0);
        if (y > x && y > z) return new Vector3(0, Mathf.Sign(v.y), 0);
        return new Vector3(0, 0, Mathf.Sign(v.z));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            btnAnim.Retry();
            smoothCameraFollow.FreezeCamera();
            GameObject splash = Instantiate(
            waterSplashParticle,
            player.transform.position,
            Quaternion.identity);

            Destroy(splash.gameObject, 3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Wall");
            IsWallAhead(currentDirection);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            isDead = true;

            // Hentikan movement
            StopAllCoroutines();
            btnAnim.Retry();
            currentVelocity = 0f;
            currentDirection = Vector3.zero;
            isMoving = false;
            enabled = false;

            // Freeze camera seperti jatuh ke air
            smoothCameraFollow.FreezeCamera();

            // Spawn darah
            if (bloodParticle != null)
            {
                GameObject blood = Instantiate(
                    bloodParticle,
                    player.position,
                    Quaternion.identity);

                Destroy(blood, 3f);
            }

            // Hide player

            MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;

            // Tunggu outro
            // yield return new WaitForSeconds(rockDeathDelay);

            // Restart
        }
    }

    bool IsWallAhead(Vector3 direction)
{
    Vector3 origin = transform.position + Vector3.up * 0.4f; 
    
    // Jika kecepatan tinggi, cek lebih jauh ke depan (misal: 1.5x lipat dari checkDistance biasa)
    float dynamicCheckDistance = checkDistance;
    if (currentVelocity > 0f) {
        dynamicCheckDistance = cubeSize * 2f;
    }

    return Physics.BoxCast(
        origin,
        new Vector3(0.5f, 0.5f, 0.5f),
        direction,
        Quaternion.identity,
        dynamicCheckDistance,
        wallLayer
    );
}


}