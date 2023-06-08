using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera MainCamera;
    public LevelManager LevelManager;
    public SettingsManager SettingsManager;

    public AudioClip Music;

    public Bounds CameraWorldBounds { get; private set; }
    public float CameraAspect { get; private set; }

    public AudioSource Audio { get; set; }

    public float ReferenceCameraAspect { get; } = 9 / 18F;

    public static GameManager Hr { get; set; }

    void Awake()
    {
        if (Hr != null)
            DestroyImmediate(gameObject);
        else
            Hr = this;

        CalculateCameraVariables();
        SettingsManager.LoadPrefs();

        LevelManager.Data = SettingsManager.CurrentLevel;

        Audio = GetComponent<AudioSource>();
        Audio.volume = SettingsManager.CurrentVolume;

    }

    void Start()
    {
        Audio.clip = Music;
        Audio.Play();
    }

    private void CalculateCameraVariables()
    {
        CameraAspect = Screen.width / (float)Screen.height;

        float cameraHeight = MainCamera.orthographicSize * 2;
        CameraWorldBounds = new Bounds(MainCamera.transform.position, new Vector2(cameraHeight * CameraAspect, cameraHeight));
    }

}
