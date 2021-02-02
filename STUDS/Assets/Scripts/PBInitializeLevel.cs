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
    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        players = ManagePlayerHub.Instance.getPlayers().ToArray();
        PlayerInputManager.instance.DisableJoining();
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
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.forward = playerSpawns[i].transform.forward;
                players[i].transform.position = playerSpawns[i].position;
                if(i > 0)
                {
                    players[i].transform.localScale = new Vector3(20, 20, 20); //Shrink the player. OG size is 30, 30, 30
                    players[i].GetComponent<CharacterMovementController>().SetBinky(true); //Activate the binky!!!!
                    players[i].GetComponent<CharacterMovementController>().isMini = true; //The Eugine will now act as a child.
                }
            }
        }
    }
}
