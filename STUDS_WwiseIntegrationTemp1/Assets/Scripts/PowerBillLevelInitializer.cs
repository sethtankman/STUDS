using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PowerBillLevelInitializer : MonoBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    private bool spawnedPlayers = false;
    private float waitTime = 8f;
    private float currentTime = 0;

    private List<GameObject> players;

    private int maxRounds;
    private int roundCount;

    public GameObject startCam;

    public Text startText;
    // Start is called before the first frame update
    void Start()
    {
        players = ManagePlayerHub.Instance.getPlayers();
        maxRounds = players.Count + 1;
        roundCount = 1;
        PlayerInputManager.instance.DisableJoining();
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
        }
        else if (!spawnedPlayers)
        {
            Debug.Log("Spawning player");
            for (int i = 0; i < players.Count; i++)
            {
                //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                players[i].transform.forward = startCam.transform.forward;
                players[i].transform.position = playerSpawns[i].position;
                if(roundCount - 1 == i)
                {
                    //TODO add method in character script players[i].GetComponent<CharacterMovementController>().
                }
                else
                {
                    //TODO add method in character script players[i].GetComponent<CharacterMovementController>().
                }
            }
        }
    }
}
