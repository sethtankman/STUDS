using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMenuActions : MonoBehaviour
{
    public static NetworkMenuActions instance;
    public Text logText;
    public StudsNetworkManager manager;
    public GameObject RoomButtonPrefab;
    public GameObject ContentPanel;
    public CSteamID SelectedRoomId;

    public List<GameObject> listOfLobbyListItems = new List<GameObject>();

    public void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    public void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    public void LogCallback(string logString, string stackTrace, LogType type)
    {
        logText.text = logString;
    }

    public void HostRoom()
    {
        if (!manager)
            manager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
        manager.StartHost();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                string roomName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name");
                Debug.Log($"Lobby {i} :: {roomName} number of players: {SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID).ToString()} max players: {SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID).ToString()}");
                if (roomName == null || roomName.Length == 0)
                    continue;
                /*if (didPlayerSearchForLobbies)
                {
                    Debug.Log("OnGetLobbyInfo: Player searched for lobbies");
                    if (SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name").ToLower().Contains(searchBox.text.ToLower()))
                    {
                        GameObject newLobbyListItem = Instantiate(RoomButtonPrefab) as GameObject;
                        LobbyListItem newLobbyListItemScript = newLobbyListItem.GetComponent<LobbyListItem>();

                        newLobbyListItemScript.lobbySteamId = (CSteamID)lobbyIDS[i].m_SteamID;
                        newLobbyListItemScript.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name");
                        newLobbyListItemScript.numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID);
                        newLobbyListItemScript.maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID);
                        newLobbyListItemScript.SetLobbyItemValues();


                        newLobbyListItem.transform.SetParent(ContentPanel.transform);
                        newLobbyListItem.transform.localScale = Vector3.one;

                        listOfLobbyListItems.Add(newLobbyListItem);
                    }
                }
                else
                {*/
                Debug.Log($"i={i}: lobbyIDS[i]={(CSteamID)lobbyIDS[i].m_SteamID}");
                CSteamID id = (CSteamID)lobbyIDS[i].m_SteamID;
                GameObject newLobbyListItem = Instantiate(RoomButtonPrefab);
                RoomButton newRoomButtonScript = newLobbyListItem.GetComponent<RoomButton>();

                newRoomButtonScript.SteamId = id;
                newRoomButtonScript.SetLobbyItemValues(roomName, 
                    SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID), 
                    SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID));


                newLobbyListItem.transform.SetParent(ContentPanel.transform);
                newLobbyListItem.transform.localScale = Vector3.one;

                listOfLobbyListItems.Add(newLobbyListItem);
                //}

                return;
            }
        }
        //if (didPlayerSearchForLobbies)
        //    didPlayerSearchForLobbies = false;
    }

    public void DestroyOldLobbyListItems()
    {
        Debug.Log("DestroyOldLobbyListItems");
        foreach (GameObject lobbyListItem in listOfLobbyListItems)
        {
            GameObject lobbyListItemToDestroy = lobbyListItem;
            Destroy(lobbyListItemToDestroy);
            lobbyListItemToDestroy = null;
        }
        listOfLobbyListItems.Clear();
    }
}
