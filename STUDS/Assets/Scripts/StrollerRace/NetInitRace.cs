using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Mirror;

/// <summary>
/// The level initializer for the stroller race levels.
/// </summary>
public class NetInitRace : NetworkBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    private bool spawnedPlayers = false;
    public float waitTime = 5f;
    private float currentTime = 0;

    public Material strollerColor1;
    public Material strollerColor2;
    public Material strollerColor3;
    public Material strollerColor4;
    public Material strollerColor5;

    private List<GameObject> players;

    public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject loadingCam;

    public TextMeshProUGUI startText;
    // Start is called before the first frame update
    void Start()
    {
        PauseV2.canPause = false;
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", true);
        players = NetGameManager.Instance.getPlayers();
        PlayerInputManager.instance.DisableJoining();
        if(pauseMenuUI)
            GameObject.Find("GameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
    }

    // Update is called once per frame
    void Update()
    {

        currentTime += Time.deltaTime;
        if (currentTime > waitTime && !spawnedPlayers)
        {
            startText.text = "";
            if (isServer && players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    GameObject stroller = Instantiate(strollerPrefab, playerSpawns[i].position + new Vector3(0, 0, 2f), Quaternion.identity);
                    NetworkServer.Spawn(stroller); 
                    RpcDetermineColor(players[i].GetComponent<NetworkCharacterMovementController>().GetColorName(), 
                        stroller.GetComponent<NetworkIdentity>().netId, 
                        players[i].GetComponent<NetworkCharacterMovementController>().getPlayerID());
                    players[i].GetComponent<NetworkCharacterMovementController>().inStrollerRace = true;
                }
            }
        }
        else if(!spawnedPlayers)
        {
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].transform.forward = new Vector3(0, 0, 1);
                    players[i].transform.position = playerSpawns[i].position;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcDetermineColor(string colorName, uint strollerID, int playerID)
    {
        if (NetworkClient.spawned.ContainsKey(strollerID) == false)
        {
            Debug.LogError("Stroller ID not found");
            return;
        }
        Destroy(loadingCam);
        spawnedPlayers = true;
        NetworkClient.spawned[strollerID].GetComponent<StrollerController>().SetID(playerID);
        NetworkClient.spawned[strollerID].GetComponent<StrollerController>().DetermineColor(colorName);
        PauseV2.canPause = true;
    }
}
