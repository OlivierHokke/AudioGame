using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class LucyExplainingState : BaseState
{
    public bool Skip = false;

    // the sound to play, can be attached in unity editor
    public AudioClip playableSound;

    // the object that is returned that we listen to, to check if sound is played
    private AudioPlayer audioPlayer;

    public override void Start(Story script)
    {
        if (!Skip)
        {
            AudioObject ao = new AudioObject(script.Lucy, playableSound);
            audioPlayer = AudioManager.PlayAudio(ao);
        }

        // set the player's compass to target lucy
        PlayerCompass.SetTarget(script.Lucy);
    }

    public override void Update(Story script)
    {
        // wait untill sound is finished, then continue
        if (Skip || audioPlayer.finished)
        {
            script.LoadState(script.InitialMove);
        }
    }

    public override void End(Story script)
    { }
}
