using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardAccess : Controller
{
    public static bool IsSelecting =>
        !string.IsNullOrEmpty(m_DetectKeyForName);
    private static string m_DetectKeyForName;

    public TMP_Text SettingsBackKey;
    public TMP_Text SettingsRestartKey;
    public TMP_Text SettingsStopMoveKey;
    
    
    private static Dictionary<string, KeyCode> DefaultKeys = new Dictionary<string, KeyCode>
    {
        {"BackKey", KeyCode.Escape},
        {"RestartKey", KeyCode.R},
        {"StopMoveKey", KeyCode.Space}
    };
    
    public static KeyCode BackKey
        => GetKey("BackKey");
    
    public static KeyCode RestartKey
        => GetKey("RestartKey");
    
    public static KeyCode StopMoveKey
        => GetKey("StopMoveKey");

    private static KeyCode GetKey(string name)
    {
        var savedKey= PlayerPrefs.GetInt(name, -1);
        if (savedKey == -1)
            return DefaultKeys[name];
        return (KeyCode) savedKey;
    }

    private void SetKey(string name, KeyCode code)
    {
        PlayerPrefs.SetInt(name, (int) code);
        PlayerPrefs.Save();
    }
    

    public void OnKeyChangeRequest(string name)
    {
        m_DetectKeyForName = name;
        SetKey(name, KeyCode.Minus);
        UpdateSettingsKeysText();
    }

    private void Start()
    {
        WorkingGameMode = GameMode.Settings;
        UpdateSettingsKeysText();
    }

    public override void Tick()
    {
        if (!string.IsNullOrEmpty(m_DetectKeyForName))
        {
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if(kcode == KeyCode.Mouse0)
                    continue;
                if (Input.GetKeyDown(kcode))
                {
                    SetKey(m_DetectKeyForName, kcode);
                    m_DetectKeyForName = null;
                    UpdateSettingsKeysText();
                    return;
                }
            }
        }
    }

    private void UpdateSettingsKeysText()
    {
        SettingsBackKey.text = BackKey.ToString("G");
        SettingsRestartKey.text = RestartKey.ToString("G");
        SettingsStopMoveKey.text = StopMoveKey.ToString("G");
    }
}
