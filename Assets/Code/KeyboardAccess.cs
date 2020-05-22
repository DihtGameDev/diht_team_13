using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAccess : MonoBehaviour
{
    private string m_DetectKeyForName;
    
    private static Dictionary<string, KeyCode> DefaultKeys = new Dictionary<string, KeyCode>
    {
        {"RestartKey", KeyCode.R},
        {"StopMoveKey", KeyCode.Space}
    };
    
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
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(m_DetectKeyForName))
        {
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    SetKey(m_DetectKeyForName, kcode);
                    m_DetectKeyForName = null;
                    return;
                }
            }
        }
    }
}
