/*****************************************************************************
// File Name :         SceneLoader.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     February 12th, 2023
//
// Brief Description : This document controls the scene loading.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _howToPlayPanel;
    [SerializeField] private GameObject _creditsPanel;

    private GameObject _currentPanel;

    private void Start()
    {
        _currentPanel = _mainMenuPanel;
    }

    /// <summary>
    /// Opens How To Play panel
    /// </summary>
    public void OpenHowToPlay()
    {
        SetCurrentPanel(_howToPlayPanel);
    }

    /// <summary>
    /// Opens Credits panel
    /// </summary>
    public void OpenCredits()
    {
        SetCurrentPanel(_creditsPanel);
    }

    /// <summary>
    /// Activates the main menu panel
    /// </summary>
    public void BackToMenu()
    {
        SetCurrentPanel(_mainMenuPanel);
    }

    /// <summary>
    /// Closes current panel and opens new panel
    /// </summary>
    /// <param name="visible"></param>
    /// <param name="newPanel"></param>
    private void SetCurrentPanel(GameObject newPanel)
    {
        _currentPanel.SetActive(false);
        _currentPanel = newPanel;
        _currentPanel.SetActive(true);
    }

    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="scene"></param>
    public void SceneLoad(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
