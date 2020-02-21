using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI, selectScreenUI, optionsUI, difficultyUI;

    public void Start()
    {
        mainMenuUI.SetActive(true);
        selectScreenUI.SetActive(false);
        optionsUI.SetActive(false);
        difficultyUI.SetActive(false);
    }

    public void PlayerVsPlayer()
    {
        UIEnginePVP.AIGame = false;
        SceneManager.LoadScene("Player vs Player");
    }

    public void PlayAgainstAI()
    {
        mainMenuUI.SetActive(false);
        selectScreenUI.SetActive(true);
    }

    public void WhiteButton()
    {
        UIEngine.playerSelectedWhite = true;
        UIEnginePVP.AIGame = true;
        selectScreenUI.SetActive(false);
        difficultyUI.SetActive(true);
    }

    public void BlackButton()
    {
        UIEngine.playerSelectedWhite = false;
        UIEnginePVP.AIGame = true;
        selectScreenUI.SetActive(false);
        difficultyUI.SetActive(true);
    }

    public void Options()
    {
        mainMenuUI.SetActive(false);
        selectScreenUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void Back()
    {
        selectScreenUI.SetActive(false);
        optionsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void BackToColourSelection()
    {
        difficultyUI.SetActive(false);
        selectScreenUI.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    #region Stockfish Difficulties
    public void VeryEasy()
    {
        Stockfish.skillLevelValue = 1;
        SceneManager.LoadScene("Player vs Player");
    }

    public void Easy()
    {
        Stockfish.skillLevelValue = 2;
        SceneManager.LoadScene("Player vs Player");
    }
    public void Medium()
    {
        Stockfish.skillLevelValue = 4;
        SceneManager.LoadScene("Player vs Player");
    }

    public void Hard()
    {
        Stockfish.skillLevelValue = 6;
        SceneManager.LoadScene("Player vs Player");
    }

    public void VeryHard()
    {
        Stockfish.skillLevelValue = 8;
        SceneManager.LoadScene("Player vs Player");
    }

    public void Extreme()
    {
        Stockfish.skillLevelValue = 10;
        SceneManager.LoadScene("Player vs Player");
    }

    public void Impossible()
    {
        Stockfish.skillLevelValue = 20;
        SceneManager.LoadScene("Player vs Player");
    }

    #endregion
}
