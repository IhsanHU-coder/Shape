using UnityEngine;
using TMPro;

[System.Serializable]
public class WorldMapData
{
    [Header("Player")]
    public CubeMapMovement player;

    [Header("UI")]
    public TextMeshProUGUI levelName;

    [Header("Camera Target")]
    public Transform targetCanvas;

    public Transform positionA;
    public Transform positionB;

    [Header("Animation")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float scaleSpeed = 5f;

    [Header("Scale")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 zoomScale = Vector3.one * 8f;

    public Vector3 targetPos;
     public Quaternion targetRot;
    public Vector3 targetScale;
}