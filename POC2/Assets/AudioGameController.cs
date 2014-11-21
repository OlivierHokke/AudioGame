using UnityEngine;
using System.Collections;

public class AudioGameController : MonoBehaviour {

    public float walkUnitsPerSecond= 4f;
    public float anglesPerPixel = 0.5f;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Vertical"), Space.Self);
        transform.Translate(Vector3.right * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Horizontal"), Space.Self);

        transform.rotation = transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * anglesPerPixel, Vector3.up);
        Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward * 5f), Color.green, Time.fixedDeltaTime);
	}
}
