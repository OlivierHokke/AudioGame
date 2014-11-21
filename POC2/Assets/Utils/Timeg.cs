using System.Collections.Generic;
using UnityEngine;

public class Timeg
{
    public static float clamp01(float speed)
    {
        return Mathf.Min(1f, Time.deltaTime * speed);
    }
}