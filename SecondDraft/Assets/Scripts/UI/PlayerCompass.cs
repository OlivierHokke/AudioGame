using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCompass : MonoBehaviour
{
    private static PlayerCompass instance;
    void Awake()
    {
        instance = this;
    }

    public static void SetTarget(GameObject targetObject)
    {
        instance.target = targetObject;
    }

    public static void SetSource(GameObject sourceObject)
    {
        instance.source = sourceObject;
    }

    public GameObject source;
    public GameObject target;
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = source.transform.InverseTransformPoint(target.transform.position);
        float angle = direction.castxz().angle();

        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.forward, angle * 180f / Mathf.PI);
	}
}
