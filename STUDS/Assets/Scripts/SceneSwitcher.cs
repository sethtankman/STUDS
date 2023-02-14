using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{
    public string specificScene = "";
    public GameObject lobbyHandler;

    public void CallHandleLeave()
    {
        lobbyHandler = GameObject.Find("SteamLobbyHandler(Clone)");
        if (lobbyHandler)
        {
            SteamLobby lobby = lobbyHandler.GetComponent<SteamLobby>();
            lobby.HandleLeave();
        } else
        {
            Debug.LogWarning("NetworkManager not located...");
        }
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
