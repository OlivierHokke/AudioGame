using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public abstract class BaseState
{
    public virtual void Start(Story script) { }
    public virtual void Update(Story script) { }
    public virtual void End(Story script) { }
}
