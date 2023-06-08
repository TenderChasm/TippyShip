using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public LevelData CurrentLevel;
    public float CurrentVolume = 1;

    public LevelData Cave;
    public LevelData City;
    public LevelData DuskCity;

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("CurrentLevelNumber", CurrentLevel.ID);
        PlayerPrefs.SetFloat("CurrentVolume", CurrentVolume);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        int currentLevelNumber = PlayerPrefs.GetInt("CurrentLevelNumber", CurrentLevel.ID);

        CurrentVolume = PlayerPrefs.GetFloat("CurrentVolume", CurrentVolume);

        CurrentLevel = currentLevelNumber switch
        {
            0 => Cave,
            1 => City,
            2 => DuskCity
        };
    }
}
