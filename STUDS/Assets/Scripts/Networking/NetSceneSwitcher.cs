using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class NetSceneSwitcher : NetworkBehaviour
{
    public string specificScene = "";

    public void CallHandleLeave()
    {
        Debug.Log("CallHandleLeave");
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
