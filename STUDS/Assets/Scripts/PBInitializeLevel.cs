using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PBInitializeLevel : MonoBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;
    public GameObject loadingScreen;

    private bool spawnedPlayers = false;
    private float waitTime = 2f;
    private float currentTime = 0;
    private List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        players = ManagePlayerHub.Instance.getPlayers();
        PlayerInputManager.instance.DisableJoining();
        if(players.Count > 0)
        {
            int dadNum = Random.Range(0, players.Count);
            for(int i = 0; i < players.Count; i++)
            {
                if(i != dadNum)
                {
                    players[i].GetComponent<CharacterMovementController>().SetIsChild(true);
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
            loadingScreen.SetActive(false);
        }
        else if (!spawnedPlayers)
        {
            Debug.Log("Spawning player");
            int i = 0;
            foreach (GameObject player in players)
            {
                player.transform.forward = playerSpawns[i].transform.forward;
                player.transform.position = playerSpawns[i].position;
                i++;
            }
        }
    }
}
