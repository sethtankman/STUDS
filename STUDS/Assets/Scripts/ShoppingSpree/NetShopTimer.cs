using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class NetShopTimer : NetworkBehaviour
{
    public SteamAchievements sa;
    public TextMeshProUGUI TimerTXT;
    public GameObject NetworkManager;
    public float timer;
    private float Sprint = 10.0f;

    [SyncVar]
    public int racePositions;
    public int noFinishPos;

    void Start()
    {
        racePositions = 1;
        noFinishPos = -1;
        NetworkManager = GameObject.Find("NetworkManager");
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }

    void Update()
    {
        Mathf.RoundToInt(timer);

        timer -= Time.deltaTime;

        if (timer > 0.0f)
        {
            Showtime(timer);
        }
        else
        {
            if (isServer)
                EndGame();
        }

    }

    void EndGame()
    {

        List<GameObject> players = NetGameManager.Instance.getPlayers();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
            {
                player.GetComponent<NetworkCharacterMovementController>().CmdSetFinishPosition(noFinishPos);
                noFinishPos--;
            }
        }
        sa.UnlockAchievement("SS_FINISH");
        StudsNetworkManager netManager = NetworkManager.GetComponent<StudsNetworkManager>();
        netManager.ServerChangeScene("NetVictoryStands");
    }

    void Showtime(float timeleft)
    {
        float min = Mathf.FloorToInt(timeleft / 60);
        float sec = Mathf.FloorToInt(timeleft % 60);

        if (sec == Sprint && min == 0.0f)
        {
            TimerTXT.fontSize += 10;
            Sprint -= 1.0f;
        }

        TimerTXT.text = string.Format("{0:00}:{1:00}", min, sec);

    }
}
