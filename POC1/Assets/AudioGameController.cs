using UnityEngine;
using System.Collections;

public class AudioGameController : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        lastMousePosition = Input.mousePosition;
	}

    public float walkUnitsPerSecond= 4f;
    public float anglesPerPixel = 0.5f;
    public Vector3 lastMousePosition;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Vertical"), Space.Self);
        transform.Translate(Vector3.right * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Horizontal"), Space.Self);

        transform.rotation = transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * anglesPerPixel, Vector3.up);
        lastMousePosition = Input.mousePosition;
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("yay");
        Destroy(col.gameObject);
        GetComponent<AudioSource>().Play();
    }
}
