using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshNode : MonoBehaviour
{
    [Header("NavMesh")]
    public List<NavMeshEdge> connections;

    [Header("Gizmo")]
    public float gizmoSize = 0.3f;

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (NavMeshEdge edge in connections)
        {
            Gizmos.DrawLine(transform.position, edge.target.transform.position);
        }
    }

    void OnValidate()
    {
        // check all connections
        CheckConnections();
    }

    void CheckConnections()
    {
        foreach (NavMeshEdge edge in connections)
        {
            // does target have me included? if not add it
            NavMeshNode node = edge.target;
            if (node == null) continue;
            if (node.connections == null) continue;

            bool found = false;
            foreach (NavMeshEdge otherEdge in node.connections)
            {
                if (otherEdge.target == this)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                node.connections.Add(new NavMeshEdge(this, edge.occlude));
            }
        }
    }

    void OnDestroy()
    {
        foreach (NavMeshEdge edge in connections)
        {
            // does target have me included? if not add it
            NavMeshNode node = edge.target;
            if (node == null) continue;
            if (node.connections == null) continue;

            for (int i = 0; i < node.connections.Count; i++)
            {
                if (node.connections[i].target == this)
                {
                    node.connections.Remove(node.connections[i]);
                    i--;
                }
            }
        }
    }

}
