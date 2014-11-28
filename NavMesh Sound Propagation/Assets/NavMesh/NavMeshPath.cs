using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NavMeshPath : List<NavMeshNode>
{
    public float length = 0f;
    public bool occluded = false;
    public float score = 0f;

    public NavMeshPath(NavMeshNode start, NavMeshNode end, float length, bool occluded = false)
    {
        Add(start);
        Add(end);
        this.occluded = occluded;
        this.length = length;
    }

    public NavMeshPath(NavMeshPath path, NavMeshEdge next)
        : base(path)
    {
        this.length = path.length + next.Length;
        this.occluded = path.occluded || next.Occluded;
        Add(next.GetTarget(this.Last()));
    }

    public NavMeshPath(NavMeshPath path, NavMeshNode next, float additionalLength, bool occluded = false)
        : base(path)
    {
        this.length = path.length + additionalLength;
        this.occluded = path.occluded || occluded;
        Add(next);
    }
}
