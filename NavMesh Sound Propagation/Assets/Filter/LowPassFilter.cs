using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LowPassFilter : BaseFilter
{
    private float[] lowPassFilter = new float[] { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.1f, 0.09f, 0.08f, 0.07f, 0.06f, 0.05f, 0.04f, 0.03f, 0.02f, 0.01f };
    CustomFilter cf;

    public LowPassFilter(FilterSettings settings)
        : base(settings)
    {
        cf = new CustomFilter(settings, lowPassFilter, 9);
    }

    public override float Apply(float[] data, int startIndex)
    {
        return cf.Apply(data, startIndex);
    }
}
