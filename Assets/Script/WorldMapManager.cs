using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance;

    [SerializeField] private CubeMapMovement player;
    [SerializeField] private TMPro.TextMeshProUGUI levelName;

    private LevelNode selectedNode;

    [Header("Camera")]
    [SerializeField] private Transform targetCanvas;
    [SerializeField] private Transform PositionA;
    [SerializeField] private Transform PositionB;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float scaleSpeed = 5f;

    [SerializeField] private Vector3 TargetPos;
    [SerializeField] private Quaternion TargetRot;

    private Vector3 dragOffset;

    [SerializeField] private Vector3 normalScale = Vector3.one;
    [SerializeField] private Vector3 zoomScale = new Vector3(1.5f, 1.5f, 1.5f);



    [SerializeField] private Vector3 targetScale;

    [SerializeField] Transform playerTrans;
    [SerializeField] Transform posATrans;
    [SerializeField] private Vector3 canvasOffset = new Vector3(-2f, 0f, 0f);
    [SerializeField] float canvasSpeed = 12.5f;

    [SerializeField] private TMPro.TextMeshProUGUI bestTimeText;
    [SerializeField] private TMPro.TextMeshProUGUI rankText;
    [SerializeField] private LevelNode[] levels;
    public BtnAnim btnAnim;
    private bool settingOpened = false;
    public GameObject textEnter;
    public Animator enterAnim;

    private void Awake()
    {
//         PlayerPrefs.DeleteAll();
// PlayerPrefs.Save();
if (Instance != null && Instance != this)
    {
        Debug.LogError("ADA DUA WorldMapManager!");
    }

    Instance = this;

    Debug.Log(name + " menjadi Instance");
        TargetPos = PositionA.position;
        TargetRot = PositionA.rotation;
        targetScale = normalScale;
    }
    private void Start()
    {
        levels = FindObjectsByType<LevelNode>(FindObjectsSortMode.None);

        System.Array.Sort(levels, (a, b) => a.levelID.CompareTo(b.levelID));
        RefreshLevels();

        foreach (LevelNode level in levels)
        {
            string key = level.sceneName + "_Completed";

            level.completed = PlayerPrefs.GetInt(key, 0) == 1;

            if (level.levelID == 1)
                level.unlocked = true;
            else
            {
                LevelNode previous = System.Array.Find(
                    levels,
                    x => x.levelID == level.levelID - 1);

                level.unlocked = previous != null && previous.completed;
            }

            level.RefreshVisual();
        }

        SpawnPlayer();
        
    }

    private void SpawnPlayer()
{
    int currentLevel;

    if (GameFlow.FromMainMenu)
    {
        currentLevel = SaveManager.GetHighestCompletedLevel(levels.Length);

        if (currentLevel == 0)
            currentLevel = 1;
    }
    else
    {
        currentLevel = SaveManager.LoadCurrentLevel();
    }

    // Belum pernah main, biarkan posisi player sesuai scene
    if (currentLevel <= 0)
    {
        Debug.Log("Belum ada save, gunakan posisi awal.");
        return;
    }

    if (currentLevel > levels.Length)
        currentLevel = levels.Length;

    LevelNode node = System.Array.Find(levels, x => x.levelID == currentLevel);

    if (node == null)
        return;

    if (player == null)
{
    Debug.LogError("Player belum ter-assign!");
    return;
}

player.transform.position = node.transform.position;
    player.SetCurrentNode(node);
    ShowEnterLevel(node);

    Debug.Log("Spawn di " + node.sceneName);
}

    public void SelectLevel(LevelNode node)
    {
        if (!node.unlocked)
            return;

        player.MoveTo(node);
    }

    public void ShowEnterLevel(LevelNode node)
    {
        
        selectedNode = node;    

        OpenSetting();

        levelName.text = node.sceneName + "!";

        LoadLevelData(node.sceneName);
    }

    public void EnterSelectedLevel()
    {
        if (selectedNode == null)
        {
            Debug.LogError("Selected Node NULL");
            return;
        }



        btnAnim.PlaySceneTransition(selectedNode.sceneName);
        PlayerPrefs.Save();
    }


    void Update()
{
    Vector3 playerTarget = playerTrans.position + canvasOffset;
    

    // PositionA selalu mengikuti player
    posATrans.position = Vector3.Lerp(
        posATrans.position,
        playerTarget,
        canvasSpeed * Time.deltaTime);

    // Tentukan target kamera
    Vector3 cameraTarget = settingOpened ? TargetPos : playerTarget;

    if (settingOpened)
{
    cameraTarget = PositionB.position;
}
else
{
    cameraTarget = playerTrans.position + canvasOffset;
}

    targetCanvas.position = Vector3.Lerp(
        targetCanvas.position,
        cameraTarget,
        moveSpeed * Time.deltaTime);

    targetCanvas.rotation = Quaternion.Slerp(
        targetCanvas.rotation,
        TargetRot,
        rotationSpeed * Time.deltaTime);

    targetCanvas.localScale = Vector3.Lerp(
        targetCanvas.localScale,
        targetScale,
        scaleSpeed * Time.deltaTime);
}

    public void OpenSetting()
    {
        if(settingOpened)
        return;
        player.pres = true;
        settingOpened = true;
        TargetPos = PositionB.position;
        TargetRot = PositionB.rotation;
        textEnter.SetActive(false);

        targetScale = zoomScale;
        Debug.Log("PosA = " + PositionA.position);
Debug.Log("PosB = " + PositionB.position);
Debug.Log("Player = " + player.transform.position);
    }

    // Hubungkan ke tombol Done
    public void CloseSetting()
    {
        settingOpened = false;
        player.pres = false;
        TargetPos = PositionA.position;
        TargetRot = PositionA.rotation;
        enterAnim.Play(0,0);
        textEnter.SetActive(true);

        targetScale = normalScale;

    }
    public void CloseTrig()
    {
        settingOpened = false;
        player.pres = false;
        TargetPos = PositionA.position;
        TargetRot = PositionA.rotation;
        enterAnim.Play(0,0);
        targetScale = normalScale;

    }
    void LoadLevelData(string sceneName)
    {
        string bestKey = sceneName + "_Best";

        if (!PlayerPrefs.HasKey(bestKey))
        {
            bestTimeText.text = "--:--:--";
            rankText.text = "NONE";
            return;
        }

        float bestTime = PlayerPrefs.GetFloat(bestKey);

        bestTimeText.text = FormatTime(bestTime);

        if (bestTime <= 10f)
            rankText.text = "FAST";
        else if (bestTime <= 30)
            rankText.text = "NORMAL";
        else if (bestTime <= 60)
            rankText.text = "SLOW";
        else
            rankText.text = "WORST";
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int ms = Mathf.FloorToInt((time * 100) % 100);

        return $"{minutes:00}:{seconds:00}:{ms:00}";
    }
    void RefreshLevels()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            bool unlocked = false;

            if (i == 0)
            {
                unlocked = true;
            }
            else
            {
                string previousScene = levels[i - 1].sceneName;

                unlocked =
                    PlayerPrefs.GetInt(previousScene + "_Completed", 0) == 1;
            }

            levels[i].unlocked = unlocked;

            Renderer renderer = levels[i].GetComponent<Renderer>();

            if (renderer != null)
            {
                // renderer.material.color =
                //     unlocked ? Color.green : Color.red;
                levels[i].completed =
                PlayerPrefs.GetInt(levels[i].sceneName + "_Completed", 0) == 1;

                levels[i].RefreshVisual();
            }
        }
    }
}