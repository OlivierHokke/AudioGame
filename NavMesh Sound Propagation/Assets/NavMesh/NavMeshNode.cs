using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavMeshNode : MonoBehaviour
{
    [Header("Gizmo")]
    public float gizmoSize = 0.3f;

    [Header("Auto Generated")]
    public NavMeshManager manager;
    public List<NavMeshEdge> edges;

    public void ValidateEdges()
    {
        for (int i = 0; i < edges.Count; i++)
            if (!edges[i].HasNode(this))
                edges.RemoveAt(i--);
        edges = edges.Distinct().ToList();
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        foreach (NavMeshEdge edge in edges) edge.DrawGizmo(0.1f);
    }

    void OnDrawGizmosSelected()
    {
        foreach (NavMeshEdge edge in edges) edge.DrawGizmo();
    }

    void OnValidate()
    {
        if (edges == null)
        {
            edges = new List<NavMeshEdge>();
        }
    }

}
