using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI, selectScreenUI;

    public void Start()
    {
        mainMenuUI.SetActive(true);
        selectScreenUI.SetActive(false);
    }

    public void Play()
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
        SceneManager.LoadScene("Player vs Player");
    }

    public void BlackButton()
    {
        UIEngine.playerSelectedWhite = false;
        UIEnginePVP.AIGame = true;
        SceneManager.LoadScene("Player vs Player");
    }


    public void Options()
    {

    }

    public void Back()
    {
        selectScreenUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void Exit()
    {

    }
}
