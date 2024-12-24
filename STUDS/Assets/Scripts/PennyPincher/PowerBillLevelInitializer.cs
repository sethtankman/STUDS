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

    public GameObject startCam;

    public Text startText;
    // Start is called before the first frame update
    void Start()
    {
      
        players = ManagePlayerHub.Instance.getPlayers();
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
        }
        else if (!spawnedPlayers)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.forward = startCam.transform.forward;
                players[i].transform.position = playerSpawns[i].position;
            }
        }
    }
}
