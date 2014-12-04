using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ElevatorState : BaseState
{
	public GameObject targetPosition;
	public AudioClip elevatorSound;
	public BaseState NextState; // TODO pass this to ancestor class?

	private AudioPlayer audioPlayer;

	public override void Start(Story script) {
		AudioObject ao = new AudioObject(script.Lucy, elevatorSound);
		audioPlayer = AudioManager.PlayAudio(ao);
	}

	public override void Update(Story script) {
		if (audioPlayer.finished) {
			script.LoadState(NextState);
		}
	}

	public override void End(Story script) {
		script.Player.transform.position = targetPosition.transform.position;
		script.Player.transform.rotation = targetPosition.transform.rotation;
	}
}
