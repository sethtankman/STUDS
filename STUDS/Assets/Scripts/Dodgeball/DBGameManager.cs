using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBGameManager : MonoBehaviour
{
    public static DBGameManager Instance { get; private set; }
    public DB_UI scorePanel;
    public Dictionary<string, int> scores = new Dictionary<string, int>();

    private void Start()
    {
        Instance = this;

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
        scorePanel.UpdateScores(owner);
    }
}