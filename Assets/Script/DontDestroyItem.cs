using UnityEngine;

public class DontDestroyItem : MonoBehaviour
{
    public static DontDestroyItem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
