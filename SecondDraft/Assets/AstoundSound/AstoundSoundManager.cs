using UnityEngine;
using System.Collections;

public class AstoundSoundManager : MonoBehaviour {
    public static AstoundSoundManager instance;
    void Awake()
    {
        instance = this;
    }

    [Tooltip("Changing this value won't matter. It is read only")]
    public bool astoundSoundEnabled = false;

    public void ToggleAstoundSound()
    {
        astoundSoundEnabled = !instance.astoundSoundEnabled;
        HandleListenerSettings();
        HandleSourceSettings();
    }

    public void EnableAstoundSound()
    {
        astoundSoundEnabled = true;
        HandleListenerSettings();
        HandleSourceSettings();
    }

    public void DisableAstoundSound()
    {
        astoundSoundEnabled = false;
        HandleListenerSettings();
        HandleSourceSettings();
    }
    
    public static void HandleAudioSource(GameObject gameObject)
    {
        if (gameObject.audio != null)
            HandleAudioSource(gameObject.audio);
    }

    public static void HandleAudioSource(AudioSource audio)
    {
        AstoundSoundRTIFilter filter = audio.GetComponent<AstoundSoundRTIFilter>();
        if (filter == null)
        {
            audio.gameObject.AddComponent<AstoundSoundRTIFilter>();
            filter = audio.GetComponent<AstoundSoundRTIFilter>();
        }
        filter.enabled = IsAstoundSoundActive();
    }

    public static void HandleListenerSettings()
    {
        bool enabled = IsAstoundSoundActive();
        AstoundSoundRTIListener[] listeners = FindObjectsOfType<AstoundSoundRTIListener>();
        foreach (var l in listeners)
        {
            // only possibly enable the listener that is currently on an active audio listener
            if (l.GetComponent<AudioListener>().enabled)
                l.enabled = enabled;
            else
                l.enabled = false;
        }
    }

    public static void HandleSourceSettings()
    {
        bool enabled = IsAstoundSoundActive();
        AstoundSoundRTIFilter[] filters = FindObjectsOfType<AstoundSoundRTIFilter>();
        foreach (var f in filters)
        {
            f.enabled = enabled;
        }
    }

    public static bool IsAstoundSoundActive()
    {
        return instance.astoundSoundEnabled;
    }
}
