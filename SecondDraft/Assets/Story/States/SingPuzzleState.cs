using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class SingPuzzleState : BaseState
{
    public BaseState NextState;

	public GameObject Monster1;
	public GameObject Monster2;
	public GameObject Monster3;
	public GameObject Ruby;


	// the sound to play, can be attached in unity editor
	public AudioClip playableSoundMonster1;
	public AudioClip playableSoundMonster2;
	public AudioClip playableSoundMonster3;
	public AudioClip playableSoundRuby;
	public AudioClip playableSoundLucyWarning;

	// the object that is returned that we listen to, to check if sound is played
	private AudioPlayer audioPlayer;


	public override void Start(Story script)
	{
		base.Start(script);


	}

	public override void Update(Story script)
	{
		base.Update(script);
		// If Alex is close to a monster, scream from the monster and Lucy is warning you to come back !
		// If Alex is close to Rubygo to end
		var distanceMonster1 = script.Lucy.transform.position - Monster1.transform.position;
		var distanceMonster2 = script.Lucy.transform.position - Monster2.transform.position;
		var distanceMonster3 = script.Lucy.transform.position - Monster3.transform.position;
		var distanceRuby = script.Lucy.transform.position - Ruby.transform.position;


		AudioObject ao = new AudioObject(script.Lucy.gameObject, playableSoundLucyWarning);
		AudioObject ao1 = new AudioObject(Monster1, playableSoundMonster1);
		AudioObject ao2 = new AudioObject(Monster2, playableSoundMonster2);
		AudioObject ao3 = new AudioObject(Monster3, playableSoundMonster3);
		AudioObject ao4 = new AudioObject(Ruby, playableSoundRuby);
		AudioManager.PlayAudio(ao1);
		AudioManager.PlayAudio(ao2);
		AudioManager.PlayAudio(ao3);
		AudioManager.PlayAudio(ao4);

		
		if (distanceMonster1.magnitude < 5f) {
			AudioManager.PlayAudio(ao);
		} else if (distanceMonster2.magnitude < 5f) {
			AudioManager.PlayAudio(ao);
		} else if (distanceMonster3.magnitude < 5f) {
			AudioManager.PlayAudio(ao);
		} else if (distanceRuby.magnitude < 5f) {
			script.LoadState(NextState);
		}

	}
	public override void End(Story script)
	{
		base.End(script);
	}



}
