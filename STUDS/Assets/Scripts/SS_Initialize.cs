using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SS_Initialize : MonoBehaviour
{
    public Transform[] playerSpawns;

    private bool spawnedPlayers = false;
    public float waitTime = 5f;
    private float currentTime = 0;

    private GameObject[] players;

    public GameObject startCam;

    public GameObject p1Paper, p2Paper, p3Paper, p4Paper;

    public Text player1List;

    public Text player2List;

    public Text player3List;

    public Text player4List;

    public Text startText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", true);
        players = ManagePlayerHub.Instance.getPlayers().ToArray();
        PlayerInputManager.instance.DisableJoining();
        if(players.Length > 0)
        {
            players[0].gameObject.AddComponent<SS_ItemTracker>();
            players[0].gameObject.GetComponent<SS_ItemTracker>().listText = player1List;
            if (players.Length > 1)
            {
                players[1].gameObject.AddComponent<SS_ItemTracker>();
                players[1].gameObject.GetComponent<SS_ItemTracker>().listText = player2List;
                p2Paper.SetActive(true);
                if (players.Length > 2)
                {
                    players[2].gameObject.AddComponent<SS_ItemTracker>();
                    players[2].gameObject.GetComponent<SS_ItemTracker>().listText = player3List;
                    p3Paper.SetActive(true);
                    if (players.Length > 3)
                    {
                        players[3].gameObject.AddComponent<SS_ItemTracker>();
                        players[3].gameObject.GetComponent<SS_ItemTracker>().listText = player4List;
                        p4Paper.SetActive(true);
                    }
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            if(startText)
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
