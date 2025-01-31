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
    [SerializeField] private GameObject[] objectsToDisableOnline;
    public GameObject playerPrefab;

    private bool spawnedPlayers = false;

    public Material strollerColor1;
    public Material strollerColor2;
    public Material strollerColor3;
    public Material strollerColor4;
    public Material strollerColor5;

    private GameObject[] players;
    private int playersLoaded = 0;

    public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject loadingCam;

    [SerializeField] private NetDynamicAICount NDAC;

    public TextMeshProUGUI startText;
    // Start is called before the first frame update
    void Start()
    {
        NetPause.canPause = false;
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", true);
        PlayerInputManager.instance.DisableJoining();
        if(pauseMenuUI)
            NetGameManager.Instance.GetComponent<NetPause>().PauseMenuUI = pauseMenuUI;
    }

    /// <summary>
    /// Each NetCMC will let the server know when they have loaded the scene.
    /// </summary>
    public void NotifyPlayerReady()
    {
        playersLoaded++;
        if (isServer && playersLoaded == NetGameManager.Instance.playerIDCount)
        {
            RpcDisableOnlineObjects();
            NDAC.FillWithAI();
            FindFirstObjectByType<NetRaceTracker>().SetColorsGivePlaceTrackers();
            Invoke(nameof(StartGame), 3.0f);
        }
    }

    private void StartGame()
    {
        if (!spawnedPlayers)
        {
            startText.text = "";
            players = GameObject.FindGameObjectsWithTag("Player");
            if (players != null)
            {

                for (int i = 0; i < players.Length; i++)
                {
                    if (!players[i].GetComponent<NetworkCharacterMovementController>().isAI)
                    {
                        GameObject stroller = Instantiate(strollerPrefab, players[i].transform.position + new Vector3(0, 0, 2f), Quaternion.identity);
                        NetworkServer.Spawn(stroller);
                        RpcDetermineColor(players[i].GetComponent<NetworkCharacterMovementController>().GetColorName(),
                            stroller.GetComponent<NetworkIdentity>().netId,
                            players[i].GetComponent<NetworkCharacterMovementController>().getPlayerID());
                    }
                    players[i].GetComponent<NetworkCharacterMovementController>().inStrollerRace = true;
                }
            }
        }
    }

    [ClientRpc]
    private void RpcDetermineColor(string colorName, uint strollerID, int playerID)
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
        NetPause.canPause = true;
    }

    [ClientRpc]
    private void RpcDisableOnlineObjects()
    {
        foreach (GameObject obj in objectsToDisableOnline)
        {
            obj.SetActive(false);
        }
    }
}
