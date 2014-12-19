using UnityEngine;

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
    protected bool paused = false;
    protected float pausedAt = 0f;


    public AudioPlayer(AudioObject audio)
    {
        this.finished = false;
        this.removable = false;
        this.paused = false;
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

    public AudioClip GetAudioClip()
    {
        if (audioAS != null)
            return audioAS.clip;
        return null;
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

    public void SetVolume(float volume)
    {
        if (audioAS != null)
            audioAS.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        if (audioAS != null)
            audioAS.pitch = pitch;
    }

    public virtual void Update(float deltaTime)
    {
        if (finished) return;

        if (audio.pausable)
        {
            if (PauseManager.paused && !paused && audioAS != null)
            {
                pausedAt = audioAS.time;
                audioAS.Stop();
                paused = true;
            }

            if (!PauseManager.paused && paused && audioAS != null)
            {
                audioAS.Play();
                audioAS.time = pausedAt;
                paused = false;
            }
        }

        if (audioAS == null || (audioAS.time > audioAS.clip.length - 0.04f))
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