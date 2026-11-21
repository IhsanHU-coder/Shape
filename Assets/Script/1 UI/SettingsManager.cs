using UnityEngine;
using UnityEngine.UI;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class SettingsManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private Slider musicSlider;

    [Header("Day Time")]
    [SerializeField] private Slider dayTimeSlider;
    [SerializeField] private Light sun;
    [SerializeField] private Material skyboxMaterial;

    [Header("Reverse Scroll")]
    [SerializeField] private Toggle reverseScrollToggle;

    private const string MusicKey = "MusicVolume";
    private const string TimeKey = "DayTime";
    private const string ReverseScrollKey = "ReverseScroll";

    public bool ReverseScroll { get; private set; }

    private void Start()
    {
        LoadSettings();

        musicSlider.onValueChanged.AddListener(SetVolume);
        dayTimeSlider.onValueChanged.AddListener(UpdateTime);
        reverseScrollToggle.onValueChanged.AddListener(SetReverseScroll);
    }

    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat(MusicKey, 1f);
        float time = PlayerPrefs.GetFloat(TimeKey, 12f);

        ReverseScroll = PlayerPrefs.GetInt(ReverseScrollKey, 0) == 1;

        musicSlider.value = volume;
        dayTimeSlider.value = time;
        reverseScrollToggle.isOn = ReverseScroll;

        AudioListener.volume = volume;

        UpdateTime(time);
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();

        GameContext.SceneEvents.Publish(
            new MusicVolumeChanged(value)
        );
    }

    private void UpdateTime(float hour)
    {
        PlayerPrefs.SetFloat(TimeKey, hour);
        PlayerPrefs.Save();

        float t = hour / 24f;

        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(
                Mathf.Lerp(-90f, 270f, t),
                -30f,
                0f
            );

            sun.intensity = Mathf.Lerp(
                0.1f,
                1.2f,
                Mathf.Sin(t * Mathf.PI)
            );
        }

        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat(
                "_Exposure",
                Mathf.Lerp(
                    0.2f,
                    1.3f,
                    Mathf.Sin(t * Mathf.PI)
                )
            );
        }

        DynamicGI.UpdateEnvironment();

        GameContext.SceneEvents.Publish(new DayTimeChanged(hour));
    }

    private void SetReverseScroll(bool value)
    {
        ReverseScroll = value;

        PlayerPrefs.SetInt(
            ReverseScrollKey,
            value ? 1 : 0
        );

        PlayerPrefs.Save();

        GameContext.SceneEvents.Publish(
            new ReverseScrollChanged(value)
        );
    }

    private void OnDestroy()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetVolume);

        if (dayTimeSlider != null)
            dayTimeSlider.onValueChanged.RemoveListener(UpdateTime);

        if (reverseScrollToggle != null)
            reverseScrollToggle.onValueChanged.RemoveListener(SetReverseScroll);
    }
}
}