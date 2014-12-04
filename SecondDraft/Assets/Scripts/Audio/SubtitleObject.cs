using UnityEngine;

public class SubtitleObject
{
    public string text;
    public string name;
    public Sprite face;

    public SubtitleObject(Sprite face, string name, string text)
    {
        this.text = text;
        this.name = name;
        this.face = face;
    }
}
