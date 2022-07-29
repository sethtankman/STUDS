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

    private int count = 300;
    [SyncVar]
    public int playerIDCount = 0;
    public int numAIToSpawnPB;

    public Dictionary<string, Material> colorMaterials;
    [SyncVar]
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
            Debug.Log($"Adding color: {colorNames[i]}");
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

    // Update is called once per frame
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

    private void OnLevelWasLoaded(int level)
    {
        // TODO: Change this for network scenes once they are ready.
        if (SceneManager.GetActiveScene().name == "TheBlock_LevelSelectOnlineMultiplayer" || SceneManager.GetActiveScene().name == "Shopping_Spree-Scott")
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "TheBlock_LevelSelectOnlineMultiplayer" && oldHub)
        {
            Destroy(this.gameObject);
        }
        else
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(false);
            }
        }
    }


    [ClientRpc]
    public void RpcSetPlayerVariables()
    {
        if (isServer)
            return;
        Debug.Log("RPC Called");
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
        playerObj.GetComponent<NetworkCharacterMovementController>().color = color;
        playerConnectionPanel.SetPanelImage(playerIDCount, color);
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

    /// <summary>
    /// This was causing issues when called from the GameManager Input System event,
    /// so now we aren't using it.
    /// </summary>
    /// <param name="pi"></param>
    public void HandlePlayerLeave(PlayerInput pi)
    {
        Debug.Log(pi.devices.ToString());
        InputSystem.RemoveDevice(pi.devices[0]);
        Debug.Log("A Player has left!");
    }

    public List<GameObject> getPlayers()
    {
        return players;
    }

    public void KickPlayer(int i)
    {
        Destroy(players[i]);
        players[i] = null;
        for (int j = i; j < 4; j++)
        {
            Debug.Log("j=" + j);
            for (int index = 0; index < players.Count; index++)
            {
                Debug.Log("Index: " + index);
                if (players[index])
                    Debug.Log(players[index].name);
            }
            if ((j + 1 == 4 || players[j + 1] == null) && players[j] != null)
            {
                Debug.Log("Destroying Player: " + j);
                Destroy(players[j]);
                players[j] = null;
            }
            else if (j + 1 != 4 || (players[j + 1] != null && players[j] == null))
            {
                Debug.Log("Creating player");
                players[j] = PlayerInput.Instantiate(playerPrefab, j, players[j + 1].GetComponent<PlayerInput>().currentControlScheme, j, players[j + 1].GetComponent<PlayerInput>().devices.ToArray()).gameObject;
                Destroy(players[j + 1]);
                players[j + 1] = null;
            }
        }
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
        players.Add(newPlayer);
    }
}
