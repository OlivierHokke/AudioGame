﻿using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    void Awake()
    {
        instance = this;
    }

    public static int activeSettingType = 0;
    public GameObject settingsBackground;
    public GameObject settingsPanel;
    public float inactiveAlpha = 0.4f;
    public bool pressedUp = false;
    public bool pressedDown = false;

    public bool IsSettingsShown()
    {
        return settingsBackground.activeInHierarchy;
    }

    public void ToggleSettings()
    {
        settingsBackground.SetActive(!settingsBackground.activeInHierarchy);
    }

    public void NextSettingType()
    {
        activeSettingType++;
        activeSettingType = Mathf.Clamp(activeSettingType, 0, settingsPanel.transform.childCount - 1);
    }

    public void PreviousSettingType()
    {
        activeSettingType--;
        activeSettingType = Mathf.Clamp(activeSettingType, 0, settingsPanel.transform.childCount - 1);
    }

    void Update()
    {
        float changeSetting = Input.GetAxisRaw("Vertical");
        if (changeSetting < -0.9f) pressedDown = true;
        if (changeSetting > 0.9f) pressedUp = true;
        if (changeSetting > -0.1f && changeSetting < 0.1f)
        {
            if (pressedDown) NextSettingType();
            if (pressedUp) PreviousSettingType();
            pressedDown = false;
            pressedUp = false;
        }
    }
}
