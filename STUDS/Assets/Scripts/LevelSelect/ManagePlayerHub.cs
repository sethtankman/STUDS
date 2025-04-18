﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class ManagePlayerHub : MonoBehaviour
{
    public List<GameObject> players;
    public Stack<string> aiColors;
    public GameObject playerPrefab, player4PlaceHolder;

    public PlayerConnection playerConnectionPanel;

    public int playerIDCount = 0, numAIToSpawnPB;

    public Material playerColor1;
    public Material playerColor2;
    public Material playerColor3;
    public Material playerColor4;

    public string colorName1;
    public string colorName2;
    public string colorName3;
    public string colorName4;

    public TextMeshProUGUI ReadyText;

    public Text StartText;

    [SerializeField] private bool playerJoined, oldHub;

    public static ManagePlayerHub Instance { get; private set; }
    public PlayerInputManager pim;
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;


    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
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
        if (playerJoined && SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelect"))
        {
            if (StartText)
                StartText.text = "";
            int readyCount = 0;
            foreach (GameObject player in players)
            {
                if (player && player.GetComponent<CharacterMovementController>().GetReadyPlayer("ManagePlayerHub"))
                {
                    readyCount++;
                }
            }
            ReadyText.text = "" + readyCount + "/" + playerIDCount + " players are ready!";
        }
        else if (playerJoined && !SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelect"))
        {
            playerJoined = false; //This is mainly to save time in the if check of the previous if block.
        }

    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (!(SceneManager.GetActiveScene().name == "TheBlock_LevelSelect") && playerIDCount != 3)
        {
            player4PlaceHolder = GameObject.Find("P4PlaceHolder");
            if (!player4PlaceHolder)
            {
                Debug.Log("Couldn't find player 4 placeholder!");
            }
            else
            {
                player4PlaceHolder.SetActive(false);
            }
        }

        if (SceneManager.GetActiveScene().name == "Neighborhood" || SceneManager.GetActiveScene().name == "Shopping_Spree-Scott")
        {
            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<CharacterMovementController>().SetAimAssist(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "TheBlock_LevelSelect" && oldHub)
        {
            Destroy(this.gameObject);
        }
        else
        {
            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<CharacterMovementController>().SetAimAssist(false);
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
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

    public void SaveState()
    {
        if (Instance != null)
        {
            Debug.Log("Making a second instance of singleton very bad");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        playerJoined = true;
        players.Add(pi.gameObject);
        DontDestroyOnLoad(pi.gameObject);
        oldHub = true;
        if (pi.gameObject.GetComponent<CharacterMovementController>())
        {

            pi.gameObject.GetComponent<CharacterMovementController>().SetPlayerID(playerIDCount);
            if (playerIDCount == 0)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor1;
                pi.gameObject.GetComponent<CharacterMovementController>().SetColorName(colorName1);
                playerConnectionPanel.SetPanelImage(playerIDCount, colorName1);

            }
            else if (playerIDCount == 1)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor2;
                pi.gameObject.GetComponent<CharacterMovementController>().SetColorName(colorName2);
                playerConnectionPanel.SetPanelImage(playerIDCount, colorName2);
            }
            else if (playerIDCount == 2)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor3;
                pi.gameObject.GetComponent<CharacterMovementController>().SetColorName(colorName3);
                playerConnectionPanel.SetPanelImage(playerIDCount, colorName3);
                player4PlaceHolder.SetActive(true);
            }
            else if (playerIDCount == 3)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor4;
                pi.gameObject.GetComponent<CharacterMovementController>().SetColorName(colorName4);
                playerConnectionPanel.SetPanelImage(playerIDCount, colorName4);
                player4PlaceHolder.SetActive(false);
            }

        }
        else
        {
            Debug.LogError("Non-CharacterMovementController Joined.");
        }
        playerIDCount++;
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


    // Not using player connection pannel
    /* public void ChangePlayerColor(int id, string color)
    {
        playerConnectionPanel.SetPanelImage(id, color);
    } */

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

    public void DeletePlayers()
    {
        foreach (GameObject player in players)
        {
            if (player.transform.parent != null)
                Destroy(player.transform.parent.gameObject);
            else
                Destroy(player);
        }
        players = new List<GameObject>();
    }

    public void AddPlayer(GameObject newPlayer)
    {
        players.Add(newPlayer);
    }
}
