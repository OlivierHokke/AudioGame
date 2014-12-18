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


    public event EventHandler<TriggerEventArgs> TriggerEntered;
    public event EventHandler<TriggerEventArgs> TriggerExit;

    public bool LockMovement = false;
	
	// Update is called once per frame
	void Update ()
    {
        BaseControls c = ControlsManager.current;
        if (!LockMovement)
        transform.position += c.GetMove();
        transform.rotation *= c.GetRotation();
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

    void OnTriggerExit(Collider other)
    {
        if (TriggerExit != null)
            TriggerExit(this, new TriggerEventArgs(other));
    }

}
