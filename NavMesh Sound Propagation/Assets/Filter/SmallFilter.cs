using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SmallFilter : BaseFilter
{
    private float[] smallFilter = new float[] { 0.25f, 0.5f, 0.25f };
    CustomFilter cf;

    public SmallFilter(FilterSettings settings)
        : base(settings)
    {
        cf = new CustomFilter(settings, smallFilter, 1);
    }

    public override float Apply(float[] data, int startIndex)
    {
        return cf.Apply(data, startIndex);
    }
}
