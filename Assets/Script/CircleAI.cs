using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class CircleAI : MonoBehaviour
{
    [Header("Behavior Settings")]
    [Tooltip("Jika DI-CENTANG (True), Circle akan diam mematung di tempat dan tidak akan patroli/kabur")]
    [SerializeField] private bool isStatue = false;

    [Header("Patrol Settings")]
    [Tooltip("Radius seberapa jauh Circle mencari titik patroli baru secara acak")]
    [SerializeField] private float patrolRadius = 15f;
    [Tooltip("Waktu tunggu (detik) di setiap titik sebelum menggelinding ke titik berikutnya")]
    [SerializeField] private float waitTime = 2f;

    [Header("Flee Settings (Kabur)")]
    [Tooltip("Jarak deteksi. Jika Square lebih dekat dari jarak ini, Circle akan kabur")]
    [SerializeField] private float fleeDistance = 5f;
    [Tooltip("Kecepatan Circle saat kabur karena kesal")]
    [SerializeField] private float fleeSpeed = 8f;
    [Tooltip("Kecepatan normal saat patroli santai")]
    [SerializeField] private float patrolSpeed = 3.5f;

    [Header("Requirement Settings")]
    [Tooltip("Daftar barang bawaan wajib yang harus dipenuhi Square")]
    [SerializeField] private List<GiftData> requiredGifts = new List<GiftData>();

    [Header("Finish Settings")]
    [SerializeField] private float suctionSpeed = 8f;
    [SerializeField] private ParticleSystem celebrationParticles;
    

    private NavMeshAgent agent;
    private Transform playerTransform;
    private PlayerInventory playerInventory;
    private CubeGridMovement playerMovement;

    private float timer;
    private bool isPlayerQualified = false;
    public bool hasFinished = false;
    [SerializeField] float waitOutro = 3f;
    public GameObject winCanvas;
    [SerializeField] private int levelID;

    [Header("Win Cutscene")]
    [SerializeField] private bool cutScene = false;
    [SerializeField] private Transform cutsceneCameraPosition;
    [SerializeField] private float waitBeforeCamera = 2f;
    [SerializeField] private float waitBeforeWinCanvas = 2f;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        winCanvas.SetActive(false);

        // Mencari objek Square (Player) di dalam game
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerInventory = player.GetComponent<PlayerInventory>();
            playerMovement = player.GetComponent<CubeGridMovement>();
        }

        agent.speed = patrolSpeed;
        timer = waitTime;
        if (!isStatue)
        {
            MoveToRandomPatrolPoint();
        }
    }

    private void Update()
    {
        if (hasFinished || playerTransform == null) return;

        // 1. Selalu cek apakah Square sudah memenuhi semua syarat kado saat ini
        CheckPlayerQualification();

        if (isStatue)
        {
            if (agent.hasPath) agent.ResetPath();
            return;
        }



        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 2. LOGIKA KABUR (Jika Square belum qualified dan nekat mendekat)
        if (!isPlayerQualified && distanceToPlayer < fleeDistance)
        {
            FleeFromPlayer();
        }
        // 3. LOGIKA PATROLI SANTAI (Jika Square jauh atau sudah bawa barang lengkap)
        else
        {
            agent.speed = patrolSpeed;
            PatrolRoutine();
        }
    }

    private void PatrolRoutine()
    {
        // Jika sudah sampai di titik patroli, hitung mundur untuk mencari titik baru
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                MoveToRandomPatrolPoint();
                timer = 0f;
            }
        }
    }

    private void MoveToRandomPatrolPoint()
    {
        // Cari titik acak di dalam radius patroli
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        // Pastikan titik acak tersebut berada di atas area lantai yang sudah di-bake
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    private void FleeFromPlayer()
    {
        agent.speed = fleeSpeed;

        // 1. Hitung arah normal menjauh (ke belakang)
        Vector3 directionToPlayer = transform.position - playerTransform.position;
        Vector3 runToPos = transform.position + directionToPlayer.normalized * fleeDistance;

        NavMeshHit navHit;
        // Cek apakah jalur belakang aman (masih di dalam NavMesh)
        if (NavMesh.SamplePosition(runToPos, out navHit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
        else
        {
            // 2. JIKA JALUR BELAKANG BUNTU (Seperti di pojokan foto):
            // Kita paksa Circle kabur melipir ke arah samping (Kanan atau Kiri dari arah hadap Square)
            Vector3 sideDirection = Vector3.Cross(directionToPlayer, Vector3.up).normalized;

            // Coba kabur ke arah kanan samping dulu
            Vector3 fleeSidePos = transform.position + sideDirection * fleeDistance;

            if (!NavMesh.SamplePosition(fleeSidePos, out navHit, 2f, NavMesh.AllAreas))
            {
                // Jika kanan buntu juga, ganti kabur melipir ke arah kiri samping
                fleeSidePos = transform.position - sideDirection * fleeDistance;
                NavMesh.SamplePosition(fleeSidePos, out navHit, fleeDistance, NavMesh.AllAreas);
            }

            agent.SetDestination(navHit.position);
        }

        Debug.Log("❌ Circle: 'Ups, hampir kejebak! Permisi, aku mau lewat samping!' *Nge-drift kabur*");
    }

    private void CheckPlayerQualification()
    {
        if (playerInventory == null) return;

        bool requirementsMet = true;
        foreach (GiftData gift in requiredGifts)
        {
            if (playerInventory.GetOwnedAmount(gift) < gift.requiredAmount)
            {
                requirementsMet = false;
                break;
            }
        }
        isPlayerQualified = requirementsMet;
    }

    // LOGIKA FINISH (Diterima karena barang lengkap)
    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished || !isPlayerQualified) return;

        CubeGridMovement player = other.GetComponent<CubeGridMovement>();
        if (player != null)
        {
            hasFinished = true;

            // Matikan sistem NavMesh agar Circle langsung berhenti diam di tempat
            agent.enabled = false;

            // Matikan kontrol gerak player agar tidak bisa digerakkan mouse lagi
            player.StopAllCoroutines();
            player.enabled = false;

            // Ambil titik terdekat di permukaan collider Circle tempat Player menyentuhnya
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            StartCoroutine(AnimateFinish(player.transform, contactPoint));
        }
    }

    private IEnumerator AnimateFinish(Transform playerTransform, Vector3 contactPoint)
    {
        float elapsed = 0f;
        Vector3 startPlayerPos = playerTransform.position;
        Quaternion startPlayerRot = playerTransform.rotation;

        // PERBAIKAN: Target posisi bukan di tengah Circle lagi, melainkan di titik kontak terdekat!
        // Sumbu Y tetap dijaga agar Player tidak amblas ke bawah lantai
        Vector3 targetPos = new Vector3(contactPoint.x, playerTransform.position.y, contactPoint.z);

        // Buat Player otomatis menghadap ke arah tengah Circle saat berhenti (Biar estetik)
        Vector3 lookDirection = (transform.position - targetPos).normalized;
        lookDirection.y = 0; // Kunci sumbu Y agar tidak mendongak/menunduk
        Quaternion targetRot = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : playerTransform.rotation;

        if (celebrationParticles != null) celebrationParticles.Play();

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * suctionSpeed;
            playerTransform.position = Vector3.Lerp(startPlayerPos, targetPos, elapsed);
            playerTransform.rotation = Quaternion.Slerp(startPlayerRot, targetRot, elapsed);
            yield return null;
        }

        playerTransform.position = targetPos;
        playerTransform.rotation = targetRot;

        Debug.Log("💖 SHAPE LOVE: Square mendarat pas di depan Circle tanpa menembus badannya!");
        // ==================== TAMBAHAN SISTEM TIMELAPSE ====================
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StopTimer();       // Hentikan hitungan waktu
            TimeManager.Instance.FinishLevel(); // Tulis waktunya ke text UI di Win Canvas
            string levelName =
    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            PlayerPrefs.SetInt(levelName + "_Completed", 1);
            PlayerPrefs.Save();
        }
        SaveManager.SaveCurrentLevel(levelID);

        // ===================================================================

        // yield return new WaitForSeconds(waitOutro);

        // winCanvas.SetActive(true);

        if (cutScene)
        {
            // Tunggu setelah player selesai diposisikan
            yield return new WaitForSeconds(waitBeforeCamera);

            SmoothCameraFollow cam = Camera.main.GetComponent<SmoothCameraFollow>();

            if (cam != null)
                cam.MoveToCutscene(cutsceneCameraPosition);

            // Tunggu lagi sebelum muncul Win Canvas
            yield return new WaitForSeconds(waitBeforeWinCanvas);
        }
        else
        {
            yield return new WaitForSeconds(waitOutro);
        }

        winCanvas.SetActive(true);
        PauseManager pause = FindFirstObjectByType<PauseManager>();

        if (pause != null)
        {
            pause.ClosePause();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (pause != null)
        {
            pause.ClosePause();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Time.timeScale = 0f;
    }

    private void OpenPause()
    {
        PauseManager pause = FindFirstObjectByType<PauseManager>();
        pause.isPaused = true;

        pause.pauseCanvas.SetActive(true);
        pause.pauseAnimator.Play("PauseOpen");

        // Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ClosePause()
    {
        PauseManager pause = FindFirstObjectByType<PauseManager>();

        pause.isPaused = false;

        pause.pauseAnimator.Play("PauseClose");

        // Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}