using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamHandler : MonoBehaviour
{
    public GameObject steamLobbyHandler;
    public static SteamLobby myHandler;

    public void spawnLobbyHandler()
    {
        if(!SteamLobby.singleton)
            myHandler = Instantiate(steamLobbyHandler).GetComponent<SteamLobby>();
    }

    public void callGetLobbies()
    {
        myHandler.GetLobbyList();
        myHandler.fetchLobbies = true;
    }

    public void stopGettingLobbies()
    {
        myHandler.fetchLobbies = false;
    }

    public void callHostGame()
    {
        myHandler.fetchLobbies = false;
        myHandler.HostLobby();
    }

    public static void callJoinRoomAsClient()
    {
        myHandler.fetchLobbies = false;
        myHandler.JoinRoomAsClient();
    }
}
