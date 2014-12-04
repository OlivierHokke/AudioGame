using UnityEngine;

public class SpeechObject
{
    public SubtitleObject subtitles;
    public AudioObject audio;

    public SpeechObject(AudioObject audio, SubtitleObject subtitles)
    {
        this.audio = audio;
        this.subtitles = subtitles;
    }
}