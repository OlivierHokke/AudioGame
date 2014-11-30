using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavMeshListener : NavMeshNode {


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(0.3f, 0.5f, 1f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 2f);
        Gizmos.color = new Color(0f, 0f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 8f);
    }

    void OnDrawGizmosSelected()
    {
        NavMeshNode[] nmns = FindObjectsOfType<NavMeshNode>();
        Vector3 pos = transform.position;

        Gizmos.color = new Color(1f,1f,1f,0.3f);

        foreach (NavMeshNode node in nmns)
        {
            if (node.IsInside(pos))
            {
                Gizmos.DrawLine(pos, node.transform.position);
            }
        }

        DrawPaths();
    }

    public List<NavMeshNode> GetNodes()
    {
        NavMeshNode[] nmns = FindObjectsOfType<NavMeshNode>();
        Vector3 pos = transform.position;
        List<NavMeshNode> nodes = new List<NavMeshNode>();
        foreach (NavMeshNode node in nmns)
        {
            if (node.IsInside(pos)) nodes.Add(node);
        }
        return nodes;
    }

    public void DrawPaths()
    {
        NavMeshSource[] sources = FindObjectsOfType<NavMeshSource>();
        List<NavMeshNode> nodes = GetNodes();

        // for all sources, find the path
        foreach (NavMeshSource source in sources)
        {
            List<NavMeshPath> todo = new List<NavMeshPath>();
            Vector3 sourcePosition = source.transform.position;

            // populate the start paths, by add all adjecent nodes
            foreach (NavMeshNode n in nodes)
            {
                float length = Vector3.Distance(this.transform.position, n.transform.position);
                NavMeshPath path = new NavMeshPath(this, n, length, this.IsOccluded(n));
                path.score = path.length + Vector3.Distance(sourcePosition, n.transform.position);
                todo.Add(path);
            }

            {
                // also add the trivial path
                float length = Vector3.Distance(sourcePosition, transform.position);
                NavMeshPath path = new NavMeshPath(this, source, length, this.IsOccluded(source));
                path.score = length;
                todo.Add(path);
            }

            todo = todo.OrderBy(o => o.score).ToList();

            float latestDrawnPathLength = 0f;
            int maxIterations = 30;
            int maxPaths = 5;
            int sourceArrivalCount = 0;

            // path find until no more options
            while (todo.Count > 0 && maxIterations > 0 && maxPaths > 0)
            {
                maxIterations--;
                NavMeshPath currentPath = todo[0];
                NavMeshNode currentNode = currentPath.Last();
                todo.RemoveAt(0);

                if (currentNode == source)
                {
                    sourceArrivalCount++;
                    //log += currentPath.length + " > " + (latestDrawnPathLength * 1.2f) + " = " + (currentPath.length > latestDrawnPathLength * 1.2f) + "\n";
                    if (currentPath.length > latestDrawnPathLength * 1.2f)
                    {
                        DrawPath(currentPath);
                        latestDrawnPathLength = currentPath.length;
                        maxPaths--;
                    }
                }
                else
                {
                    {
                        // first add the trivial path from current node to source
                        float remainder = Vector3.Distance(sourcePosition, currentNode.transform.position);
                        NavMeshPath trivialPath = new NavMeshPath(currentPath, source, remainder, currentNode.IsOccluded(source));
                        trivialPath.score = trivialPath.length;
                        todo.Add(trivialPath);
                    }

                    foreach (NavMeshEdge e in currentNode.edges)
                    {
                        // add new neighbouring edges
                        NavMeshNode nextNode = e.GetTarget(currentNode);
                        if (currentPath.Contains(nextNode)) continue;
                        float remainder = Vector3.Distance(sourcePosition, nextNode.transform.position);
                        NavMeshPath nextPath = new NavMeshPath(currentPath, e);
                        nextPath.score = nextPath.length + remainder;
                        todo.Add(nextPath);
                    }

                    todo = todo.OrderBy(o => o.score).ToList();
                }
            }

            foreach (NavMeshPath p in todo)
            {
                string log = "";
                Vector3 previous = transform.position;
                float length = 0f;
                foreach (NavMeshNode n in p)
                {
                    length += Vector3.Distance(n.transform.position, previous);
                    log += length + "\n";
                    previous = n.transform.position;
                }
                //Debug.Log(log);
            }
        }
    }

    public void DrawPath(NavMeshPath path)
    {
        Vector3 offset = Vector3.zero.mutate(0.2f);
        Vector3 previous = transform.position + offset;
        Gizmos.color = Color.magenta;
        foreach (NavMeshNode n in path)
        {
            Vector3 current = n.transform.position + offset;
            Gizmos.DrawLine(previous, current);
            previous = current;
        }
    }
}
