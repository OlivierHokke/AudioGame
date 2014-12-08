using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Lucy flies to the target location and emits sounds to guide the player
/// </summary>
[Serializable]
public class SimpleFollowLucyState : LucyRingingBellState
{
    public GameObject TargetLocation;
    public BaseState NextState;
    public AudioClip SuccesSound;


    [Tooltip("Time it takes lucy to fly to the new location")]
    public float LucyAppearanceDelay = 5f;

    private float timeInState = 0f;
    private Vector3 lucyStartPosition;
    private Quaternion lucyStartRotation;



    public override void Start(Story script)
    {
        base.Start(script);

        timeInState = 0f;
        lucyStartPosition = script.Lucy.transform.position;
        lucyStartRotation = script.Lucy.transform.rotation;
    }

    public override void Update(Story script)
    {
        base.Update(script);

        timeInState += Time.deltaTime;
        float progress = timeInState / LucyAppearanceDelay;
        if (progress < 1)
        {
            script.Lucy.transform.position = this.lucyStartPosition + ((this.TargetLocation.transform.position - lucyStartPosition) * Ease.ioSinusoidal(progress));
            script.Lucy.transform.rotation = Quaternion.RotateTowards(this.lucyStartRotation, this.TargetLocation.transform.rotation, 
                Quaternion.Angle(this.lucyStartRotation, this.TargetLocation.transform.rotation) * Ease.ioSinusoidal(progress));
        }
        else
        {
            script.Lucy.transform.position = TargetLocation.transform.position;
            script.Lucy.transform.rotation = TargetLocation.transform.rotation;
        }

        //Vector3 distance = TargetLocation.transform.position - script.Player.transform.position;
        //// we arrived at the target location, thus load our next state
        //if (distance.magnitude < 2f)
        //{
        //    if (SuccesSound != null)
        //        AudioManager.PlayAudio(new AudioObject(TargetLocation, SuccesSound));
        //    script.LoadState(NextState);
        //}
    }

    public override void PlayerEnteredTrigger(Collider collider, Story script)
    {
        if (collider.gameObject == TargetLocation.gameObject)
        {
            if (SuccesSound != null)
                AudioManager.PlayAudio(new AudioObject(TargetLocation, SuccesSound));

            script.LoadState(NextState);
        }
        base.PlayerEnteredTrigger(collider, script);
    }

    public override void End(Story script)
    {
        base.End(script);
    }
}
