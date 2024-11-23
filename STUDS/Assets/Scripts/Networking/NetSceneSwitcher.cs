using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class NetSceneSwitcher : NetworkBehaviour
{
    public string specificScene = "";

    private void Start()
    {
        if (gameObject.name == "NetPauseMenuPanel") // Objects with a network identity are set to active by Mirror.
        {
            gameObject.SetActive(false);
        }
    }

    public void CallHandleLeave()
    {
        SteamLobby.singleton.HandleLeave();
    }

    public void LoadSpecificScene()
    {
        FindObjectOfType<NetworkManager>().ServerChangeScene(specificScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
