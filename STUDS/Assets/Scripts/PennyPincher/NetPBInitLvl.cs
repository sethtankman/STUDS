using Mirror;
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
    public GameObject playerPrefab, AIPrefab;
    public GameObject loadingScreen;
    public GameObject pauseMenuUI;

    private bool[] aiInstantiated;
    private bool spawnedPlayers = false;
    private float waitTime = 5f;
    private float currentTime = 0;
    private int numAI;
    private List<GameObject> players;
    private Stack<string> aiColors;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Reached start");
        loadingScreen.SetActive(true);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Penny", true);

        players = NetGameManager.Instance.getPlayers();
        numAI = NetGameManager.Instance.numAIToSpawnPB;
        aiInstantiated = new bool[numAI];
        aiColors = NetGameManager.Instance.aiColors;
        PlayerInputManager.instance.DisableJoining();


        if (pauseMenuUI)
            GameObject.Find("GameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        else
        {
            Debug.LogError("PB no pause menu UI?");
        }
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
                    if (aiColors.Count != 0)
                    {
                        Debug.Log("Instantiating...");
                        GameObject AI = Instantiate(AIPrefab, playerSpawns[i].position, playerSpawns[i].transform.rotation);
                        string aiColor = aiColors.Pop();
                        AI.GetComponentInChildren<NetworkCharacterMovementController>(true).SetColorName(aiColor);
                        AI.GetComponentInChildren<SkinnedMeshRenderer>(true).material = kidsMaterials[GetColorIndex(aiColor)];
                        NetGameManager.Instance.AddPlayer(AI);
                    }
                    aiInstantiated[k] = true;
                }
                i++;
            }
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
