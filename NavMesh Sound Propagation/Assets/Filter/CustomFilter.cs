using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CustomFilter : BaseFilter
{
    private float[] filter;
    private int center;

    public CustomFilter(FilterSettings settings, float[] filter, int center) : base(settings)
    {
        this.filter = filter;
        this.center = center;
    }

    public override float Apply(float[] data, int startIndex)
    {
        // add offset due to center
        int f_index = filter.Length - 1;
        int d_index = startIndex + filter.Length - center - settings.delay;
        int d_stop = d_index - filter.Length;

        d_index = Mathf.Min(d_index, data.Length - 1);
        d_stop = Mathf.Max(d_stop, 0);

        float processed = 0f;
        while (d_index > d_stop)
            processed += filter[f_index--] * data[d_index--];

        return processed * settings.normalize * settings.volume;
    }
}
