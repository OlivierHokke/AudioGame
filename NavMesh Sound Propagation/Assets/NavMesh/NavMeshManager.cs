using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class NavMeshManager : MonoBehaviour {

    public List<NavMeshEdge> connections;

    void OnValidate()
    {
        CheckConnections();
    }

    void CheckConnections()
    {
        NavMeshNode[] allNodes = transform.root.GetComponentsInChildren<NavMeshNode>();

        foreach (NavMeshNode n in allNodes)
        {
            n.edges = new List<NavMeshEdge>();
            n.manager = this;
        }

        foreach (NavMeshEdge e in connections)
        {
            AddEdgeToNode(e, e.Node1);
            AddEdgeToNode(e, e.Node2);
        }
    }

    void AddEdgeToNode(NavMeshEdge edge, NavMeshNode node)
    {
        if (node == null) return;
        if (!node.edges.Contains(edge))
        {
            node.edges.Add(edge);
        }
    }

    [MenuItem("NavMesh/Disconnect Selected Nodes [Fully] %#d")]
    static void DisconnectSelectedNodes()
    {
        Transform[] ts = Selection.GetTransforms(SelectionMode.OnlyUserModifiable);
        HandleConnection(ts, ts, true, false);
    }

    [MenuItem("NavMesh/Connect Selected Nodes [Fully] %#c")]
    static void ConnectSelectedNodes()
    {
        Transform[] ts = Selection.GetTransforms(SelectionMode.OnlyUserModifiable);
        HandleConnection(ts, ts, false, false);
    }

    [MenuItem("NavMesh/Connect Selected Nodes [Fully + Occluded] %#&c")]
    static void ConnectSelectedNodesOccluded()
    {
        Transform[] ts = Selection.GetTransforms(SelectionMode.OnlyUserModifiable);
        HandleConnection(ts, ts, false, true);
    }

    [MenuItem("NavMesh/Smart Connect (6-connected) %#&s")]
    static void SmartConnect6()
    {
        Vector3[] directions = new Vector3[] { Vector3.up, Vector3.right, Vector3.forward };
        object[] all = Selection.GetFiltered(typeof(NavMeshNode), SelectionMode.Unfiltered);
        HandleSmartConnection(all, directions);
    }

    [MenuItem("NavMesh/Smart Connect (12-connected)")]
    static void SmartConnect12()
    {
        Vector3[] directions = new Vector3[] { Vector3.up, Vector3.right, Vector3.forward, 
            (Vector3.up + Vector3.right).normalized,
            (Vector3.up + Vector3.left).normalized,
            (Vector3.up + Vector3.forward).normalized,
            (Vector3.up + Vector3.back).normalized,
            (Vector3.right + Vector3.forward).normalized,
            (Vector3.right + Vector3.back).normalized
        };
        object[] all = Selection.GetFiltered(typeof(NavMeshNode), SelectionMode.Unfiltered);
        HandleSmartConnection(all, directions);
    }

    [MenuItem("NavMesh/Smart Connect (14-connected)")]
    static void SmartConnect14()
    {
        Vector3[] directions = new Vector3[] { Vector3.up, Vector3.right, Vector3.forward, 
            (Vector3.up + Vector3.right + Vector3.forward).normalized,
            (Vector3.up + Vector3.right - Vector3.forward).normalized,
            (Vector3.up - Vector3.right + Vector3.forward).normalized,
            (Vector3.up - Vector3.right - Vector3.forward).normalized
        };
        object[] all = Selection.GetFiltered(typeof(NavMeshNode), SelectionMode.Unfiltered);
        HandleSmartConnection(all, directions);
    }

    [MenuItem("NavMesh/Smart Connect (20-connected)")]
    static void SmartConnect20()
    {
        Vector3[] directions = new Vector3[] { Vector3.up, Vector3.right, Vector3.forward, 
            (Vector3.up + Vector3.right).normalized,
            (Vector3.up + Vector3.left).normalized,
            (Vector3.up + Vector3.forward).normalized,
            (Vector3.up + Vector3.back).normalized,
            (Vector3.right + Vector3.forward).normalized,
            (Vector3.right + Vector3.back).normalized,
            
            (Vector3.up + Vector3.right + Vector3.forward).normalized,
            (Vector3.up + Vector3.right - Vector3.forward).normalized,
            (Vector3.up - Vector3.right + Vector3.forward).normalized,
            (Vector3.up - Vector3.right - Vector3.forward).normalized,
        };
        object[] all = Selection.GetFiltered(typeof(NavMeshNode), SelectionMode.Unfiltered);
        HandleSmartConnection(all, directions);
    }

    static void HandleSmartConnection(object[] all, Vector3[] directions)
    {
        NavMeshManager manager = null;
        foreach (NavMeshNode nodeSource in all)
        {
            if (manager == null) manager = nodeSource.transform.root.GetComponentInChildren<NavMeshManager>();

            foreach (Vector3 direction in directions)
            {
                NavMeshNode bestTarget = null;
                float bestDistance = float.MaxValue;
                foreach (NavMeshNode nodeTarget in all)
                {
                    if (nodeSource == nodeTarget) continue;
                    // get position of target node in local space of current node
                    Vector3 local = nodeSource.transform.InverseTransformPoint(nodeTarget.transform.position);
                    // get the distance from source to target
                    float distance = local.magnitude;
                    // get the amount of times the direction should be used to get as close to the target as possible on the directional line
                    float directionalDist = Vector3.Dot(direction, local);
                    // if the distance is much larger than de direction distance, then the target is probably not on the directional line
                    if (directionalDist < 0 || distance > directionalDist * 1.1f) continue;
                    else if (distance < bestDistance)
                    {
                        bestTarget = nodeTarget;
                        bestDistance = distance;
                    }
                }
                if (bestTarget)
                {
                    NavMeshEdge edge = new NavMeshEdge(nodeSource, bestTarget, false);
                    if (!manager.connections.Contains(edge))
                    {
                        manager.connections.Add(edge);
                    }
                }
            }
        }

        manager.CheckConnections();
    }

    static void HandleConnection(Transform[] first, Transform[] second, bool disconnect, bool occlude)
    {
        NavMeshManager manager = null;

        foreach (Transform t in first)
        {
            if (manager == null) manager = t.root.GetComponentInChildren<NavMeshManager>();

            NavMeshNode node = t.GetComponent<NavMeshNode>();
            if (node == null) continue;

            foreach (Transform other in second)
            {
                if (other == t) continue;
                NavMeshNode otherNode = other.GetComponent<NavMeshNode>();
                if (otherNode == null) continue;

                NavMeshEdge edge = new NavMeshEdge(node, otherNode, occlude);
                if (disconnect)
                {
                    if (manager.connections.Contains(edge))
                    {
                        manager.connections.Remove(edge);
                    }
                }
                else
                {
                    if (!manager.connections.Contains(edge))
                    {
                        manager.connections.Add(edge);
                    }
                }
            }
        }

        manager.CheckConnections();
    }
}
