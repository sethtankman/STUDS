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
    private int dbNum = 0;

    private void Start()
    {
        Instance = this;
        availableDodgeballs = new List<GameObject>();
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
        if(scores[owner] >= 100)
        {
            EndGame();
        }
    }

    public void deListDodgeball(GameObject dodgeball)
    {
        availableDodgeballs.Remove(dodgeball);
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
            player.GetComponent<CharacterMovementController>().SetFinishPosition(Array.IndexOf(scoreOrder, color));
        }

        SceneManager.LoadScene("VictoryStands");
    }

    internal List<GameObject> GetAvailableDodgeballs()
    {
        return availableDodgeballs;
    }
}
