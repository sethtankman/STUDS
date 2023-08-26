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
    public List<GameObject> players;
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

    [SerializeField] private bool playerJoined, oldHub;

    public static NetGameManager Instance { get; private set; }
    public PlayerInputManager pim;

    private void Awake()
    {
        colorMaterials = new Dictionary<string, Material>();
        for (int i = 0; i < colorNames.Length; i++)
        {
            //Debug.Log($"Adding color: {colorNames[i]}");
            colorMaterials[colorNames[i]] = materials[i];
        }
         
    }


    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            for (int i = 0; i < colorNames.Length; i++)
                availableColors.Add(colorNames[i]);
        }

        players = new List<GameObject>();
        playerJoined = false;

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
                        PauseV2 pauseFunction = GameObject.Find("GameManager").GetComponent<PauseV2>();
                        pauseFunction.Pause();
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
        if (playerJoined && SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
        {
            if (StartText)
                StartText.text = "";
            int readyCount = 0;
            foreach (GameObject player in players)
            {
                if (player && player.GetComponent<NetworkCharacterMovementController>().GetReadyPlayer("ManagePlayerHub"))
                {
                    readyCount++;
                }
            }
            ReadyText.text = "" + readyCount + "/" + playerIDCount + " players are ready!";
        }
        else if (playerJoined && !SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
        {
            playerJoined = false; //This is mainly to save time in the if check of the previous if block.
        }

    }

    // For debugging.
    private void OnDisable()
    {
        //Debug.LogWarning("Who disabled me?");
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

            GameObject.Find("GameManager").GetComponent<PauseV2>().Pause();
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    // Sets aim assist to true or false per level.
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "TheBlock_LevelSelectOnlineMultiplayer" || SceneManager.GetActiveScene().name == "Network_Shopping_Spree")
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
            }
        }
        else
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(false);
            }
        }
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
        foreach (GameObject playerObj in players)
        {   
            int playerID = playerObj.GetComponent<NetworkCharacterMovementController>().getPlayerID();
            Debug.Log($"Player ID: {playerID}");
            playerObj.name = $"STUD{playerID + 1}";
            AssignColor(playerObj, playerColors[playerID]);
        }
    }


    [ClientRpc]
    public void RpcSaveState()
    {
        if (Instance != null)
        {
            Debug.LogError("Making a second instance of singleton very bad");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }
    }

    [ClientRpc]
    public void RpcAddPlayer(uint playerID)
    {
        Debug.Log("RPC Add Player");
        if (NetworkIdentity.spawned.ContainsKey(playerID))
            AddPlayer(NetworkIdentity.spawned[playerID].gameObject);
        else
            Debug.LogError("NetwordIdentity of AI was not assigned.");
    }

    public void ChangePlayerColor(int playerID, string colorName)
    {
        string oldColor = playerColors[playerID];
        availableColors.Add(oldColor);
        Debug.Log($"Attempting to remove: {colorName}");
        availableColors.Remove(colorName);
        playerColors[playerID] = colorName;
    }

    public void AssignColor(GameObject playerObj, string color)
    {
        playerObj.GetComponentInChildren<SkinnedMeshRenderer>().material = colorMaterials[color];
        NetworkCharacterMovementController netCMC = playerObj.GetComponent<NetworkCharacterMovementController>();
        netCMC.color = color;
        playerConnectionPanel.SetPanelImage(netCMC.getPlayerID(), color);
    }


    public void NetworkPlayerJoin(GameObject player)
    {
        playerJoined = true;
        players.Add(player);
        Debug.Log($"Added player{playerIDCount}");
        DontDestroyOnLoad(player);
        if (isServer)
        {
            player.GetComponent<NetworkCharacterMovementController>().SetPlayerID(playerIDCount);
            player.name = $"STUD{playerIDCount + 1}";
            string color = availableColors.ToArray()[0];
            AssignColor(player, color);
            playerColors[playerIDCount] = color;
            availableColors.Remove(color);
            playerIDCount++;
        } 
    }

    public List<GameObject> getPlayers()
    {
        return players;
    }

    /// <summary>
    /// Adjusts player list and available colors when a player leaves.
    /// </summary>
    /// <param name="i">The index in the player list of the player to be kicked.</param>
    public void RemovePlayerAssignments(int i)
    {
        Debug.Log($"Calling Remove Player Assignments {i}");
        if (i < players.Count)
            players.RemoveAt(i);
        availableColors.Add(playerColors[i]);
        playerIDCount--;
    }

    /// <summary>
    /// refreshes player list.
    /// </summary>
    /// <returns>If this game manager is on server</returns>
    public bool DeletePlayers()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        players = new List<GameObject>();
        return isServer;
    }

    public void AddPlayer(GameObject newPlayer)
    {
        Debug.Log("Adding Player");
        players.Add(newPlayer);
    }
}
