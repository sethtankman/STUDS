using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamHandler : MonoBehaviour
{
    public GameObject steamLobbyHandler;

    public void spawnLobbyHandler()
    {
        if(!SteamLobby.singleton)
            Instantiate(steamLobbyHandler).GetComponent<SteamLobby>();
    }

    public void callGetLobbies()
    {
        SteamLobby.singleton.GetLobbyList();
        SteamLobby.singleton.fetchLobbies = true;
    }

    public void stopGettingLobbies()
    {
        SteamLobby.singleton.fetchLobbies = false;
    }

    public void callHostGame()
    {
        SteamLobby.singleton.fetchLobbies = false;
        SteamLobby.singleton.HostLobby();
    }

    public static void callJoinRoomAsClient()
    {
        SteamLobby.singleton.fetchLobbies = false;
        SteamLobby.singleton.JoinRoomAsClient();
    }
}
