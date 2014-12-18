using System;
using UnityEngine;

[Serializable]
public abstract class BaseControls
{
    public abstract void OnEnable();
    public abstract Vector3 GetMove();
    public abstract Quaternion GetRotation();
    public abstract void OnDisable();
}
