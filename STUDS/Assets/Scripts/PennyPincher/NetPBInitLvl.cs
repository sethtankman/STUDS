using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The Level Initializer for the Power Bill Level
/// </summary>
public class NetPBInitLvl : NetworkBehaviour
{
    public Transform[] playerSpawns;
    public Material[] kidsMaterials;
    public GameObject[] timeoutPos;
    public GameObject[] backInPos;
    public GameObject playerPrefab, AIPrefab;
    public GameObject loadingScreen;
    public GameObject pauseMenuUI;

    private bool[] aiInstantiated;
    private bool spawnedPlayers = false;
    private int numAI, playersLoaded;
    private GameObject[] players;
    private Stack<string> aiColors;

    private void Awake()
    {
        NetPWRBill_Manager.backInPos = backInPos;
        NetPWRBill_Manager.timeoutPos = timeoutPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Penny", true);

        PlayerInputManager.instance.DisableJoining();

        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
            player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);

        if (pauseMenuUI)
            GameObject.Find("NetGameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        else
        {
            Debug.LogError("PB no pause menu UI?");
        }
    }

    public void NotifyPlayerReady()
    {
        playersLoaded++;
        if (isServer && playersLoaded == NetGameManager.Instance.playerIDCount)
        {
            StartSequence();
        }
    }

    private void StartSequence()
    {
        numAI = NetGameManager.Instance.numAIToSpawnPB;
        aiInstantiated = new bool[numAI];
        aiColors = NetGameManager.Instance.aiColors;
        int i = 4 - numAI;
        for (int k = 0; k < numAI; k++)
        {
            if (aiInstantiated[k] == false)
            {
                if (aiColors.Count != 0)
                {
                    GameObject AI = Instantiate(AIPrefab, playerSpawns[i].position, playerSpawns[i].transform.rotation);
                    NetworkServer.Spawn(AI);
                    string aiColor = aiColors.Pop();

                    AI.GetComponent<NetworkCharacterMovementController>().RpcSetColorName(aiColor);
                    RpcSetKidMaterial(AI.GetComponent<NetworkIdentity>().netId, GetColorIndex(aiColor));
                }
                aiInstantiated[k] = true;
            }
            i++;
        }
        Invoke("StartGame", 5.0f);
    }

    private void StartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcSetKidMaterial(uint netID, int v)
    {
        Debug.Log("RpcSetKidMaterial");
        if (NetworkClient.spawned.ContainsKey(netID))
        {
            Debug.Log($"Setting material to {v}");
            NetworkClient.spawned[netID].GetComponentInChildren<SkinnedMeshRenderer>(true).material = kidsMaterials[v];
        }
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        spawnedPlayers = true;
        loadingScreen.SetActive(false);
        PauseV2.canPause = true;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<NetKidTimeout>().Init();
        }
    }

    private int GetColorIndex(string _color)
    {
        switch (_color)
        {
            case "red":
                return 0;
            case "blue":
                return 1;
            case "purple":
                return 2;
            case "yellow":
                return 3;
            case "green":
                return 4;
            default:
                Debug.LogError("Invalid color name");
                return -1;
        }
    }

    public bool IsLevelLoaded()
    {
        return spawnedPlayers;
    }
}
