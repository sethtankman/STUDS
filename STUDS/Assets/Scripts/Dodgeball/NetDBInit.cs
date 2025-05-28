using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Mirror;

/// <summary>
/// The level initializer for the online dodgeball level.
/// </summary>
public class NetDBInit : NetworkBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    [SerializeField] private Material[] materials;

    private GameObject[] players;
    private List<string> aiColors;
    private int playersLoaded = 0;

    //public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject startCam;
    public GameObject startUI;

    public TextMeshProUGUI startText;
    [SerializeField] private NetDBUI scorePanel;
    [SerializeField] private NetDynamicAICount NDAC;

    // Called for all players when they load the level.
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Dodgeball", true);
        NetPause.canPause = false;
        if (NetGameManager.Instance)
        {
            NetGameManager.Instance.GetComponent<NetPause>().PauseMenuUI = pauseMenuUI;
        }
        else
            Debug.LogWarning("Manage Player Hub not found!");
        PlayerInputManager.instance.DisableJoining();

        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().isLocalPlayer)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
                player.GetComponent<NetworkCharacterMovementController>().SetCanMove(false);
            }
        }
    }

    /// <summary>
    /// Initiates starting sequence once all clients have been loaded.
    /// </summary>
    private void StartSequence()
    {
        NDAC.FillWithAI();
        aiColors = new List<string> { "red", "blue", "purple", "yellow", "green" };
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if(player.GetComponent<NetworkCharacterMovementController>().isAI == false)
            {
                aiColors.Remove(player.GetComponent<NetworkCharacterMovementController>().color);
            }
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkCharacterMovementController>().isAI)
            {
                string aiColor = aiColors[0];
                players[i].GetComponentInChildren<NetworkCharacterMovementController>(true).SetColorName(aiColor);
                RpcSetMaterial(players[i], aiColor);
                aiColors.Remove(aiColor);
            }
        }
        Invoke("StartGame", 5.0f);
    }

    /// <summary>
    /// Only called on the server.
    /// </summary>
    public void NotifyPlayerReady()
    {
        playersLoaded++;
        if (isServer && playersLoaded == NetGameManager.Instance.playerIDCount)
        {
            StartSequence();
        }
    }

    private void StartGame()
    {
        if (isServer)
        {
            RpcStartGame();
        }
    }

    /// <summary>
    /// Starts the game for all players
    /// </summary>
    [ClientRpc]
    private void RpcStartGame()
    {
        scorePanel.UpdateSpriteColors();
        NetDBGameManager.Instance.InitScores();
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) { Debug.LogError("No players found"); }
        foreach(GameObject player in players)
        {
            player.GetComponent<NetworkCharacterMovementController>().SetCanMove(true);
        }
        Destroy(startCam);
        Destroy(startText);
        Destroy(startUI);
        NetPause.canPause = true;
    }

    [ClientRpc]
    private void RpcSetMaterial(GameObject player, string aiColor)
    {
        player.GetComponentInChildren<SkinnedMeshRenderer>(true).material = materials[GetColorIndex(aiColor)];
    }

    private int GetColorIndex(string _color)
    {
        switch (_color)
        {
            case "red":
                return 0;
            case "blue":
                return 1;
            case "purple":
                return 2;
            case "yellow":
                return 3;
            case "green":
                return 4;
            default:
                Debug.LogError("Invalid color name");
                return -1;
        }
    }
}
