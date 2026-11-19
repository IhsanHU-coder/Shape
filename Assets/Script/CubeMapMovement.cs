using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CubeMapMovement : MonoBehaviour
{
    [Header("Roll Settings")]
    [SerializeField] private float cubeSize = 1f;
    [SerializeField] private float rollSpeed = 360f;

    [Header("Current Node")]
    [SerializeField] private PathNode currentNode;

    private bool moving = false;
    public bool IsMoving => moving;

    private LevelNode targetLevel;

    public WorldMapManager worldMapManager;
    public LevelNode CurrentLevelNode => currentNode as LevelNode;
    public bool pres = false;
    public GameObject textEnter;
    // public Animator enterAnim;

    public void MoveTo(LevelNode target)
    {
        if (moving)
            return;

            Debug.Log($"Current = {(currentNode as LevelNode)?.levelID}");
    Debug.Log($"Target  = {target.levelID}");

        targetLevel = target;

        List<PathNode> path = FindPath(currentNode, target);

        if (path == null)
            return;

        StartCoroutine(FollowPath(path));
    }

    IEnumerator FollowPath(List<PathNode> path)
    {
        moving = true;

        // index 0 adalah node tempat player berada sekarang
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 direction =
                path[i].transform.position -
                transform.position;

            direction.y = 0;
            direction.Normalize();

            yield return RollOneStep(direction);

            currentNode = path[i];
            if (currentNode is LevelNode level)
            {
                SaveManager.SaveCurrentLevel(level.levelID);
            }
        }

        moving = false;
        currentNode = path[path.Count - 1];


        // WorldMapManager.Instance.ShowEnterLevel(targetLevel);
    }

    IEnumerator RollOneStep(Vector3 direction)
    {
        Vector3 pivot =
            transform.position +
            Vector3.down * cubeSize * 0.5f +
            direction * cubeSize * 0.5f;

        Vector3 axis =
            Vector3.Cross(Vector3.up, direction);

        float angle = 0;

        while (angle < 90)
        {
            float step = rollSpeed * Time.deltaTime;

            if (angle + step > 90)
                step = 90 - angle;

            transform.RotateAround(pivot, axis, step);

            angle += step;

            yield return null;
        }

        SnapToGrid();
    }

    void SnapToGrid()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Round(pos.x / cubeSize) * cubeSize;
        pos.y = Mathf.Round(pos.y / cubeSize) * cubeSize;
        pos.z = Mathf.Round(pos.z / cubeSize) * cubeSize;

        transform.position = pos;

        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        transform.rotation =
            Quaternion.LookRotation(
                GetClosestAxis(forward),
                GetClosestAxis(up));
    }

    Vector3 GetClosestAxis(Vector3 v)
    {
        float x = Mathf.Abs(v.x);
        float y = Mathf.Abs(v.y);
        float z = Mathf.Abs(v.z);

        if (x > y && x > z)
            return new Vector3(Mathf.Sign(v.x), 0, 0);

        if (y > x && y > z)
            return new Vector3(0, Mathf.Sign(v.y), 0);

        return new Vector3(0, 0, Mathf.Sign(v.z));
    }

    List<PathNode> FindPath(PathNode start, PathNode goal)
    {
        List<PathNode> result = new List<PathNode>();

        if (Search(start, goal, result, new HashSet<PathNode>()))
            return result;

        return null;
    }

    bool Search(PathNode current,
                PathNode goal,
                List<PathNode> path,
                HashSet<PathNode> visited)
    {
        Debug.Log("Visit : " + current.name);
        if (visited.Contains(current))
            return false;

        visited.Add(current);

        path.Add(current);

        if (current == goal)
            return true;

        foreach (PathNode next in current.connectedNodes)
        {
            if (next == null)
                continue;

            if (!next.unlocked)
                continue;

            if (Search(next, goal, path, visited))
                return true;
        }

        path.RemoveAt(path.Count - 1);

        return false;
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     LevelNode level = other.GetComponent<LevelNode>();

    //     if (level == null)
    //         return;

    //     SaveManager.SaveCurrentLevel(level.levelID);

    //     if (level != targetLevel)
    //         return;

    //     WorldMapManager.Instance.ShowEnterLevel(level);
    // }
    private void OnTriggerEnter(Collider other)
{
    LevelNode level = other.GetComponent<LevelNode>();

    if (level == null)
        return;
    if (level == targetLevel)
    {
        WorldMapManager.Instance.ShowEnterLevel(level);
    }
    SaveManager.SaveCurrentLevel(level.levelID);

    currentNode = level;
}
    // private void OnTriggerStay(Collider other)
    // {
    //     if (Keyboard.current.enterKey.wasPressedThisFrame)
    //     {
    //         LevelNode level = other.GetComponent<LevelNode>();

    //         if (level == null)
    //             return;

    //         // Pastikan ini memang level tujuan
    //         if (level != targetLevel)
    //             return;
    //         WorldMapManager.Instance.ShowEnterLevel(level);
    //     }
    // }
    private void OnTriggerStay(Collider other)
{
        // textEnter.SetActive(true);

    if (Keyboard.current.enterKey.wasPressedThisFrame){


    LevelNode level = other.GetComponent<LevelNode>();

    if (level == null)
        return;

    WorldMapManager.Instance.ShowEnterLevel(level);
    }
}

private void OnTriggerExit(Collider other)
{
    WorldMapManager.Instance.CloseTrig();
    textEnter.SetActive(false);

}
    public void SetCurrentNode(PathNode node)
    {
        currentNode = node;
    }
    private void Update()
{
    
    // if (moving)
    //     return;
    // if (pres)return;
    // if (Keyboard.current.enterKey.wasPressedThisFrame)
    // {
    //     LevelNode level = currentNode as LevelNode;

    //     if (level != null)
    //     {
    //     targetLevel = level;
    //         WorldMapManager.Instance.ShowEnterLevel(level);
    //         pres = true;
    //     }
    // }
}
}