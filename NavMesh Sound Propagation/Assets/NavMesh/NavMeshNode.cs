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

    public Vector3 min;
    public Vector3 max;

    public void ValidateEdges()
    {
        //for (int i = 0; i < edges.Count; i++)
        //    if (!edges[i].HasNode(this))
        //        edges.RemoveAt(i--);
        //edges = edges.Distinct().ToList();

        min = transform.position;
        max = transform.position;

        foreach (NavMeshEdge edge in edges)
        {
            min = edge.GetTarget(this).transform.position.Min(min);
            max = edge.GetTarget(this).transform.position.Max(max);
        }
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

    public bool IsInside(Vector3 point)
    {
        return point.Inside(min, max);
    }

    public bool IsOccluded(NavMeshNode target)
    {
        Vector3 direction = target.transform.position - this.transform.position;
        return Physics.Raycast(this.transform.position, direction, direction.magnitude);
    }

    void OnValidate()
    {
        if (edges == null)
        {
            edges = new List<NavMeshEdge>();
        }
    }

}
