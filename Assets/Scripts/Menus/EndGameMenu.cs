/*****************************************************************************
// File Name :         EndGameMenu.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     February 12th, 2023
//
// Brief Description : This document controls the end game scene canvas menu
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject _winText;
    [SerializeField] private GameObject _loseText;

    private void Start()
    {
        SetWinText();
    }

    /// <summary>
    /// Enables win or loss text depending on results of game
    /// </summary>
    public void SetWinText()
    {
        _winText.SetActive(EndGame.wonGame);
        _loseText.SetActive(!EndGame.wonGame);
    }

    /// <summary>
    /// Restarts the game
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Reeturns to main menu
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Start");
    }

    /// <summary>
    /// Qutis game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
