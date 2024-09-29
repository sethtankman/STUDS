using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetDBGameManager : MonoBehaviour
{
    public static NetDBGameManager Instance { get; private set; }
    public DB_UI scorePanel;
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
        SteamAchievements.UnlockAchievement("DB_COUCH"); // TODO: Make sure this doesn't get copied over to online
    }

    public void InitScores()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            scores[player.GetComponent<CharacterMovementController>().color] = 0;
        }
    }

    public void AddPoints(string owner, int pointValue)
    {
        if (!scores.ContainsKey(owner))
            scores.Add(owner, 0);
        scores[owner] += pointValue;
        scoreOrder = scorePanel.UpdateScores(owner);
        if(scores[owner] >= 15 && winnerFound == false)
        {
            StartCoroutine(EndGame());
        }
    }

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

    private IEnumerator EndGame()
    {
        winnerFound = true;
        yield return new WaitForSeconds(1.0f);
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allPlayers)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                player.transform.parent = null;
                DontDestroyOnLoad(player);
            }
            string color = player.GetComponent<NetworkCharacterMovementController>().GetColorName();
            int placement = Array.IndexOf(scoreOrder, color) + 1;
            if(placement == 4) { placement = -1; } // Since 4th place is represented as -1 in victoryStands.
            player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(placement);
        }
        SteamAchievements.UnlockAchievement("DB_WINNER");
        SceneManager.LoadScene("VictoryStands");
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

    internal List<GameObject> GetAvailableDodgeballs()
    {
        return availableDodgeballs;
    }
}
