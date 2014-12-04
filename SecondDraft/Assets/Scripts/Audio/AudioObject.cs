using UnityEngine;

public class AudioObject
{
    public float delay;
    public float volume;

    public AudioClip clip;
    public GameObject parent;

    public AudioObject(GameObject parent, AudioClip clip, float volume = 1f, float delay = 0f)
    {
        this.volume = volume;
        this.delay = delay;
        this.parent = parent;
        this.clip = clip;
    }
}