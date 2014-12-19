using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;

public class SettingsButton : MonoBehaviour {

    public delegate void ChangedEventHandler();

    [Serializable]
    public class SettingObject {
        public string name;
        public string description;
        public AudioClip audio;
        public UnityEvent onSet;
    }

    [Header("Setting")]
    public string name;
    public AudioClip playOnActive;
    public List<SettingObject> settings;

    [Header("State")]
    public int currentSettingIndex = 0;
    private int previousSettingIndex = -1;

    [Header("References")]
    public SideButtonAnimator left;
    public SideButtonAnimator right;
    public Text title;
    public Text settingText;
    public Text descriptionText;

    private bool pressedLeft;
    private bool pressedRight;
	
	// Update is called once per frame
	void Update () 
    {
        float changeSetting = Input.GetAxisRaw("Horizontal");
        if (changeSetting < -0.9f) pressedLeft = true;
        if (changeSetting > 0.9f) pressedRight = true;
        if (changeSetting > -0.1f && changeSetting < 0.1f)
        {
            if (pressedLeft) Left();
            if (pressedRight) Right();
            pressedLeft = false;
            pressedRight = false;
        }

        if (previousSettingIndex != currentSettingIndex && settings.Count > 0)
        {
            SettingObject obj = settings[currentSettingIndex];
            title.text = name;
            settingText.text = obj.name;
            descriptionText.text = obj.description;
            if (obj.audio != null)
            {
                AudioObject ao = new AudioObject(gameObject, obj.audio, 1f);
                AudioManager.PlayAudio(ao);
            }
            left.inactive = !HasLeft();
            right.inactive = !HasRight();
            obj.onSet.Invoke();
            previousSettingIndex = currentSettingIndex;

        }
	}

    public void Left()
    {
        if (!HasLeft()) return;
        currentSettingIndex--;
        currentSettingIndex = Mathf.Clamp(currentSettingIndex, 0, settings.Count - 1);
    }

    public void Right()
    {
        if (!HasRight()) return;
        currentSettingIndex++;
        currentSettingIndex = Mathf.Clamp(currentSettingIndex, 0, settings.Count - 1);
    }

    public bool HasLeft()
    {
        return currentSettingIndex > 0;
    }

    public bool HasRight()
    {
        return currentSettingIndex < settings.Count - 1;
    }
}
