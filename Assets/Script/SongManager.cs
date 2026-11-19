using UnityEngine;
using UnityEngine.SceneManagement;

public class SongManager : MonoBehaviour
{
    [Header("Destroy on these scenes")]
    [SerializeField] private string[] destroyScenes;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (string sceneName in destroyScenes)
        {
            if (scene.name == sceneName)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}

