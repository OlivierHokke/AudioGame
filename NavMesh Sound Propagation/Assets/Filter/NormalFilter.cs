using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NormalFilter : BaseFilter
{
    public NormalFilter(FilterSettings settings)
        : base(settings)
    {

    }

    public override float Apply(float[] data, int startIndex)
    {
        return data[Mathf.Max(0, startIndex - settings.delay)] * settings.normalize * settings.volume;
    }
}
