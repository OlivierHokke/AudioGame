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
    public FixedDirectionControls fixedDirectionControls;
    public ControllerOption DefaultControls = ControllerOption.SingleAxisControls;

    public void SetControls(BaseControls controls)
    {
        if (current != null)
            current.OnDisable();
        current = controls;
        current.OnEnable();
    }

    void Start()
    {
        switch(DefaultControls)
        {
            case ControllerOption.FixedDirectionControls: SetControls(fixedDirectionControls); break;
            case ControllerOption.SingleAxisControls: SetControls(singleAxisControls); break;
        }
    }

    public enum ControllerOption
    {
        SingleAxisControls, FixedDirectionControls
    }
}
