﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{
    public string specificScene = "";

    /// <summary>
    /// Called from buttons that return you to the main menu.
    /// </summary>
    public void CallHandleLeave()
    {
        Debug.Log("CallHandleLeave");
        SteamLobby.singleton.HandleLeave();
    }

    public void LoadSpecificScene()
    {
        SceneManager.LoadScene(specificScene);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("GarageScene");
    }

    public void PreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
