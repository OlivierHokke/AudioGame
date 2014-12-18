using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;
    void Awake()
    {
        instance = this;
    }

    public bool oculusRiftEnabled;
    public GameObject oculusRiftRoot;
    public GameObject oculusRiftCamera;
    public GameObject normalRoot;
    public GameObject normalCamera;

    public static Vector3 GetCameraForwardVector() { return instance.getCameraForwardVector(); }
    private Vector3 getCameraForwardVector()
    {
        return (oculusRiftEnabled ? oculusRiftCamera : normalCamera).transform.TransformDirection(Vector3.forward);
    }

    public static Vector3 GetCameraRightVector() { return instance.getCameraRightVector(); }
    private Vector3 getCameraRightVector()
    {
        return (oculusRiftEnabled ? oculusRiftCamera : normalCamera).transform.TransformDirection(Vector3.right);
    }

    public static void UseOculusRiftCameras() { instance.useOculusRiftCameras(); }
    private void useOculusRiftCameras()
    {
        oculusRiftEnabled = true;
        if (!oculusRiftRoot.activeInHierarchy) oculusRiftRoot.SetActive(true);
        if (normalRoot.activeInHierarchy) normalRoot.SetActive(false);
        PlayerCompass.SetSource(oculusRiftCamera);
    }

    public static void UseNormalCamera() { instance.useNormalCamera(); }
    private void useNormalCamera()
    {
        oculusRiftEnabled = false;
        if (oculusRiftRoot.activeInHierarchy) oculusRiftRoot.SetActive(false);
        if (!normalRoot.activeInHierarchy) normalRoot.SetActive(true);
        PlayerCompass.SetSource(normalCamera);
    }

    public void ToggleOculus() 
    {
        if (oculusRiftEnabled) {
            UseNormalCamera();
        } else {
            UseOculusRiftCameras();
        }
    }

    void Start()
    {
        if (oculusRiftEnabled)
        {
            UseOculusRiftCameras();
        } 
        else
        {
            UseNormalCamera();
        }
    }
}
