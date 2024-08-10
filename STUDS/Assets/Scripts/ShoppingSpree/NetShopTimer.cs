using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class NetShopTimer : NetworkBehaviour
{
    public TextMeshProUGUI TimerTXT;
    public GameObject NetworkManager;
    private float Sprint = 10.0f;
    private float serverStartTime;
    private bool gameEnded = false;
    [SerializeField] private float gameTime;
    [SyncVar]
    public int racePositions;
    [SyncVar]
    public int noFinishPos;

    void Start()
    {
        racePositions = 1;
        noFinishPos = -1;
        NetworkManager = GameObject.Find("NetworkManager");
        gameTime += (float)NetworkTime.time;
        serverStartTime = (float)NetworkTime.time;
        
    }

    void Update()
    {
        if (NetworkTime.time < gameTime)
        {
            Showtime();
        }
        else
        {
            if (isServer && !gameEnded)
            {
                EndGame();
                gameEnded = true; // This way we should only call it once.
            }
        }
    }

    void EndGame()
    { 
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
            {
                // TODO: Tally up number of items turned in
                player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(noFinishPos);
                noFinishPos--;
            }
            Destroy(player.GetComponent<SS_ItemTracker>());
        }
        SteamAchievements.UnlockAchievement("SS_ONLINE");
        StudsNetworkManager netManager = NetworkManager.GetComponent<StudsNetworkManager>();
        netManager.ServerChangeScene("NetVictoryStands");
    }

    void Showtime()
    {
        float timeleft = gameTime - (float)NetworkTime.time;
        float min = Mathf.FloorToInt(timeleft / 60);
        float sec = Mathf.FloorToInt(timeleft % 60);

        if (sec == Sprint && min == 0.0f)
        {
            TimerTXT.fontSize += 10;
            Sprint -= 1.0f;
        }

        TimerTXT.text = string.Format("{0:00}:{1:00}", min, sec);

    }

    /// <summary>
    /// Increment race positions. Should only be performed by server.
    /// </summary>
    public void IncrementRacePositions()
    {
        racePositions++;
    }
}
