using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void Empty(this MonoBehaviour obj) 
    {
        while (obj.transform.childCount > 0)
        {
            GameObject.Destroy(obj.transform.GetChild(0));
        }
    }
}