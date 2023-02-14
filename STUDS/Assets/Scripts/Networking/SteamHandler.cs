using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamHandler : MonoBehaviour
{
    public GameObject steamLobbyHandler;
    public SteamLobby myHandler;

    public void spawnLobbyHandler()
    {
        myHandler = Instantiate(steamLobbyHandler).GetComponent<SteamLobby>();
    }

    public void callHostGame()
    {
        myHandler.HostLobby();
    }
}
