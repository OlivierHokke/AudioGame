using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public float walkUnitsPerSecond= 4f;
    public float anglesPerPixel = 0.5f;
    new public Camera camera;
    public float maxLookVertical = 25f;
    public float minLookVertical = -25f;
    public float currentLookVertical = 0f;

    public event EventHandler<TriggerEventArgs> TriggerEntered;

    public bool LockMovement = false;

	// Update is called once per frame
	void Update ()
    {
        if (!LockMovement)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Vertical"), Space.Self);
            transform.Translate(Vector3.right * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Horizontal"), Space.Self);
        }

        transform.rotation = transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * anglesPerPixel, Vector3.up);
        currentLookVertical += Input.GetAxis("Mouse Y") * anglesPerPixel;
        currentLookVertical = Mathf.Clamp(currentLookVertical, minLookVertical, maxLookVertical);
        camera.transform.localEulerAngles = -Vector3.right * currentLookVertical;
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (TriggerEntered != null)
            TriggerEntered(this, new TriggerEventArgs(other));
    }


}
