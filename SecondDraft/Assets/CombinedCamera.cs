using UnityEngine;
using System.Collections;

public class CombinedCamera : MonoBehaviour {

    public Camera normalCamera;
    public Camera oculusCamera;

	// Update is called once per frame
	void Update () {
        if (oculusCamera.gameObject.activeInHierarchy)
        {
            this.transform.rotation = oculusCamera.transform.rotation;
        }
        else
        {
            this.transform.rotation = normalCamera.transform.rotation;
        }
	}
}
