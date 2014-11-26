using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class NavMeshOccluder : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up + Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((Vector3.up + Vector3.right - Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up + Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((Vector3.up - Vector3.right + Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up + Vector3.right - Vector3.forward) * 0.5f), transform.TransformPoint((Vector3.up - Vector3.right - Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up - Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((Vector3.up - Vector3.right - Vector3.forward) * 0.5f));

        Gizmos.DrawLine(transform.TransformPoint((-Vector3.up + Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up + Vector3.right - Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((-Vector3.up + Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up - Vector3.right + Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((-Vector3.up + Vector3.right - Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up - Vector3.right - Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((-Vector3.up - Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up - Vector3.right - Vector3.forward) * 0.5f));

        Gizmos.DrawLine(transform.TransformPoint((Vector3.up + Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up + Vector3.right + Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up + Vector3.right - Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up + Vector3.right - Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up - Vector3.right + Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up - Vector3.right + Vector3.forward) * 0.5f));
        Gizmos.DrawLine(transform.TransformPoint((Vector3.up - Vector3.right - Vector3.forward) * 0.5f), transform.TransformPoint((-Vector3.up - Vector3.right - Vector3.forward) * 0.5f));
    }
}
