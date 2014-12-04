using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float walkUnitsPerSecond= 4f;
    public float anglesPerPixel = 0.5f;
    public Camera camera;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Vertical"), Space.Self);
        transform.Translate(Vector3.right * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Horizontal"), Space.Self);

        transform.rotation = transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * anglesPerPixel, Vector3.up);
        camera.transform.rotation = camera.transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * anglesPerPixel, Vector3.left);
	}
}
