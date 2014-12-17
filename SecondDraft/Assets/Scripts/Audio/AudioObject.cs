using UnityEngine;

public class AudioObject
{
    public float delay;
    public float volume;
    public bool loop;

    public AudioClip clip;
    public GameObject parent;

    public AudioObject(GameObject parent, AudioClip clip, float volume = 1f, float delay = 0f, bool loop = false)
    {
        this.volume = volume;
        this.delay = delay;
        this.parent = parent;
        this.clip = clip;
        this.loop = loop;
    }
}