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
    public float waitTime = 5f;
    private float currentTime = 0;

    private List<GameObject> players;
    private GameObject localPlayer;

    public GameObject startCam;
    public GameObject pauseMenuUI;
    public GameObject p1Paper;

    public Text startText;

    public Image[] p1shoppingItemImages;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", true);
        PlayerInputManager.instance.DisableJoining();
        players = NetGameManager.Instance.getPlayers();
        //RpcGiveTrackers();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].AddComponent<SS_ItemTracker>();
            if (players[i].GetComponent<NetworkIdentity>().hasAuthority)
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
            GameObject.Find("GameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        else
        {
            Debug.LogError("SS no pause menu UI");
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            if (startText)
                startText.text = "";
            Debug.Log("Loading in players");
            for (int i = 0; i < players.Count; i++)
            {
                spawnedPlayers = true;
            }
            GameObject.Find("Row1").GetComponent<NetRandomPicker>().RandomDeactivate();
        }
        else if (!spawnedPlayers)
        {
            Debug.Log("Spawning player");
            for (int i = 0; i < players.Count; i++)
            {
                //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                players[i].transform.forward = new Vector3(0, 0, 1);
                players[i].transform.position = playerSpawns[i].position;
            }

        }
    }

}