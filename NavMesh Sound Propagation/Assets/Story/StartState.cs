using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class StartState : BaseState
{
    public GameObject startLocation;
    public GameObject targetLocation;
    public float lucyAppearanceDelay = 5f;

    public override void Start(Story script)
    {
        script.player.transform.position = startLocation.transform.position;
        script.player.transform.rotation = startLocation.transform.rotation;
    }

    public override void Update(Story script)
    {
        Vector3 distance = targetLocation.transform.position - script.player.transform.position;

        // we arrived at the target location, thus load our next state
        if (distance.magnitude < 2f)
        {
            script.LoadState(script.someState);
        }
    }

    public override void End(Story script)
    {
        throw new NotImplementedException();
    }
}
