using UnityEngine;
using System.Collections;

public class FireballController : SpellController {
	
	protected override void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Debug.LogWarning ("Burn it!");
		}
	}
}
