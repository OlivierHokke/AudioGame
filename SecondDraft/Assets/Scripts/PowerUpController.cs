using UnityEngine;
using System.Collections;

public class PowerUpController : SpellController {

	protected override void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Debug.LogWarning ("Give it power!");
		}
	}
}
