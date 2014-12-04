using UnityEngine;

public class SpeechPlayer : AudioPlayer
{
    SpeechObject speech;

    public SpeechPlayer(SpeechObject speech)
        : base(speech.audio)
    {
        this.speech = speech;
        // show subtitles
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (finished)
        {
            // remove/deactivate subtitles?
        }
    }
}