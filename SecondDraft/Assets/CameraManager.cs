using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;
    void Awake()
    {
        instance = this;
    }

    public Camera combinedCamera;
    public GameObject oculusRiftRoot;
    public Camera oculusRiftCamera;
    public GameObject normalRoot;
    public Camera normalCamera;
    public bool oculusRiftEnabled;
    public bool oculusRiftConnected;
    public bool useMonoCamera;
    public Camera current;

    public static Vector3 GetCameraForwardVector() { return instance.getCameraForwardVector(); }
    private Vector3 getCameraForwardVector()
    {
        return current.transform.TransformDirection(Vector3.forward);
    }

    public static Vector3 GetCameraRightVector() { return instance.getCameraRightVector(); }
    private Vector3 getCameraRightVector()
    {
        return current.transform.TransformDirection(Vector3.right);
    }

    public static void UseOculusRiftCameras() { instance.useOculusRiftCameras(); }
    private void useOculusRiftCameras()
    {
        oculusRiftEnabled = true;
        HandleSettings();
    }

    public static void UseNormalCamera() { instance.useNormalCamera(); }
    private void useNormalCamera()
    {
        oculusRiftEnabled = false;
        HandleSettings();
    }

    private void HandleSettings()
    {
        if (oculusRiftEnabled)
        {
            oculusRiftRoot.SetActive(true);
            normalRoot.SetActive(false);
            if (useMonoCamera)
            {
                current = combinedCamera;
                //combinedCamera.gameObject.SetActive(true);
                combinedCamera.depth = 2;
            }
            else
            {
                current = oculusRiftCamera;
                //combinedCamera.gameObject.SetActive(false);
                combinedCamera.depth = -2;
            }
        }
        else
        {
            oculusRiftRoot.SetActive(false);
            normalRoot.SetActive(true);
            combinedCamera.gameObject.SetActive(true);
            current = combinedCamera;
        }

        PlayerCompass.SetSource(current.gameObject);
    }

    public void ToggleFullOculusRift()
    {
        oculusRiftEnabled = !oculusRiftEnabled;
        useMonoCamera = !oculusRiftEnabled;
        HandleSettings();
    }

    public void ToggleMonoCamera()
    {
        useMonoCamera = !useMonoCamera;
        HandleSettings();
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

    void Update()
    {
        oculusRiftConnected = OVRDevice.IsHMDPresent();
    }
}
