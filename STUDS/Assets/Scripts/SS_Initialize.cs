using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SS_Initialize : MonoBehaviour
{
    public Transform[] playerSpawns;

    private bool spawnedPlayers = false;
    public float waitTime = 6f;
    private float currentTime = 0;

    private GameObject[] players;

    public GameObject strollerPrefab;

    public GameObject startCam;

    public Text player1List;

    public Text player2List;

    public Text startText;
    // Start is called before the first frame update
    void Start()
    {
        players = ManagePlayerHub.Instance.getPlayers().ToArray();
        PlayerInputManager.instance.DisableJoining();
        if(players.Length > 0)
        {
            players[0].gameObject.GetComponent<SS_ItemTracker>().listText = player1List;

        }else if(players.Length > 1) 
        {
            players[1].gameObject.GetComponent<SS_ItemTracker>().listText = player2List;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            startText.text = "";
            Debug.Log("Loading in players");
            for (int i = 0; i < players.Length; i++)
            { 
                spawnedPlayers = true;
            }
        }
        else if (!spawnedPlayers)
        {
            Debug.Log("Spawning player");
            for (int i = 0; i < players.Length; i++)
            {
                //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                players[i].transform.forward = new Vector3(0, 0, 1);
                players[i].transform.position = playerSpawns[i].position;
            }
        }
    }
}
