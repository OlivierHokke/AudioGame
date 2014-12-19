﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {


	public AudioClip growlSound;
	private AudioPlayer growlPlayer;

	private bool onFire = false;

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

	public void useGrowl() {
		AudioObject ao = new AudioObject(this.gameObject, growlSound);
		growlPlayer = AudioManager.PlayAudio(ao);
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

	public void setOnFire(bool state) {
		onFire = state;
	}

	public bool isOnFire() {
		return onFire;
	}
}
