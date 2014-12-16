using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[ExecuteInEditMode]
public class CanvasResizer : MonoBehaviour {

    public Camera renderingCamera;

	// Update is called once per frame
	void Update () {
        (transform as RectTransform).sizeDelta = new Vector2(renderingCamera.pixelWidth, renderingCamera.pixelHeight);
	}
}
