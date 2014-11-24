using UnityEngine;

[System.Serializable]
public class NavMeshEdge
{
    public NavMeshNode target;
    public bool occlude;

    public NavMeshEdge(NavMeshNode target, bool occlude)
    {
        this.target = target;
        this.occlude = occlude;
    }
}
