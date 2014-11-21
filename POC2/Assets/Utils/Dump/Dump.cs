using UnityEngine;
using System.Collections;

public class Dump : MonoBehaviour
{
    void Awake() { sTransform = transform; }
    private static Transform sTransform { get; set; }
    public static void me(Transform t) { t.parent = sTransform; }
    public static void me(GameObject go) { me(go.transform); }
    public static void me(MonoBehaviour mb) { me(mb.transform); }
    public static void me(Transform t, float lifeTime) { me(t); setLifeTime(t.gameObject, lifeTime); }
    public static void me(GameObject go, float lifeTime) { me(go.transform, lifeTime); }
    public static void me(MonoBehaviour mb, float lifeTime) { me(mb.transform, lifeTime); }
    public static void me(Transform t, float minLifeTime, float maxLifeTime) { me(t); setLifeTime(t.gameObject, minLifeTime, maxLifeTime); }
    public static void me(GameObject go, float minLifeTime, float maxLifeTime) { me(go.transform, minLifeTime, maxLifeTime); }
    public static void me(MonoBehaviour mb, float minLifeTime, float maxLifeTime) { me(mb.transform, minLifeTime, maxLifeTime); }

    private static void setLifeTime(GameObject go, float lifeTime)
    {
        go.AddComponent<DumpTimer>().SetLifetime(lifeTime);
    }

    private static void setLifeTime(GameObject go, float minLifeTime, float maxLifeTime)
    {
        go.AddComponent<DumpTimer>().SetLifetime(minLifeTime, maxLifeTime);
    }

    public static GameObject spawn(Transform parent, GameObject t, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject go = (GameObject)GameObject.Instantiate(t);
        go.transform.parent = parent;
        go.transform.rotation = rotation;
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        return go;
    }


    public static GameObject spawn(Transform parent, GameObject t, float lifeTime)
    {
        GameObject go = spawn(parent, t, t.transform.position, t.transform.rotation, t.transform.localScale);
        setLifeTime(go, lifeTime);
        return go;
    }
    public static GameObject spawn(Transform parent, GameObject t)
    {
        return spawn(parent, t, t.transform.position, t.transform.rotation, t.transform.localScale);
    }
    public static GameObject spawn(GameObject t)
    {
        return spawn(sTransform, t);
    }
    public static GameObject spawn(GameObject t, float lifeTime)
    {
        return spawn(sTransform, t, lifeTime);
    }


    public static GameObject spawn(Transform parent, GameObject t, Vector3 position, float lifeTime)
    {
        GameObject go = spawn(parent, t, position, t.transform.rotation, t.transform.localScale);
        setLifeTime(go, lifeTime);
        return go;
    }
    public static GameObject spawn(Transform parent, GameObject t, Vector3 position)
    {
        return spawn(parent, t, position, t.transform.rotation, t.transform.localScale);
    }
    public static GameObject spawn(GameObject t, Vector3 position)
    {
        return spawn(sTransform, t, position);
    }
    public static GameObject spawn(GameObject t, Vector3 position, float lifeTime)
    {
        return spawn(sTransform, t, position, lifeTime);
    }


    public static GameObject spawn(Transform parent, GameObject t, Vector3 position, Vector3 scale, float lifeTime)
    {
        GameObject go = spawn(parent, t, position, t.transform.rotation, scale);
        setLifeTime(go, lifeTime);
        return go;
    }
    public static GameObject spawn(Transform parent, GameObject t, Vector3 position, Vector3 scale)
    {
        return spawn(parent, t, position, t.transform.rotation, scale);
    }
    public static GameObject spawn(GameObject t, Vector3 position, Vector3 scale)
    {
        return spawn(sTransform, t, position, scale);
    }
    public static GameObject spawn(GameObject t, Vector3 position, Vector3 scale, float lifeTime)
    {
        return spawn(sTransform, t, position, scale, lifeTime);
    }


    public static void empty() {
        for (int i = 0; i < sTransform.childCount; i++)
        {
            GameObject.Destroy(sTransform.GetChild(i).gameObject);
        }
    }
}