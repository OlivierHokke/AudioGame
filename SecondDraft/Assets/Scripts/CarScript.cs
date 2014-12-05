using UnityEngine;
using System.Collections;

public class CarScript : MonoBehaviour {

	private CarGeneratorScript generator;
	private float speed;

	public void Init(CarGeneratorScript g, float startingSpeed) {
		this.generator = g;
		this.speed = startingSpeed;
	}

	void FixedUpdate() {
		transform.position += transform.forward*((speed)*Time.deltaTime);
	}

	void OnTriggerExit(Collider other) {
		if (other == generator.carLiveArea.collider) {
			Destroy (gameObject);
		}
	}
}
