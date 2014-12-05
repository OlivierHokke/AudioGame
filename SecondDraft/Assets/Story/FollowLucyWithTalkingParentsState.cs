using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class FollowLucyWithTalkingParentsState : SimpleFollowLucyState
{
    public AudioClip MotherSound;
    public AudioClip FatherSound;
    public AudioClip BrotherSound;
	public AudioClip LucySound;

    public GameObject Mother;
    public GameObject Father;
    public GameObject Brother;

    public float MaxRandomDelay = 0.5f;

    private AudioPlayer motherPlayer;
    private AudioPlayer fatherPlayer;
    private AudioPlayer brotherPlayer;
	private AudioPlayer LucyPlayer;


    public override void Start(Story script)
    {
        AudioObject m = new AudioObject(script.Lucy, MotherSound, 1, Randomg.Range(0, MaxRandomDelay));
        AudioObject f = new AudioObject(script.Lucy, FatherSound, 1, Randomg.Range(0, MaxRandomDelay));
        AudioObject b = new AudioObject(script.Lucy, BrotherSound, 1, Randomg.Range(0, MaxRandomDelay));
		AudioObject l = new AudioObject(script.Lucy, LucySound, 1, Randomg.Range(0, MaxRandomDelay));
		AudioObject lb = new AudioObject(script.Lucy, LucyBell, 1, Randomg.Range(0, MaxRandomDelay));

        motherPlayer = PlayWithRandomDelay(Mother, MotherSound);
        fatherPlayer = PlayWithRandomDelay(Father, FatherSound);
        brotherPlayer = PlayWithRandomDelay(Brother, BrotherSound);
		LucyPlayer = PlayWithRandomDelay (Lucy, LucySound);
		//LucyBellPlayer = PlayWithRandomDelay (Lucy, LucyBell);


        base.Start(script);
    }

    private AudioPlayer PlayWithRandomDelay(GameObject source, AudioClip clip)
    {
        AudioObject ao = new AudioObject(source, clip, 1, Randomg.Range(0, MaxRandomDelay));
        return AudioManager.PlayAudio(ao);
    }

    public override void Update(Story script)
    {

        if (motherPlayer.finished) motherPlayer = PlayWithRandomDelay(Mother, MotherSound);
        if (fatherPlayer.finished) fatherPlayer = PlayWithRandomDelay(Father, FatherSound);
        if (brotherPlayer.finished) brotherPlayer = PlayWithRandomDelay(Brother, BrotherSound);

		//LucyBellPlayer = PlayWithRandomDelay (Lucy, LucyBell);

	if (LucyBellPlayer!=null){
			LucyBellPlayer = PlayWithRandomDelay (Lucy, LucyBell);
		}

        base.Update(script);
    }

    public override void End(Story script)
    {
        // TODO: stop the players
        base.End(script);
    }
    
}