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
            Debug.Log("Spawning player");
            for (int i = 0; players[i]; i++)
            {
                players[i].transform.forward = playerSpawns[i].transform.forward;
                players[i].transform.position = playerSpawns[i].position;
            }
        }
    }

    public bool IsLevelLoaded()
    {
        return spawnedPlayers;
    }
}
