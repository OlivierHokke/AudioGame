using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavMeshEdge : IEqualityComparer<NavMeshEdge>, IEquatable<NavMeshEdge>
{
    public NavMeshNode Node1;
    public NavMeshNode Node2;
    public bool Occluded;
    public float Length { get; private set; }

    public NavMeshEdge(NavMeshNode node1, NavMeshNode node2, bool occluded)
    {
        this.Node1 = node1;
        this.Node2 = node2;
        this.Length = (node1.transform.position - node2.transform.position).magnitude;

        this.Occluded = Physics.Raycast(Node1.transform.position, node2.transform.position - Node1.transform.position, this.Length);
    }

    public NavMeshNode GetTarget(NavMeshNode source)
    {
        if (source == Node1) return Node2;
        else return Node1;
    }

    public bool HasNode(NavMeshNode source)
    {
        return (Node1 == source || Node2 == source);
    }

    public void DrawGizmo(float alpha = 1f)
    {
        if (Node1 == null || Node2 == null) return;
        Gizmos.color = Occluded ? new Color(1f, 0f, 0.4f, alpha * 2f) : new Color(0.1f, 1f, 0f, alpha * 0.5f);
        Gizmos.DrawLine(Node1.transform.position, Node2.transform.position);
    }

    public bool Equals(NavMeshEdge x, NavMeshEdge y)
    {
        return x.HasNode(y.Node1) && x.HasNode(y.Node2);
    }

    public int GetHashCode(NavMeshEdge obj)
    {
        if (obj == null) return 0;
        return obj.Node1.GetHashCode() ^ obj.Node2.GetHashCode();
    }

    public bool Equals(NavMeshEdge other)
    {
        return HasNode(other.Node1) && HasNode(other.Node2);
    }
}
