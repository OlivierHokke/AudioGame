using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshSource : NavMeshNode
{

    public override List<NavMeshNode> GetNodes()
    {
        NavMeshNode[] nmns = FindObjectsOfType<NavMeshNode>();
        Vector3 pos = transform.position;
        List<NavMeshNode> nodes = new List<NavMeshNode>();
        foreach (NavMeshNode node in nmns)
        {
            if (node.IsInside(pos) && node != this) nodes.Add(node);
        }
        return nodes;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawSphere(transform.position, gizmoSize);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 3f);
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 5f);
        Gizmos.color = new Color(1f, 0f, 0f, 0.05f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 7f);
    }

    void OnDrawGizmosSelected()
    {
        NavMeshNode[] nmns = FindObjectsOfType<NavMeshNode>();
        Vector3 pos = transform.position;

        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);

        foreach (NavMeshNode node in nmns)
        {
            if (node.IsInside(pos))
            {
                Gizmos.DrawLine(pos, node.transform.position);
            }
        }
    }
}
