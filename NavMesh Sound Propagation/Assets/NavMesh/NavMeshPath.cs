using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NavMeshPath : List<NavMeshNode>
{
    public float length = 0f;
    public bool occluded = false;
    public float score = 0f;
    public float remainderScore = 0f;

    public NavMeshPath() { }

    public NavMeshPath(NavMeshPath path, NavMeshNode next, float remainderScore, float penalty= 0f)
        : base(path)
    {
        this.remainderScore = remainderScore;
        this.score = remainderScore + penalty;

        if (path.Count > 0) // the other path contains some info we need to take into account
        {
            NavMeshNode from = path.Last();
            float extraLength = from.Distance(next);

            this.length = path.length + extraLength;
            this.occluded = path.occluded || from.IsOccluded(next);
            this.score += path.score - path.remainderScore + extraLength;
        }

        Add(next);
    }

    public override string ToString()
    {
        string path = Count + "; ";
        path += "Length  = " + length + "; Score = " + score + " ; ";
        foreach (NavMeshNode n in this)
        {
            path += n.GetType().Name + " (" + n.GetHashCode() + "), ";
        }
        return path;
    }
}
