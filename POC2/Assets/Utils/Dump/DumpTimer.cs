using UnityEngine;
using System.Collections;

public class DumpTimer : MonoBehaviour {
    
    public float minLifetime = 10f;
    public float maxLifetime = 20f;
    private float myLifetime;

    public void SetLifetime(float lt)
    {
        minLifetime = lt;
        maxLifetime = lt;
    }
    public void SetLifetime(float min, float max)
    {
        minLifetime = min;
        maxLifetime = max;
    }

    void Start()
    {
        myLifetime = Random.Range(minLifetime, maxLifetime);
    }

	// Update is called once per frame
	void Update () {
        myLifetime -= Time.deltaTime;
        if (myLifetime < 0)
        {
            GameObject.Destroy(gameObject);
        }
	}
}
