using Steamworks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteamLobby : MonoBehaviour
{

    private StudsNetworkManager netManager;
    private const string hostAddressKey = "HostAddress";

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyDataUpdate_t> LobbyListRequested;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;

    public List<CSteamID> lobbyIDS = new List<CSteamID>();
    public bool fetchLobbies;
    public ulong current_lobbyID;

    struct LobbyMetaData
    {
        public string m_Key;
        public string m_Value;
    }

    struct LobbyMembers
    {
        public CSteamID m_SteamID;
        public LobbyMetaData[] m_Data;
    }
    struct Lobby
    {
        public CSteamID m_SteamID;
        public CSteamID m_Owner;
        public LobbyMembers[] m_Members;
        public int m_MemberLimit;
        public LobbyMetaData[] m_Data;
    }

    public void Start()
    {
        netManager = GetComponent<StudsNetworkManager>();

        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam Manager not initialized");
            return;
        }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoin);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyListRequested = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
    }

    public void HostLobby()
    {
        Debug.Log("Trying to host lobby");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, netManager.maxConnections);
    }

    public void JoinRoomAsClient()
    {
        if (NetworkMenuActions.instance.SelectedRoomId.IsValid())
            SteamMatchmaking.JoinLobby(NetworkMenuActions.instance.SelectedRoomId);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"OnLobbyCreated abort: {callback.m_eResult}");
            return;
        }

        netManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            $"{SteamFriends.GetPersonaName().ToString()}'s lobby");
    }

    private void OnGameLobbyJoin(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        current_lobbyID = callback.m_ulSteamIDLobby;
        Debug.Log("OnLobbyEntered for lobby with id: " + current_lobbyID.ToString());
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            hostAddressKey);

        netManager.networkAddress = hostAddress;
        netManager.StartClient();
        lobbyIDS.Clear();

        if (GameObject.Find("OnlineMenu"))
        {
            if (NetworkMenuActions.instance.listOfLobbyListItems.Count > 0)
                NetworkMenuActions.instance.DestroyOldLobbyListItems();
        } else
        {
            Debug.LogWarning("OnlineMenu wasn't found and you were trying to delete from it.  just remove this if statement if this keeps happening.");
        }
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        if (NetworkMenuActions.instance.listOfLobbyListItems.Count > 0)
            NetworkMenuActions.instance.DestroyOldLobbyListItems();
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);

        }

    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        NetworkMenuActions.instance.DisplayLobbies(lobbyIDS, result);
    }

    public void GetLobbyList()
    {
        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        Debug.Log("Attempting to get lobby list");
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();

    }
}
