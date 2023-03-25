using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamHandler : MonoBehaviour
{
    public GameObject steamLobbyHandler;
    public static SteamLobby myHandler;

    public void spawnLobbyHandler()
    {
        Debug.Log("Calling Spawn Lobby Handler");
        if(!SteamLobby.singleton)
            myHandler = Instantiate(steamLobbyHandler).GetComponent<SteamLobby>();
    }

    public void callGetLobbies()
    {
        Debug.Log("Calling Get Lobbies");
        myHandler.GetLobbyList();
    }

    public void callHostGame()
    {
        myHandler.HostLobby();
    }

    public static void callJoinRoomAsClient()
    {
        myHandler.JoinRoomAsClient();
    }
}
