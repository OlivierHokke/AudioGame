using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Lucy flies to the target location and emits sounds to guide the player
/// </summary>
[Serializable]
public class SimpleFollowLucyState : BaseState
{
    public GameObject TargetLocation;
    public BaseState NextState;

    [Tooltip("Time it takes lucy to fly to the new location")]
    public float LucyAppearanceDelay = 5f;

    private float timeInState = 0f;
    private Vector3 lucyStartPosition;
    private Quaternion lucyStartRotation;

    public override void Start(Story script)
    {
        timeInState = 0f;
        lucyStartPosition = script.Lucy.transform.position;
        lucyStartRotation = script.Lucy.transform.rotation;
    }

    public override void Update(Story script)
    {
        timeInState += Time.deltaTime;
        float progress = timeInState / LucyAppearanceDelay;
        // TODO: Add Oliviers easing
        script.Lucy.transform.position = this.lucyStartPosition + ((this.lucyStartPosition - script.Lucy.transform.position) * Ease.ioSinusoidal(progress));

        Vector3 distance = TargetLocation.transform.position - script.Player.transform.position;

        // we arrived at the target location, thus load our next state
        if (distance.magnitude < 2f)
        {
            script.LoadState(NextState);
        }
    }

    public override void End(Story script)
    {
        throw new NotImplementedException();
    }
}
