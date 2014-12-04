using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class LucyExplainingState : BaseState
{
    public override void Start(Story script)
    { 
        // TODO: Start playing lucy talking
    }

    public override void Update(Story script)
    {
        // TODO: If Lucy's finished talking, goto next state
        if (true)
        {
            script.LoadState(script.InitialMove);
        }
    }

    public override void End(Story script)
    { }
}
