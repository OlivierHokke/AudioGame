using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public abstract class BaseState
{
    public abstract void Start(Story script);
    public abstract void Update(Story script);
    public abstract void End(Story script);
}
