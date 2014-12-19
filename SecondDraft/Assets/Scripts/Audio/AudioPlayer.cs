﻿using UnityEngine;

public class AudioPlayer
{
    protected AudioObject audio;
    public bool finished
    {
        get;
        protected set;
    }
    public bool removable
    {
        get;
        protected set;
    }

    protected GameObject audioGO;
    protected AudioSource audioAS;
    protected AstoundSoundRTIFilter filterAS;


    public AudioPlayer(AudioObject audio)
    {
        this.finished = false;
        this.removable = false;
        this.audio = audio;

        // create audio source with clip
        audioGO = (GameObject)GameObject.Instantiate(AudioManager.instance.audioSourcePrefab);
        audioGO.transform.parent = audio.parent.transform;
        audioGO.transform.localPosition = Vector3.zero;

        SoundSystemManager.HandleAudioSource(audioGO);

        audioAS = audioGO.GetComponent<AudioSource>();
        audioAS.clip = audio.clip;
        audioAS.volume = audio.volume;
        audioAS.loop = audio.loop;
        audioAS.PlayDelayed(audio.delay);
    }

    public void StopPlaying()
    {
        if (audioAS != null)
            audioAS.Stop();
    }

    public void StartPlaying()
    {
        if (audioAS != null)
            audioAS.Play();
    }

    public void SetPitch(float pitch)
    {
        if (audioAS != null)
            audioAS.pitch = pitch;
    }

    public virtual void Update(float deltaTime)
    {
        if (finished) return;

        if (audioAS == null || !audioAS.isPlaying)
        {
            finished = true;
            removable = true;
        }
    }

    public virtual void OnRemove()
    {
        GameObject.Destroy(audioGO);
    }
}