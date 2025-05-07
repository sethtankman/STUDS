using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using Mirror;
using System;

/// <summary>
/// Takes the place of Manage Player Hub for network play.
/// </summary>
public class NetGameManager : NetworkBehaviour
{
    public List<GameObject> AIplayers;
    public Stack<string> aiColors;
    public List<string> availableColors;
    public GameObject playerPrefab, player4PlaceHolder;

    public PlayerConnection playerConnectionPanel;

    [SyncVar]
    public int playerIDCount = 0;
    public int numAIToSpawnPB;

    public Dictionary<string, Material> colorMaterials;
    public readonly SyncDictionary<int, string> playerColors = new SyncDictionary<int, string>();

    public Material[] materials;
    public string[] colorNames;

    public TextMeshProUGUI ReadyText;

    public Text StartText;

    [SerializeField] private bool playerJoined;
    [SerializeField] private NetPause np; // to be set in editor

    public static NetGameManager Instance { get; private set; }
    public PlayerInputManager pim;

    private bool UILocked = false;

    private void Awake()
    {
        colorMaterials = new Dictionary<string, Material>();
        for (int i = 0; i < colorNames.Length; i++)
        {
            colorMaterials[colorNames[i]] = materials[i];
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        AIplayers = new List<GameObject>();
        playerJoined = false;
        if (isServer)
        {
            for (int i = 0; i < colorNames.Length; i++)
                availableColors.Add(colorNames[i]);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        Debug.Log("A device has been added.");
                        // New Device.
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        Debug.Log("A Player has disconnected");
                        np.Pause();
                        break;
                    case InputDeviceChange.Reconnected:
                        Debug.Log("A Player has reconnected");
                        // Plugged back in.
                        break;
                    case InputDeviceChange.Removed:
                        Debug.Log("A device has been removed");
                        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                        break;
                    default:
                        // See InputDeviceChange reference for other event types.
                        break;
                }
            };
    }

    // Update is called once per frame.  Updates playersReady text, removes start text.
    void Update()
    {
        if (!UILocked && playerJoined && SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
        {
            if (StartText)
                StartText.text = "";
            int readyCount = 0;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player && player.GetComponent<NetworkCharacterMovementController>().GetReadyPlayer())
                {
                    readyCount++;
                }
            }
            if (readyCount == playerIDCount)
            {
                ReadyText.text = "Loading level...";
                UILocked = true;
            } else
            {
                ReadyText.text = $"{readyCount}/{playerIDCount} players are ready!";
            }
        }
        else if (playerJoined && !SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
        {
            playerJoined = false; //This is mainly to save time in the if check of the previous if block.
        }

    }

    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");

            GameObject.Find("GameManager").GetComponent<NetPause>().Pause();
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        playerJoined = true;
        playerIDCount++;
    }

    /// <summary>
    /// Currently I will assume that when a player leaves, their gameobject is destroyed.
    /// This is how we will know which player left, which color to make available again,
    /// and which ID to call KickPlayer with.
    /// </summary>
    public void HandleLeavePlayer(int id)
    {
        RemovePlayerAssignments(id);
    }

    [ClientRpc]
    public void RpcSetPlayerVariables()
    {
        if (isServer)
            return;
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            NetworkCharacterMovementController netCMC = playerObj.GetComponent<NetworkCharacterMovementController>();
            if (netCMC.isAI) { continue; } // Do not need to set AI colors in RPC
            int playerID = netCMC.getPlayerID();
            Debug.Log($"Player ID: {playerID}");
            playerObj.name = $"STUD{playerID + 1}";
            playerObj.GetComponentInChildren<SkinnedMeshRenderer>().material = colorMaterials[playerColors[playerID]];
            netCMC.SetToMini(netCMC.isMini);
            netCMC.color = playerColors[playerID];
        }
    }


    [ClientRpc]
    public void RpcSaveState()
    {
        if (Instance != null)
        {
            //Changed From Error to Warning since this is expected to happen when you play a level -> return to level select -> play another level.
            Debug.LogWarning("Making a second instance of singleton very bad");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }

        NetGameManager.Instance.AIplayers = new List<GameObject>();
    }

    [ClientRpc]
    public void RpcAddPlayer(uint playerID)
    {
        Debug.Log("RPC Add Player");
        if (NetworkClient.spawned.ContainsKey(playerID))
            AddPlayer(NetworkClient.spawned[playerID].gameObject);
        else
            Debug.LogError("NetwordIdentity of AI was not assigned.");
    }

    public void ChangePlayerColor(int playerID, string colorName)
    {
        string oldColor = playerColors[playerID];
        availableColors.Add(oldColor);
        availableColors.Remove(colorName);
        playerColors[playerID] = colorName;
    }

    /// <summary>
    /// Only to be called by the server
    /// </summary>
    /// <param name="playerObj"></param>
    /// <param name="playerID"></param>
    public void AssignColor(GameObject playerObj, int playerID)
    {
        string color = availableColors.ToArray()[0];
        playerObj.GetComponentInChildren<SkinnedMeshRenderer>().material = colorMaterials[color];
        NetworkCharacterMovementController netCMC = playerObj.GetComponent<NetworkCharacterMovementController>();
        netCMC.color = color;
        playerColors[playerID] = color;
        availableColors.Remove(color);
        if (playerConnectionPanel)
            playerConnectionPanel.SetPanelImage(netCMC.getPlayerID(), color);
        else
            Debug.Log("Player connection panel missing.");
    }


    public void NetworkPlayerJoin(GameObject player)
    {
        playerJoined = true;
        if (isServer)
        {
            player.GetComponent<NetworkCharacterMovementController>().SetPlayerID(playerIDCount);
            player.name = $"STUD{playerIDCount + 1}";
            
            AssignColor(player, playerIDCount);
            playerIDCount++;
        } 
    }

    public List<GameObject> getPlayers()
    {
        return AIplayers;
    }

    /// <summary>
    /// Adjusts player list and available colors when a player leaves.
    /// </summary>
    /// <param name="i">The index in the player list of the player to be kicked.</param>
    public void RemovePlayerAssignments(int i)
    {
        Debug.Log($"Calling Remove Player Assignments {i}");
        if(playerColors.ContainsKey(i))
            availableColors.Add(playerColors[i]);
        else
        {
            Debug.LogWarning($"PlayerColors does not contain object at index {i}");
        }
        playerIDCount--;
    }

    /// <summary>
    /// refreshes player list.  Also Removes Player Input Manager.
    /// </summary>
    /// <returns>If this game manager is on server</returns>
    public bool DeletePlayers()
    {
        foreach (GameObject player in AIplayers)
        {
            if (player.transform.parent != null)
                Destroy(player.transform.parent.gameObject);
            else
                Destroy(player);
        }
        AIplayers = new List<GameObject>();
        Destroy(GetComponent<PlayerInputManager>());
        return isServer;
    }

    public void RemoveInstance()
    {
        Instance = null;
    }

    public void AddPlayer(GameObject newPlayer)
    {
        AIplayers.Add(newPlayer);
    }

    public void SetPauseMenuPanel(GameObject panel)
    {
        np.PauseMenuUI = panel;
    }
}
