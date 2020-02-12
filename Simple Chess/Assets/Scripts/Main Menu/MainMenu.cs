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
        SceneManager.LoadScene("AI Game");
    }

    public void BlackButton()
    {
        UIEngine.playerSelectedWhite = false;
        SceneManager.LoadScene("AI Game");
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
