using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BaseFilter
{
    public FilterSettings settings;

    public BaseFilter(FilterSettings settings)
    {
        this.settings = settings;
    }

    public abstract float Apply(float[] data, int startIndex);
}

public class FilterSettings
{
    public int delay;
    public float normalize;
    public float volume;

    public FilterSettings(float volume, float normalize, int delay)
    {
        this.delay = delay;
        this.normalize = normalize;
        this.volume = volume;
    }
}