/*****************************************************************************
// File Name :         EndGame.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     February 12th, 2023
//
// Brief Description : This document controls the end of the game info
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public static bool wonGame;

    // Start is called before the first frame update
    void Start()
    {
        wonGame = false;
    }

    /// <summary>
    /// Loads end game scene
    /// </summary>
    public void LoadEndGame()
    {
        SceneManager.LoadScene("End Game");
    }
}
