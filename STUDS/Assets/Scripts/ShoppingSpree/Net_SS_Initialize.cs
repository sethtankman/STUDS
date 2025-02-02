using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;


/// <summary>
/// The level initializer for the Shopping Spree Level in Networked multiplayer.
/// </summary>
public class Net_SS_Initialize : NetworkBehaviour
{
    public Transform[] playerSpawns;

    private bool spawnedPlayers = false;
    private int playersLoaded = 0;

    private GameObject[] players;
    private GameObject localPlayer;

    public GameObject startCam;
    public GameObject pauseMenuUI;
    public GameObject p1Paper;

    public Text startText;

    public Image[] p1shoppingItemImages;

    // Start is called before the first frame update
    void Start()
    {
        NetPause.canPause = false;
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", true);
        PlayerInputManager.instance.DisableJoining();
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].AddComponent<SS_ItemTracker>();
            players[i].GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
            if (players[i].GetComponent<NetworkIdentity>().isOwned)
            {
                localPlayer = players[i];
            }
            else
            {
                players[i].GetComponent<SS_ItemTracker>().isLocal = false;
            }
        }
        if (localPlayer)
        {
            SS_ItemTracker tracker = localPlayer.GetComponent<SS_ItemTracker>();
            tracker.shoppingItemImages = p1shoppingItemImages;
            tracker.myPaper = p1Paper;
        }
        if (pauseMenuUI)
            GameObject.Find("NetGameManager").GetComponent<NetPause>().PauseMenuUI = pauseMenuUI;
        else
        {
            Debug.LogError("SS no pause menu UI");
        }
    }


    public void NotifyPlayerReady()
    {
        playersLoaded++;
        if (isServer && playersLoaded == NetGameManager.Instance.playerIDCount)
        {
            Invoke(nameof(StartGame), 5.0f);
        }
    }

    private void StartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        startCam.SetActive(false);
        if (startText)
            startText.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            spawnedPlayers = true;
        }
        GameObject.Find("Row1").GetComponent<NetRandomPicker>().RandomDeactivate();
        NetPause.canPause = true;
    }
}
