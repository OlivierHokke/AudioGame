using UnityEngine;
using System.Collections;
using System;

[Serializable]
public abstract class SpellController : MonoBehaviour
{
	private MallumController Mallum;
	public float initialSpeed, speed;
	
	public virtual void Init(MallumController m, float startingSpeed) {
		this.Mallum = m;
		this.initialSpeed = startingSpeed;
		this.speed = initialSpeed;
	}

	void FixedUpdate() {
		transform.position += transform.forward*((speed)*Time.deltaTime);
	}
	
	protected abstract void OnTriggerEnter (Collider other);
	
	void OnTriggerExit(Collider other) {
		if (other == Mallum.fireballLiveArea.collider) {
			Debug.Log ("Destroyed");
			Destroy (gameObject);
		}
	}
}

