using UnityEngine;

public class CameraBoost : MonoBehaviour
{
    public Camera cam;

    public float normalFOV = 60;
    public float boostFOV = 80;
    public float speed = 5f;

    public bool boosting;

    void Update()
    {
        float target = boosting ? boostFOV : normalFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            target,
            Time.deltaTime * speed
        );
    }
}