using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public struct PositionRotation
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public PositionRotation(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }
}
