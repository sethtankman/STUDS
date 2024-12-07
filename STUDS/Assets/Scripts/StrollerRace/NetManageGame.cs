using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

/// <summary>
/// The game manager for the racing levels.
/// </summary>
public class NetManageGame : NetworkBehaviour
{

    private List<PlayerConfiguration> configs;

    public GameObject[] checkpoints;

    public StudsNetworkManager netManager;

    public TextMeshProUGUI FinishText;
    public TextMeshProUGUI FinishTimer;

    public float endTimer;

    public float swapTime;

    public string soundName;

    public AK.Wwise.Event mySource;

    [SyncVar] private bool display;
    [SyncVar] private int playerID;
    private bool playerFinish = false;

    private int positions = 1;
    private int noFinishPositions = -1;
    private bool endSequenceCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
        swapTime += (float)NetworkTime.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            FinishText.text = $"Player {playerID} has finished the race!";
            playerFinish = true;
            endTimer += (float)NetworkTime.time;
            if (NetworkTime.time >= swapTime)
            {
                display = false;
                swapTime = (float)NetworkTime.time + 3.0f;
            }
        }
        else if (!display)
        {
            FinishText.text = "";
        }

        if (playerFinish)
        {
            FinishTimer.text = "A player has finished the race! The race will end in " + (int)(endTimer - NetworkTime.time) + " seconds!";
            if(NetworkTime.time >= endTimer && endSequenceCalled == false)
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
        if (isServer && collider.CompareTag("Player"))
        {
            NetworkCharacterMovementController netCMC = collider.GetComponent<NetworkCharacterMovementController>();
            if(netCMC.isAI 
                || netCMC.getCheckpointCount() == checkpoints.Length 
                && netCMC.GetHasGrabbed() 
                && netCMC.GetFinishPosition() == 0)
            {
                
                if (!collider.GetComponent<NetworkCharacterMovementController>().isAI)
                {
                    display = true;
                    if (SceneManager.GetActiveScene().name.Equals("NetBlock"))
                        SteamAchievements.UnlockAchievement("SR_NE_ONLINE");
                    else if(SceneManager.GetActiveScene().name.Equals("NetDowntown"))
                        SteamAchievements.UnlockAchievement("SR_DT_ONLINE");
                    mySource.Post(gameObject);
                }
                playerID = collider.GetComponent<NetworkCharacterMovementController>().getPlayerID() + 1;
                collider.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(positions);
                positions++;
            } else
            {
                Debug.LogWarning($"Player {collider.name} failed finish checks! AI: {netCMC.isAI}, allCheckpoints: {netCMC.getCheckpointCount() == checkpoints.Length}, HasGrabbed: {netCMC.GetHasGrabbed()}, FinishPos: {netCMC.GetFinishPosition() == 0}");
            }
        }
    }

    public void UpdatePlayerConfigsList(List<PlayerConfiguration> configs)
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
