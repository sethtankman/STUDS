using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;

public class ManagePlayerHub : MonoBehaviour
{
    public List<GameObject> players;

    public PlayerConnection playerConnectionPanel;

    private int playerIDCount = 0, count = 300;

    public Material playerColor1;
    public Material playerColor2;
    public Material playerColor3;
    public Material playerColor4;

    public string colorName1;
    public string colorName2;
    public string colorName3;
    public string colorName4;

    public Text ReadyText;

    public Text StartText;

    private bool playerJoined;

    public static ManagePlayerHub Instance { get; private set; }
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
        if (SceneManager.GetActiveScene().name == "TheBlock_Scott" || SceneManager.GetActiveScene().name == "Shopping_Spree-Scott")
        {
            //Debug.Log("This was called.");
            foreach (GameObject player in players)
            {
                player.GetComponent<CharacterMovementController>().SetAimAssist(true);
            }
        }
        else
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<CharacterMovementController>().SetAimAssist(false);
            }
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

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        playerJoined = false;

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
                if (player.GetComponent<CharacterMovementController>().GetReadyPlayer())
                {
                    readyCount++;
                }
            }
            ReadyText.text = "" + readyCount + "/" + players.Count + " players are ready! Choose your level!";
        }
        else if (playerJoined && !SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelect"))
        {
            playerJoined = false; //This is mainly to save time in the if check of the previous if block.
        }

    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        playerJoined = true;
        players.Add(pi.gameObject);
        DontDestroyOnLoad(pi.gameObject);
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
            }
            else if (playerIDCount == 3)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor4;
                pi.gameObject.GetComponent<CharacterMovementController>().SetColorName(colorName4);
                playerConnectionPanel.SetPanelImage(playerIDCount, colorName4);
            }

        }
        else if (pi.gameObject.GetComponent<CM_CharacterMovementController>())
            pi.gameObject.GetComponent<CM_CharacterMovementController>().SetPlayerID(playerIDCount);

        playerIDCount++;
    }

    public void HandlePlayerLeave(PlayerInput pi)
    {
        players.Remove(pi.gameObject);
        Debug.Log("A Player has left!");
    }

    public List<GameObject> getPlayers()
    {
        return players;
    }

    public void DeletePlayers()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
    }
}
