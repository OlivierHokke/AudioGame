using UnityEngine;
using System.Collections;

public class ControlsManager : MonoBehaviour {
    public static ControlsManager instance;
    void Awake()
    {
        instance = this;
    }

    public static BaseControls current;
    public SingleAxisControls singleAxisControls;

    public void SetControls(BaseControls controls)
    {
        if (current != null)
            current.OnDisable();
        current = controls;
        current.OnEnable();
    }

    void Start()
    {
        SetControls(singleAxisControls);
    }
}
