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


    public AudioPlayer(AudioObject audio)
    {
        this.finished = false;
        this.removable = false;
        this.audio = audio;

        // create audio source with clip
        audioGO = (GameObject)GameObject.Instantiate(AudioManager.instance.audioSourcePrefab);
        audioGO.transform.parent = audio.parent.transform;
        audioGO.transform.localPosition = Vector3.zero;
        audioAS = audioGO.GetComponent<AudioSource>();
        audioAS.clip = audio.clip;
        audioAS.volume = audio.volume;
        audioAS.PlayDelayed(audio.delay);
    }

    public virtual void Update(float deltaTime)
    {
        if (finished) return;

        if (!audioAS.isPlaying)
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