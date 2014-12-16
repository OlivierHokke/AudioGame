using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public float walkUnitsPerSecond = 4f;
    public float turnUnitsPerSecond = 180f;
    public float anglesPerPixel = 0.5f;
    public float maxLookVertical = 25f;
    public float minLookVertical = -25f;
    public float currentLookVertical = 0f;
    public float currentLookHorizontal = 0f;

    public bool oculusRiftEnabled;
    public GameObject oculusRiftRoot;
    public GameObject oculusRiftCamera;
    public GameObject normalRoot;
    public GameObject normalCamera;

    public event EventHandler<TriggerEventArgs> TriggerEntered;

    public bool LockMovement = false;
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 fw;
        if (oculusRiftEnabled)
        {
            if (!oculusRiftRoot.activeInHierarchy) oculusRiftRoot.SetActive(true);
            if (normalRoot.activeInHierarchy) normalRoot.SetActive(false);
            PlayerCompass.SetSource(oculusRiftCamera);
            fw = oculusRiftCamera.transform.TransformDirection(Vector3.forward);
        }
        else
        {
            if (oculusRiftRoot.activeInHierarchy) oculusRiftRoot.SetActive(false);
            if (!normalRoot.activeInHierarchy) normalRoot.SetActive(true);
            PlayerCompass.SetSource(normalCamera);
            fw = normalCamera.transform.TransformDirection(Vector3.forward);
        }

        if (!LockMovement)
        {
            fw = fw.sety(0).normalized;
            transform.Translate(fw * Time.deltaTime * walkUnitsPerSecond * Input.GetAxis("Vertical"), Space.Self);
        }

        currentLookHorizontal += Time.deltaTime * turnUnitsPerSecond * Input.GetAxis("Horizontal");
        currentLookHorizontal += Input.GetAxis("Mouse X") * anglesPerPixel;
        normalRoot.transform.localEulerAngles = new Vector3(0f, currentLookHorizontal, 0f);
        normalRoot.transform.localEulerAngles = new Vector3(0f, currentLookHorizontal, 0f);

        //normalRoot.transform.rotation = normalRoot.transform.rotation * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * anglesPerPixel, Vector3.up);
        /*currentLookVertical += Input.GetAxis("Mouse Y") * anglesPerPixel;
        currentLookVertical = Mathf.Clamp(currentLookVertical, minLookVertical, maxLookVertical);
        camera.transform.localEulerAngles = -Vector3.right * currentLookVertical;*/
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
