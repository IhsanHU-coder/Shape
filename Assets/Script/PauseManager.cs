using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    private const string MusicKey = "MusicVolume";
    [Header("Day Time")]
    [SerializeField] private Slider dayTimeSlider;
    [SerializeField] private Light sun;
    private const string TimeKey = "DayTime";
    [SerializeField] private Material skyboxMaterial;

    public bool isPaused = false;
    public GameObject pauseCanvas;
    public Animator pauseAnimator;
    [SerializeField] private CircleAI circleAI;

    public bool reverseScroll = false;
    [SerializeField] private Toggle reverseScrollToggle;
    public CubeGridMovement movement;
    
    

    void Awake()
    {

            reverseScroll = PlayerPrefs.GetInt("ReverseScroll", 0) == 1;
    }
    public void SetReverseScroll(bool value)
{
    reverseScroll = value;

    PlayerPrefs.SetInt("ReverseScroll", value ? 1 : 0);
    PlayerPrefs.Save();
}
    private void OnReverseChanged(bool value)
    {
        SetReverseScroll(value);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        float volume = PlayerPrefs.GetFloat(MusicKey, 1f);
        float time = PlayerPrefs.GetFloat(TimeKey, 12f);

        musicSlider.value = volume;
        AudioListener.volume = volume;
        dayTimeSlider.value = time;
        UpdateTime(time);

        musicSlider.onValueChanged.AddListener(SetVolume);
        dayTimeSlider.onValueChanged.AddListener(UpdateTime);
        reverseScrollToggle.isOn = reverseScroll;
reverseScrollToggle.onValueChanged.AddListener(OnReverseChanged);
        if (circleAI == null)
        circleAI = FindFirstObjectByType<CircleAI>();
    }

    void Update()
    {
        if (circleAI != null && circleAI.hasFinished)
    {
        if (isPaused)
        {
            isPaused = false;
            pauseCanvas.SetActive(false);
        }

        return;
    }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();
    }

    private void UpdateTime(float hour)
    {
        PlayerPrefs.SetFloat(TimeKey, hour);
        PlayerPrefs.Save();

        float t = hour / 24f;

        // Matahari berputar
        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(
                Mathf.Lerp(-90f, 270f, t),
                -30f,
                0f);

            sun.intensity = Mathf.Lerp(0.1f, 1.2f, Mathf.Sin(t * Mathf.PI));
        }

        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Exposure",
                Mathf.Lerp(0.2f, 1.3f, Mathf.Sin(t * Mathf.PI)));
        }

        DynamicGI.UpdateEnvironment();
    }

    private void TogglePause()
    {
        if (!isPaused)
        {
            OpenPause();
        }
        else
        {
            ClosePause();
        }
    }
    private void OpenPause()
    {
        isPaused = true;

        pauseCanvas.SetActive(true);

        pauseAnimator.Play("PauseOpen");

        // Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ClosePause()
    {
        isPaused = false;

        pauseAnimator.Play("PauseClose");

        // Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
