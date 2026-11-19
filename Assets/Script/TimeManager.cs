using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI worstTimeText;

    private float elapsedTime;
    private bool isRunning;

    private string levelKey;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        levelKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        ResetTimer();
        StartTimer();

        LoadSavedTimes();
    }

    private void Update()
    {
        if (isRunning)
            elapsedTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public string GetFormattedTime()
    {
        return FormatTime(elapsedTime);
    }

    public void FinishLevel()
    {
        StopTimer();

        float current = elapsedTime;

        if (currentTimeText != null)
            currentTimeText.text = FormatTime(current);

        string bestKey = levelKey + "_Best";
        string worstKey = levelKey + "_Worst";

        // BEST
        if (!PlayerPrefs.HasKey(bestKey) || current < PlayerPrefs.GetFloat(bestKey))
        {
            PlayerPrefs.SetFloat(bestKey, current);
        }

        // WORST
        if (!PlayerPrefs.HasKey(worstKey) || current > PlayerPrefs.GetFloat(worstKey))
        {
            PlayerPrefs.SetFloat(worstKey, current);
        }

        PlayerPrefs.SetInt(levelKey + "_Completed", 1);

        PlayerPrefs.Save();

        LoadSavedTimes();
    }

    void LoadSavedTimes()
    {
        string bestKey = levelKey + "_Best";
        string worstKey = levelKey + "_Worst";

        if (PlayerPrefs.HasKey(bestKey))
        {
            if (bestTimeText != null)
                bestTimeText.text = FormatTime(PlayerPrefs.GetFloat(bestKey));
        }
        else
        {
            if (bestTimeText != null)
                bestTimeText.text = "--:--:--";
        }

        if (PlayerPrefs.HasKey(worstKey))
        {
            if (worstTimeText != null)
                worstTimeText.text = FormatTime(PlayerPrefs.GetFloat(worstKey));
        }
        else
        {
            if (worstTimeText != null)
                worstTimeText.text = "--:--:--";
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int ms = Mathf.FloorToInt((time * 100) % 100);

        return $"{minutes:00}:{seconds:00}:{ms:00}";
    }
}