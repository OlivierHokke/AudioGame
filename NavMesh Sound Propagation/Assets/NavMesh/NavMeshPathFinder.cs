using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class NavMeshPathFinder
{
    private const float PENALTY = 30f;
    private const float START_SCORE = -50f;

    private Dictionary<NavMeshNode, List<NavMeshPath>> pathsPerNode = new Dictionary<NavMeshNode, List<NavMeshPath>>();
    private Dictionary<NavMeshNode, float> nodePenalties = new Dictionary<NavMeshNode, float>();
    private List<NavMeshPath> todo = new List<NavMeshPath>();

    private NavMeshNode start;
    private NavMeshNode end;

    public NavMeshPathFinder(NavMeshNode start, NavMeshNode end)
    {
        this.start = start;
        this.end = end;
        InitializePaths();
    }

    private void InitializePaths()
    {
        nodePenalties.Add(end, -20 * PENALTY);
        AddPath(new NavMeshPath(), start);
    }

    private void Sort()
    {
        todo.Sort((a, b) => a.score.CompareTo(b.score));
    }

    private float GetPenalty(NavMeshNode n)
    {
        if (!nodePenalties.ContainsKey(n))
            nodePenalties.Add(n, START_SCORE);
        return nodePenalties[n];
    }

    private void AddPath(NavMeshPath source, NavMeshNode add)
    {
        // Only add if node does not exist in path yet
        if (source.Contains(add)) return;

        // Determine temporary remainder score due to remaining distance
        float remainderScore = add.Distance(end);

        // Make new path, with penalty if applicable
        NavMeshPath path = new NavMeshPath(source, add, remainderScore, GetPenalty(add));

        // Add this path to all nodes for quick reference
        foreach (NavMeshNode n in path)
        {
            if (!pathsPerNode.ContainsKey(n)) 
                pathsPerNode.Add(n, new List<NavMeshPath>());
            pathsPerNode[n].Add(path);
        }

        // Add the new path
        todo.Add(path);
    }

    private void AddSuccessors(NavMeshPath from)
    {
        // Add trivial path
        AddPath(from, end);
        
        // Add connected nodes
        NavMeshNode node = from.Last();
        List<NavMeshNode> nodes = node.GetNodes();
        foreach (NavMeshNode n in nodes)
        {
            AddPath(from, n);
        }
    }

    private void AddPenalty(NavMeshNode n)
    {
        // Check if penalty object already exists
        if (!nodePenalties.ContainsKey(n))
            nodePenalties.Add(n, START_SCORE);

        // Save penalty
        nodePenalties[n] += PENALTY;

        // Add penalty to existing paths
        foreach (NavMeshPath path in pathsPerNode[n])
        {
            path.score += PENALTY;
        }
    }

    private void AddPenalties(NavMeshPath path)
    {
        foreach (NavMeshNode n in path)
        {
            if (n == end || n == start) continue;
            AddPenalty(n);
        }
    }

    private void RemovePath(NavMeshPath removable)
    {
        // Remove path
        todo.Remove(removable);

        // Go over all nodes in the removable path to remove the cache of a path
        foreach (NavMeshNode n in removable)
        {
            pathsPerNode[n].Remove(removable);
        }
    }

    public NavMeshPath FindPath(float minimumLength)
    {
        float maxIterations = 25;
        while (maxIterations > 0 && todo.Count > 0)
        {
            // Sort the todo, to get the path with the lowest score
            Sort();

            // Extract current todo item
            NavMeshPath currentPath = todo[0];
            NavMeshNode currentNode = currentPath.Last();

            // Remove current todo
            RemovePath(currentPath);

            // Check if target reached
            if (currentNode == end)
            {
                // Add penalties for the found path
                AddPenalties(currentPath);

                // Check if path is long enough
                if (currentPath.length >= minimumLength)
                    // Return the found path
                    return currentPath;
                //else
                    //Debug.Log("Ignored path due to its length " + currentPath.length + " ; min = "+ minimumLength);
            }
            else
            {
                // Add successors
                AddSuccessors(currentPath);
            }

            maxIterations--;
        }
        return null;
    }

    private void LogTodo()
    {
        string log = "";
        foreach (NavMeshPath path in todo) {
            log += path + "\n";
        }
        Debug.Log(log);
    }
}
