using UnityEngine;
using System.Collections;

public class ToggleActive : MonoBehaviour {

    public GameObject toToggle;
    public void Toggle()
    {
        toToggle.SetActive(!toToggle.activeInHierarchy);
    }
}
