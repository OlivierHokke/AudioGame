using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ElevatorState : BaseState
{
	public GameObject targetPlayerPosition;
    public GameObject targetLucyPosition;
	public AudioClip elevatorSound;
	public BaseState NextState; // TODO pass this to ancestor class?

	private AudioPlayer audioPlayer;

	public override void Start(Story script) {
        script.Player.GetComponent<PlayerController>().LockMovement = true;
		AudioObject ao = new AudioObject(script.Lucy, elevatorSound);
		audioPlayer = AudioManager.PlayAudio(ao);
	}

	public override void Update(Story script) {
		if (audioPlayer.finished) {
			script.LoadState(NextState);
		}
	}

	public override void End(Story script) {
        script.Player.GetComponent<PlayerController>().LockMovement = false;
		script.Player.transform.position = targetPlayerPosition.transform.position;
		script.Player.transform.rotation = targetPlayerPosition.transform.rotation;
        script.Lucy.transform.position = targetLucyPosition.transform.position;
        script.Lucy.transform.rotation = targetLucyPosition.transform.rotation;
	}
}
