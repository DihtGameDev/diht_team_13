using System;
using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public enum GameMode
{
    Menu = 0,
    Pause,
    Settings,
    Default
}

public class Game : MonoBehaviour
{
    public static GameMode CurrentGameMode { get; private set; } = GameMode.Menu;

    public GameObject Menu;
    public GameObject Settings;
    
    private LevelController m_LevelController;
    
    void Start()
    {
        m_LevelController = FindObjectOfType<LevelController>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyboardAccess.BackKey) && !KeyboardAccess.IsSelecting)
        {
            if (CurrentGameMode == GameMode.Settings || CurrentGameMode == GameMode.Default)
            {
                CurrentGameMode = GameMode.Menu;
                Settings.SetActive(false);
                Menu.SetActive(true);
            }
        }
    }
    
    public void OnPlay()
    {
        m_LevelController.ResetLevel();
        CurrentGameMode = GameMode.Default;
    }
    
    public void OnSettings()
    {
        Settings.SetActive(true);
        CurrentGameMode = GameMode.Settings;
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
