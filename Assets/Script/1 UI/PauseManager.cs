using UnityEngine;
using UnityEngine.UI;
using PapayaKit.Core;


namespace Papayakit.Ihsan
{
public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Animator pauseAnimator;
    [SerializeField] private Button closeButton;

    public bool IsPaused { get; private set; }

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<PauseButtonRequested>(OnPauseRequested);
        GameContext.SceneEvents.Subscribe<PauseCloseRequested>(OnPauseCloseRequested);
        GameContext.SceneEvents.Subscribe<GameFinished>(OnGameFinished);
    }

    private void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }

    private void OnPauseRequested(PauseButtonRequested evt)
    {
        if (IsPaused)
            ClosePause();
        else
            OpenPause();
    }

    private void OnPauseCloseRequested(PauseCloseRequested evt)
    {
        ClosePause();
    }

    private void OpenPause()
    {
        IsPaused = true;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        if (pauseAnimator != null)
            pauseAnimator.Play("PauseOpen");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameContext.SceneEvents.Publish(new GamePausedChanged(true));
    }

    public void ClosePause()
    {
        if (!IsPaused)
            return;

        IsPaused = false;

        if (pauseAnimator != null)
            pauseAnimator.Play("PauseClose");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameContext.SceneEvents.Publish(new GamePausedChanged(false));
    }

    private void OnGameFinished(GameFinished evt)
    {
        if (IsPaused)
            ClosePause();
    }

    private void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<PauseButtonRequested>(OnPauseRequested);
        GameContext.SceneEvents.Unsubscribe<PauseCloseRequested>(OnPauseCloseRequested);
        GameContext.SceneEvents.Unsubscribe<GameFinished>(OnGameFinished);
    }
}
}