using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class CameraBoost : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("FOV Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float boostFOV = 80f;
    [SerializeField] private float speed = 5f;

    private bool _boosting;

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<CameraBoostChanged>(OnBoostChanged);
    }

    private void Update()
    {
        if (cam == null)
            return;

        float targetFOV = _boosting ? boostFOV : normalFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * speed
        );
    }

    private void OnBoostChanged(CameraBoostChanged evt)
    {
        _boosting = evt.IsBoosting;
    }

    private void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<CameraBoostChanged>(OnBoostChanged);
    }
}
}