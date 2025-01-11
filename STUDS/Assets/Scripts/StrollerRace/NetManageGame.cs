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
    private int prevTime = 0;
    private bool endSequenceCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        swapTime += (float)NetworkTime.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (display)
            {
                RpcPlayerFinish($"Player {playerID} has finished the race!");
                if (NetworkTime.time >= swapTime)
                {
                    display = false;
                    swapTime = (float)NetworkTime.time + 3.0f;
                }
            }

            if (playerFinish)
            {
                if (prevTime != (int)(endTimer - NetworkTime.time)) // Reduces network traffic
                {
                    prevTime = (int)(endTimer - NetworkTime.time);
                    RpcEndSequence($"A player has finished the race! The race will end in {prevTime} seconds!");
                }
                if (endTimer - NetworkTime.time <= 0)
                {
                    StudsNetworkManager.singleton.ServerChangeScene("NetVictoryStands");
                    playerFinish = false;
                }
            }
            else
            {
                FinishTimer.text = "";
            }
        } else // Not server
        {
            if (display)
            {
                if (NetworkTime.time >= swapTime)
                {
                    display = false;
                    swapTime = (float)NetworkTime.time + 3.0f;
                }
            }
            if (playerFinish)
            {
                if (endTimer - NetworkTime.time <= 0)
                {
                    playerFinish = false;
                }
            }
            else
            {
                FinishTimer.text = "";
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (isServer)
            {
                NetworkCharacterMovementController netCMC = collider.GetComponent<NetworkCharacterMovementController>();
                if (netCMC.isAI
                    || netCMC.getCheckpointCount() == checkpoints.Length
                    && netCMC.GetHasGrabbed()
                    && netCMC.GetFinishPosition() == 0)
                {
                    if (!netCMC.isAI)
                    {
                        display = true;
                    }
                    playerID = netCMC.getPlayerID() + 1;
                    netCMC.SetFinishPosition(positions);
                    positions++;
                }
                else
                {
                    Debug.LogWarning($"Player {collider.name} failed finish checks! AI: {netCMC.isAI}, allCheckpoints: {netCMC.getCheckpointCount() == checkpoints.Length}, HasGrabbed: {netCMC.GetHasGrabbed()}, FinishPos: {netCMC.GetFinishPosition() == 0}");
                }
            }
            if (collider.GetComponent<NetworkCharacterMovementController>().isLocalPlayer)
            {
                if (SceneManager.GetActiveScene().name.Equals("NetBlock"))
                    SteamAchievements.UnlockAchievement("SR_NE_ONLINE");
                else if (SceneManager.GetActiveScene().name.Equals("NetDowntown"))
                    SteamAchievements.UnlockAchievement("SR_DT_ONLINE");
                mySource.Post(gameObject);
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

    [ClientRpc]
    private void RpcPlayerFinish(string _text)
    {
        FinishText.text = _text;
        playerFinish = true;
        endTimer += (float)NetworkTime.time;
    }

    [ClientRpc]
    private void RpcEndSequence(string _text)
    {
        FinishTimer.text = _text;
        if (endSequenceCalled == false)
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
                if (isServer && player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
                {
                    player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(noFinishPositions);
                    noFinishPositions--;
                }
            }
        }
    }
}
