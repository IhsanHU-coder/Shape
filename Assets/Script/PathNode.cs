using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Connected Nodes")]
    public List<PathNode> connectedNodes = new List<PathNode>();

    
    public bool unlocked = true;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (PathNode node in connectedNodes)
        {
            if (node != null)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
#endif
}