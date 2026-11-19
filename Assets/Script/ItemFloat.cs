using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private bool floating = true;
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotateSpeed = 60f;

    [Header("Glow")]
    [SerializeField] private bool glow = true;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowIntensity = 3f;

    private Vector3 startPos;
    private Material mat;

    void Start()
    {
        startPos = transform.position;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
        {
            mat = targetRenderer.material;

            if (glow)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", glowColor * glowIntensity);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    void Update()
    {
        if (floating)
        {
            transform.position = startPos +
                Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        }

        if (rotate)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    public void SetGlow(bool value)
    {
        glow = value;

        if (mat == null) return;

        if (glow)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
        }
    }
}