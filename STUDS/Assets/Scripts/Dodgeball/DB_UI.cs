using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DB_UI : MonoBehaviour
{
    [SerializeField] private Text[] Scores = new Text[4];
    private Dictionary<int, string> scoreOrder = new Dictionary<int, string>();

    private void Start()
    {
        int i = 0;
        foreach (GameObject player in ManagePlayerHub.Instance.players)
        {
            scoreOrder.Add(i, player.GetComponent<CharacterMovementController>().color);
            i++;
        }
    }

    public void UpdateScores(string updatedOwner)
    {
        int i = 0;
        foreach (KeyValuePair<int, string> pair in scoreOrder)
        {
            if (pair.Value.Equals(updatedOwner))
                break;
            if (DBGameManager.Instance.scores[pair.Value] < DBGameManager.Instance.scores[updatedOwner])
            {
                Queue<string> next = new Queue<string>();
                next.Enqueue(pair.Value);
                scoreOrder[pair.Key] = updatedOwner;
                i = pair.Key + 1;
                while(i < DBGameManager.Instance.scores.Count && scoreOrder[i] != updatedOwner)
                {
                    next.Enqueue(scoreOrder[i + 1]);
                    scoreOrder[i] = next.Dequeue();
                    i++;
                }
                scoreOrder[i] = next.Dequeue();
                break;
            } 
            i++;
        }
        i = 0;
        while (i < DBGameManager.Instance.scores.Count)
        {
            Scores[i].text = $"{scoreOrder[i]}: {DBGameManager.Instance.scores[scoreOrder[i]]}";
            i++;
        }
    }
}
