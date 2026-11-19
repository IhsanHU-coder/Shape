using System.Collections.Generic;
using UnityEngine;

public class AllLevelStart : MonoBehaviour
{
    public static AllLevelStart Instance;

    public List<WorldMapData> maps = new();

    private void Awake()
    {
        Instance = this;
        Debug.Log(name + " menjadi Instance");

        foreach (var map in maps)
        {
            map.targetPos = map.positionA.position;
            map.targetRot = map.positionA.rotation;
            map.targetScale = map.normalScale;
        }
    }

    private void Update()
    {
        
        foreach (var map in maps)
        {
            Debug.Log("AllLevelStart : " +
    map.targetCanvas.name +
    " ID = " +
    map.targetCanvas.GetInstanceID());
            map.targetCanvas.position = Vector3.Lerp(
                map.targetCanvas.position,
                map.targetPos,
                map.moveSpeed * Time.deltaTime);

            map.targetCanvas.rotation = Quaternion.Slerp(
                map.targetCanvas.rotation,
                map.targetRot,
                map.rotationSpeed * Time.deltaTime);

            map.targetCanvas.localScale = Vector3.Lerp(
                map.targetCanvas.localScale,
                map.targetScale,
                map.scaleSpeed * Time.deltaTime);
        }
    }

    public void Open(WorldMapData map)
    {
        map.targetPos = map.positionB.position;
        map.targetRot = map.positionB.rotation;
        map.targetScale = map.zoomScale;
    }

    public void Close(WorldMapData map)
    {
        map.targetPos = map.positionA.position;
        map.targetRot = map.positionA.rotation;
        map.targetScale = map.normalScale;
    }
}