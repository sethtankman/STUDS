using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class NetManageGame : NetworkBehaviour
{

    private List<PlayerConfiguration> configs;

    public GameObject[] checkpoints;

    public StudsNetworkManager netManager;

    public TextMeshProUGUI FinishText;
    public TextMeshProUGUI FinishTimer;

    public SteamAchievements sa;

    public float endTimer;

    public float swapTime;

    public string soundName;

    public AK.Wwise.Event mySource;

    private float timeCount;
    private bool display;
    private int playerID;
    private float timer;
    private bool playerFinish = false;

    private int positions = 1;
    private int noFinishPositions = -1;
    private bool endSequenceCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
        netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            timeCount += Time.deltaTime;
            FinishText.text = $"Player {playerID} has finished the race!";
            playerFinish = true;
            if (timeCount >= swapTime)
            {
                display = false;
                timeCount = 0;
            }
        }
        else if (!display)
        {
            FinishText.text = "";
        }

        if (playerFinish)
        {
            timer += Time.deltaTime;
            FinishTimer.text = "A player has finished the race! The race will end in " + (int)(endTimer - timer) + " seconds!";
            if(timer >= endTimer && endSequenceCalled == false)
            {
                endSequenceCalled = true;
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in allPlayers)
                {
                    if (player.GetComponent<NetworkCharacterMovementController>().isAI)
                    {
                        DontDestroyOnLoad(player.transform.parent.gameObject);
                        NetGameManager.Instance.AddPlayer(player);
                    }
                }
                List<GameObject> players = NetGameManager.Instance.getPlayers();
                foreach (GameObject player in players)
                {
                    if(player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
                    {
                        player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(noFinishPositions);
                        noFinishPositions--;
                    }
                }
                netManager.ServerChangeScene("NetVictoryStands");
            }
        }
        else
        {
            FinishTimer.text = "";
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if(collider.GetComponent<NetworkCharacterMovementController>().getCheckpointCount() == checkpoints.Length && collider.GetComponent<NetworkCharacterMovementController>().GetHasGrabbed())
            {
                if(collider.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
                {
                    display = true;
                    if (SceneManager.GetActiveScene().name.Equals("TheBlock_Scott"))
                        sa.UnlockAchievement("SR_COMPLETE");
                    else
                        sa.UnlockAchievement("SR_DOWNTOWN");
                    playerID = collider.GetComponent<NetworkCharacterMovementController>().getPlayerID() + 1;
                    collider.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(positions);
                    positions++;
                    if (collider.GetComponent<NetworkCharacterMovementController>().isAI == false)
                    {
                        mySource.Post(gameObject);
                    }
                }

            }
        }
    }

    public void updatePlayerConfigsList(List<PlayerConfiguration> configs)
    {
        foreach(PlayerConfiguration config in configs)
        {
            if (!this.configs.Contains(config))
            {
                this.configs.Add(config);
            }
        }
    }
}
