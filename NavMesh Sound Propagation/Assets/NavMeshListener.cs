using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavMeshListener : NavMeshNode {

    public float[] filter = new float[AudioGeneratorTest.MAX_FIR_LENGTH];

    public List<List<BaseFilter>> threadSafeFilters = new List<List<BaseFilter>>();

    public float normalizer = 0f;
    public float unitScalar = 10f;
    public float falloffScalar = 10f;

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
        // Draw a blue sphere at the transform's position
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

        //DrawPaths();
    }

    void Update()
    {
        DrawPaths();
    }

    public void DrawPaths()
    {
        List<NavMeshPath> paths = new List<NavMeshPath>();

        NavMeshSource[] sources = FindObjectsOfType<NavMeshSource>();
        // for all sources, find the path
        foreach (NavMeshSource source in sources)
        {
            NavMeshPathFinder pathFinder = new NavMeshPathFinder(this, source);
            float maxPaths = 10f;
            float minLength = 0f;

            while (maxPaths > 0)
            {
                //maxPaths--;
                NavMeshPath path = pathFinder.FindPath(minLength);
                if (path == null) break;
                minLength = path.length * 1f;
                DrawPath(path);
                paths.Add(path);
            }
        }

        MakeFilter(paths);
    }

    public void MakeFilter(List<NavMeshPath> paths)
    {
        List<BaseFilter> filters = new List<BaseFilter>();
        normalizer = 1f / paths.Count;
        foreach (NavMeshPath p in paths)
        {
            AddPathToFilter(p, filters);
        }
        threadSafeFilters.Add(filters);
    }

    public void AddPathToFilter(NavMeshPath p, List<BaseFilter> filters)
    {
        float length = p.length / unitScalar;
        float speedSound = 340f;
        int delay = (int)((length / speedSound) * 44100);
        float volume = Mathf.Min(1f, (falloffScalar * falloffScalar) / (length * length));

        if (p.occluded)
            filters.Add(new LowPassFilter(new FilterSettings(volume, normalizer, delay)));
        else
            filters.Add(new NormalFilter(new FilterSettings(volume, normalizer, delay)));
    }

    public void DrawPath(NavMeshPath path)
    {
        Vector3 offset = Vector3.zero.mutate(0.2f);
        Vector3 previous = transform.position + offset;
        //Gizmos.color = Color.magenta;
        foreach (NavMeshNode n in path)
        {
            Vector3 current = n.transform.position + offset;
            //Gizmos.DrawLine(previous, current);
            previous = current;
        }
    }
}
