using System;
using PapayaKit.Core;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Papayakit.Ihsan
{
public class TimeManager : MonoBehaviour, IDisposable
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI worstTimeText;

    private float _elapsedTime;
    private bool _isRunning;

    private string _levelKey;

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<GameTimerStarted>(OnTimerStarted);
        GameContext.SceneEvents.Subscribe<GameTimerStopped>(OnTimerStopped);
        GameContext.SceneEvents.Subscribe<GameFinished>(OnGameFinished);
    }

    private void Start()
    {
        _levelKey = SceneManager.GetActiveScene().name;

        ResetTimer();
        StartTimer();

        LoadSavedTimes();
    }

    private void Update()
    {
        if (!_isRunning)
            return;

        _elapsedTime += Time.deltaTime;

        if (currentTimeText != null)
            currentTimeText.text = FormatTime(_elapsedTime);
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ResetTimer()
    {
        _elapsedTime = 0f;
    }

    public float GetElapsedTime()
    {
        return _elapsedTime;
    }

    public string GetFormattedTime()
    {
        return FormatTime(_elapsedTime);
    }

    public void FinishLevel()
    {
        StopTimer();

        float current = _elapsedTime;

        string bestKey = _levelKey + "_Best";
        string worstKey = _levelKey + "_Worst";

        if (!PlayerPrefs.HasKey(bestKey) ||
            current < PlayerPrefs.GetFloat(bestKey))
        {
            PlayerPrefs.SetFloat(bestKey, current);
        }

        if (!PlayerPrefs.HasKey(worstKey) ||
            current > PlayerPrefs.GetFloat(worstKey))
        {
            PlayerPrefs.SetFloat(worstKey, current);
        }

        PlayerPrefs.SetInt(_levelKey + "_Completed", 1);

        PlayerPrefs.Save();

        LoadSavedTimes();

        if (currentTimeText != null)
            currentTimeText.text = FormatTime(current);
    }

    private void LoadSavedTimes()
    {
        string bestKey = _levelKey + "_Best";
        string worstKey = _levelKey + "_Worst";

        if (bestTimeText != null)
        {
            bestTimeText.text = PlayerPrefs.HasKey(bestKey)
                ? FormatTime(PlayerPrefs.GetFloat(bestKey))
                : "--:--:--";
        }

        if (worstTimeText != null)
        {
            worstTimeText.text = PlayerPrefs.HasKey(worstKey)
                ? FormatTime(PlayerPrefs.GetFloat(worstKey))
                : "--:--:--";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private void OnTimerStarted(GameTimerStarted evt)
    {
        StartTimer();
    }

    private void OnTimerStopped(GameTimerStopped evt)
    {
        StopTimer();
    }

    private void OnGameFinished(GameFinished evt)
    {
        FinishLevel();
    }

    public void Dispose()
    {
        GameContext.SceneEvents.Unsubscribe<GameTimerStarted>(OnTimerStarted);
        GameContext.SceneEvents.Unsubscribe<GameTimerStopped>(OnTimerStopped);
        GameContext.SceneEvents.Unsubscribe<GameFinished>(OnGameFinished);
    }

    private void OnDisable()
    {
        Dispose();
    }
}
}