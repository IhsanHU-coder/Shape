using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class SceneTransitionController : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private Animator outro;
    [SerializeField] private float transitionDelay = 3.5f;

    private bool _isTransitioning;

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<SceneTransitionRequested>(OnSceneTransition);
        GameContext.SceneEvents.Subscribe<RetryButtonRequested>(OnRetryRequested);
    }

    private void Start()
    {
        if (outro != null)
            outro.Play("intro");
    }

    public void LoadScene(string sceneName)
    {
        if (_isTransitioning)
            return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isTransitioning = true;

        if (outro != null)
            outro.Play("outro");

        yield return new WaitForSeconds(transitionDelay);

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneTransition(SceneTransitionRequested evt)
    {
        LoadScene(evt.SceneName);
    }

    private void OnRetryRequested(RetryButtonRequested evt)
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<SceneTransitionRequested>(OnSceneTransition);
        GameContext.SceneEvents.Unsubscribe<RetryButtonRequested>(OnRetryRequested);
    }
}
}