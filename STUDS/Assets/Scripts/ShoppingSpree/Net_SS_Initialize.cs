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
    [SerializeField] private float waitTime = 5f;

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
        PauseV2.canPause = false;
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
            GameObject.Find("NetGameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        else
        {
            Debug.LogError("SS no pause menu UI");
        }
        waitTime += (float)NetworkTime.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkTime.time > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            if (startText)
                startText.text = "";
            for (int i = 0; i < players.Length; i++)
            {
                spawnedPlayers = true;
            }
            GameObject.Find("Row1").GetComponent<NetRandomPicker>().RandomDeactivate();
            PauseV2.canPause = true;
        }
    }

}
