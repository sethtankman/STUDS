using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetDBGameManager : NetworkBehaviour
{
    public static NetDBGameManager Instance { get; private set; }
    public NetDBUI scorePanel;
    /// Maps color to score
    public Dictionary<string, int> scores = new Dictionary<string, int>();
    [SerializeField] private List<GameObject> availableDodgeballs;
    [SerializeField] private string[] scoreOrder;
    [SerializeField] private GameObject[] players;
    [SerializeField] private bool[] hitByArr;
    private int dbNum = 0;
    private bool winnerFound = false;

    private void Start()
    {
        Instance = this;
        availableDodgeballs = new List<GameObject>();
        players = GameObject.FindGameObjectsWithTag("Player");
        SteamAchievements.UnlockAchievement("DB_ONLINE"); 
    }

    public void InitScores()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            scores[player.GetComponent<NetworkCharacterMovementController>().color] = 0;
        }
    }

    /// <summary>
    /// Only has functionality on the Server.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="pointValue"></param>
    public void AddPoints(string owner, int pointValue)
    {
        if (!isServer)
            return;
        if (!scores.ContainsKey(owner))
            scores.Add(owner, 0);
        scores[owner] += pointValue;
        scoreOrder = scorePanel.UpdateScores(owner);
        if(scores[owner] >= 15 && winnerFound == false)
        {
            RpcEndGame();
        }
    }

    /// <summary>
    /// PRE: This method should only be run on the Server.
    /// POST: Removes dodgeball availability for AI and tells them to loiter if that was their target.
    /// </summary>
    /// <param name="dodgeball">Dodgeball being delisted.</param>
    public void deListDodgeball(GameObject dodgeball)
    {
        availableDodgeballs.Remove(dodgeball);
        foreach (GameObject player in players)
        {
            if(player.GetComponent<NetworkCharacterMovementController>().isAI && player.GetComponent<NetDodgeballAI>().CompareTarget(dodgeball))
            {
                player.GetComponent<NetDodgeballAI>().Loiter(false);
            }
        }
    }

    /// <summary>
    /// PRE: This method should only be run on the Server.
    /// POST: Removes dodgeball availability for AI and tells them to loiter if that was their target.
    /// </summary>
    /// <param name="dodgeball">Dodgeball being delisted.</param>
    /// <param name="taker">GameObject taking the dodgeball.</param>
    public void deListDodgeball(GameObject dodgeball, GameObject taker)
    {
        availableDodgeballs.Remove(dodgeball);
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().isAI && player.GetComponent<NetDodgeballAI>().CompareTarget(dodgeball))
            {
                if(taker.name != player.name)
                    player.GetComponent<NetDodgeballAI>().Loiter(false);
            }
        }
    }

    public void enlistDodgeball(GameObject dodgeball)
    {
        dbNum++;
        dodgeball.name = dodgeball.name + dbNum.ToString();
        availableDodgeballs.Add(dodgeball);
    }

    /// <summary>
    /// Called by RPCEndGame after 1s delay.
    /// </summary>
    private void EndGame()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allPlayers)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                player.transform.parent = null;
                DontDestroyOnLoad(player); // Players need to persist into the next scene to load their data.
            }
            string color = player.GetComponent<NetworkCharacterMovementController>().GetColorName();
            int placement = Array.IndexOf(scoreOrder, color) + 1;
            if (placement == 4) { placement = -1; } // Since 4th place is represented as -1 in victoryStands.
            player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(placement);
        }
        FindObjectOfType<NetworkManager>().ServerChangeScene("NetVictoryStands");
    }

    public void GetHitBy(int obstacleIndex)
    {
        hitByArr[obstacleIndex] = true;
        for(int i = 0; i < hitByArr.Length; i++)
        {
            if (hitByArr[i] == false)
                return;
        }
        SteamAchievements.UnlockAchievement("DB_OUCH");
    }

    [ClientRpc]
    private void RpcEndGame()
    {
        winnerFound = true;
        Invoke("EndGame", 1.0f);
    }

    internal List<GameObject> GetAvailableDodgeballs()
    {
        return availableDodgeballs;
    }
}
