using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public LevelData Cave;
    public LevelData City;
    public LevelData DuskCity;

    public Button CaveButton;
    public Button CityButton;
    public Button DuskCityButton;

    public Menu Menu;

    public Slider Slider;

    private bool levelChanged;

    void OnEnable()
    {
        levelChanged = false;

        if (GameManager.Hr.SettingsManager.CurrentLevel == Cave)
            CaveButton.Select();
        else if (GameManager.Hr.SettingsManager.CurrentLevel == City)
            CityButton.Select();
        else if (GameManager.Hr.SettingsManager.CurrentLevel == DuskCity)
            DuskCityButton.Select();

        Slider.value = GameManager.Hr.SettingsManager.CurrentVolume;
    }

    private void ProcessBackButton()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (levelChanged)
            {
                GameManager.Hr.LevelManager.ClearLevel();
                GameManager.Hr.LevelManager.InitializeLevel();
            }

            GameManager.Hr.SettingsManager.SavePrefs();

            Menu.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnLevelButtonClick(LevelData data)
    {
        if (GameManager.Hr.SettingsManager.CurrentLevel != data)
        {
            GameManager.Hr.SettingsManager.CurrentLevel = data;

            levelChanged = true;
        }
    }

    public void OnVolumeChange(float volume)
    {
        GameManager.Hr.SettingsManager.CurrentVolume = volume;
        GameManager.Hr.Audio.volume = volume;
    }

    void Update()
    {
        ProcessBackButton();
    }
}
