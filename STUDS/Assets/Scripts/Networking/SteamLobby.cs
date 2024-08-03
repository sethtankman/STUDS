using Steamworks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteamLobby : NetworkBehaviour
{

    private StudsNetworkManager netManager;
    private const string hostAddressKey = "HostAddress";
    private int refreshTimer;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyDataUpdate_t> LobbyListRequested;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;

    public List<CSteamID> lobbyIDS = new List<CSteamID>();
    public bool fetchLobbies;
    public ulong current_lobbyID;
    public CSteamID joinedLobbyID;

    /// <summary>The one and only SteamLobby</summary>
    public static SteamLobby singleton { get; private set; }

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

    public void Awake()
    {
        if (!InitializeSingleton()) return;
    }

    /// <summary>
    /// Every time we enter the Main Menu, we need to start using the new Network Manager.  
    /// Players also need to individually leave the lobby on steam's server once they leave the lobby.
    /// </summary>
    /// <param name="level">number of the main menu is 1</param>
    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
             netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
    }

    public void Start()
    {
        netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
        refreshTimer = 1000;

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

    public void Update()
    {
        if(fetchLobbies)
        {
            refreshTimer--;
            if(refreshTimer < 0)
            {
                refreshTimer = 1000;
                GetLobbyList();
            }
        }
    }

    public void OnDisable()
    {
        Debug.Log("Can't disable SteamLobby, only destroy");
        singleton = null;
        Destroy(gameObject);
    }

    public bool InitializeSingleton()
    {
        if (singleton != null && singleton == this)
            return true;

        if (singleton != null)
        {
            Destroy(gameObject);

            // Return false to not allow collision-destroyed second instance to continue.
            return false;
        }
        Debug.Log("SteamLobby created singleton (DontDestroyOnLoad)");
        singleton = this;
        DontDestroyOnLoad(gameObject);

        return true;
    }

    public void HostLobby()
    {
        Debug.Log("Trying to host lobby");
        netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
        if(netManager)
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, netManager.maxConnections);
        else { Debug.LogError("Could not find NetworkManager");  }
    }

    /// <summary>
    /// When we return to the main menu, stop being a host or client.
    /// </summary>
    public void HandleLeave()
    {
        PauseV2.gameisPaused = false;
        netManager.StopHost();
        SteamMatchmaking.LeaveLobby(joinedLobbyID);
    }

    public void JoinRoomAsClient()
    {
        if (NetworkMenuActions.instance.SelectedRoomId.IsValid())
        {
            SteamMatchmaking.JoinLobby(NetworkMenuActions.instance.SelectedRoomId);
            joinedLobbyID = NetworkMenuActions.instance.SelectedRoomId;

        }
    }

    /// <summary>
    /// Sets the joined Steam lobby to unavailable.
    /// </summary>
    public void SetLobbyClosed()
    {
        SteamMatchmaking.SetLobbyData(joinedLobbyID, "isClosed", "t");
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        Debug.Log("OnLobbyCreated");
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError($"OnLobbyCreated abort: {callback.m_eResult}");
            return;
        }

        netManager.StartHost();

        joinedLobbyID = new CSteamID(callback.m_ulSteamIDLobby); // I added this to make leaving have the CSteamID it needs - Addison
        SteamMatchmaking.SetLobbyData(joinedLobbyID, hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(
            joinedLobbyID,
            "name",
            $"{SteamFriends.GetPersonaName()}'s lobby");
        bool result = SteamMatchmaking.SetLobbyData(joinedLobbyID, "isClosed", "f");
    }

    private void OnGameLobbyJoin(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        current_lobbyID = callback.m_ulSteamIDLobby;
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(current_lobbyID),
            hostAddressKey);
        string isClosed = SteamMatchmaking.GetLobbyData(
            new CSteamID(current_lobbyID),
            "isClosed");
        if(isClosed == "t")
        {
            HandleLeave();
            GetLobbyList();
            return;
        }


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
        if (lobbyIDS.Count > 0) { 
            lobbyIDS.Clear();
            NetworkMenuActions.instance.DestroyOldLobbyListItems();
        }

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        Debug.Log("Attempting to get lobby list");
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();

    }
}
