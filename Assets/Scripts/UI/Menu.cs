using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Settings Settings;
    
    public void OnSettingsButton()
    {
        Settings.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnPlayButton()
    {
        GameManager.Hr.LevelManager.ClearLevel();
        GameManager.Hr.LevelManager.InitializeLevel();
        GameManager.Hr.LevelManager.StartLevel();
        gameObject.SetActive(false);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
