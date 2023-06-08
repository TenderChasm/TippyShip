using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgamePopup : MonoBehaviour
{
    public Text ScoreField;
    public Menu Menu;

    public void SetScoreOnFinalScreen(int score)
    {
        ScoreField.text = "x" + score.ToString();
    }

    public void OnMenuButtonClick()
    {
        GameManager.Hr.LevelManager.ClearLevel();
        GameManager.Hr.LevelManager.InitializeLevel();

        Menu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ProcessBackButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnMenuButtonClick();
    }

    void Update()
    {
        ProcessBackButton();
    }
}
