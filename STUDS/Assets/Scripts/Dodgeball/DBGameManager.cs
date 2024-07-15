using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DBGameManager : MonoBehaviour
{
    public static DBGameManager Instance { get; private set; }
    public DB_UI scorePanel;
    /// Maps color to score
    public Dictionary<string, int> scores = new Dictionary<string, int>();
    [SerializeField] private List<GameObject> availableDodgeballs;
    [SerializeField] private string[] scoreOrder;
    [SerializeField] private GameObject[] players;
    private int dbNum = 0;

    private void Start()
    {
        Instance = this;
        availableDodgeballs = new List<GameObject>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void InitScores()
    {
        foreach (GameObject player in ManagePlayerHub.Instance.getPlayers())
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
        if(scores[owner] >= 50)
        {
            EndGame();
        }
    }

    public void deListDodgeball(GameObject dodgeball)
    {
        availableDodgeballs.Remove(dodgeball);
        foreach (GameObject player in players)
        {
            if(player.GetComponent<CharacterMovementController>().isAI && player.GetComponent<DodgeballAI>().CompareTarget(dodgeball))
            {
                player.GetComponent<DodgeballAI>().Loiter(false);
            }
        }
    }

    public void deListDodgeball(GameObject dodgeball, GameObject taker)
    {
        availableDodgeballs.Remove(dodgeball);
        foreach (GameObject player in players)
        {
            if (player.GetComponent<CharacterMovementController>().isAI && player.GetComponent<DodgeballAI>().CompareTarget(dodgeball))
            {
                if(taker.name != player.name)
                    player.GetComponent<DodgeballAI>().Loiter(false);
            }
        }
    }

    public void enlistDodgeball(GameObject dodgeball)
    {
        dbNum++;
        dodgeball.name = dodgeball.name + dbNum.ToString();
        availableDodgeballs.Add(dodgeball);
    }

    private void EndGame()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allPlayers)
        {
            if (player.GetComponent<CharacterMovementController>().isAI)
            {
                player.transform.parent = null;
                DontDestroyOnLoad(player);
            }
            string color = player.GetComponent<CharacterMovementController>().GetColorName();
            int placement = Array.IndexOf(scoreOrder, color) + 1;
            if(placement == 4) { placement = -1; } // Since 4th place is represented as -1 in victoryStands.
            player.GetComponent<CharacterMovementController>().SetFinishPosition(placement);
        }

        SceneManager.LoadScene("VictoryStands");
    }

    internal List<GameObject> GetAvailableDodgeballs()
    {
        return availableDodgeballs;
    }
}
