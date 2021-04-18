using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PBInitializeLevel : MonoBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab, AIPrefab;
    public GameObject loadingScreen;

    private bool[] aiInstantiated;
    private bool spawnedPlayers = false;
    private float waitTime = 2f;
    private float currentTime = 0;
    private int numAI;
    private List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        players = ManagePlayerHub.Instance.getPlayers();
        numAI = ManagePlayerHub.Instance.numAIToSpawnPB;
        aiInstantiated = new bool[numAI];
        PlayerInputManager.instance.DisableJoining();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > waitTime && !spawnedPlayers)
        {
            loadingScreen.SetActive(false);
            spawnedPlayers = true;
        }
        else if (!spawnedPlayers)
        {
            // Debug.Log("Spawning player");
            int i = 0;
            foreach (GameObject player in players)
            {
                player.transform.forward = playerSpawns[i].transform.forward;
                player.transform.position = playerSpawns[i].position;
                i++;
            }
            for(int k = 0; k < numAI; k++)
            {
                if (aiInstantiated[k] == false)
                {
                    GameObject AI = Instantiate(AIPrefab, playerSpawns[i].position, playerSpawns[i].transform.rotation);
                    aiInstantiated[k] = true;
                }
                i++;
            }
        }
    }

    public bool IsLevelLoaded()
    {
        return spawnedPlayers;
    }
}
