using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DB_UI : MonoBehaviour
{
    [SerializeField] private Text[] Scores = new Text[4];
    [SerializeField] private Texture[] Textures = new Texture[5];
    private Dictionary<int, string> scoreOrder = new Dictionary<int, string>();

    /// <summary>
    /// Sets the colors initially of the scoreboard and backend values.
    /// </summary>
    public void UpdateSpriteColors()
    {
        int i = 0;
        foreach (GameObject player in ManagePlayerHub.Instance.players)
        {
            scoreOrder.Add(i, player.GetComponent<CharacterMovementController>().color);
            Scores[i].text = $"{i+1}   0";
            SetSpriteColor(scoreOrder[i], i);
            i++;
        }
    }

    /// <summary>
    /// Updates the scoreOrder dictionary for the score display.
    /// PRE: Scores should have been updated in DBGameManager.Instance.scores before this.
    /// POST: Arranges scores in order in the scoreOrder dictionary.
    /// </summary>
    /// <param name="updatedOwner">The name of the person who increased their score</param>
    public void UpdateScores(string updatedOwner)
    {
        int i = 0;
        // Iterate through each <place, name> pair on the scoreOrder dictionary
        // And re-order the scores if someone overtakes someone else.
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
        // Put the updated scores on the screen in order.
        while (i < DBGameManager.Instance.scores.Count)
        {
            Scores[i].text = $"{i+1}   {DBGameManager.Instance.scores[scoreOrder[i]]}";
            SetSpriteColor(scoreOrder[i], i);
            i++;
        }
    }

    private void SetSpriteColor(string _color, int _i)
    {
        switch (_color)
        {
            case "red":
                Scores[_i].GetComponentInParent<RawImage>().texture = Textures[0];
                break;
            case "yellow":
                Scores[_i].GetComponentInParent<RawImage>().texture = Textures[1];
                break;
            case "green":
                Scores[_i].GetComponentInParent<RawImage>().texture = Textures[2];
                break;
            case "blue":
                Scores[_i].GetComponentInParent<RawImage>().texture = Textures[3];
                break;
            case "purple":
                Scores[_i].GetComponentInParent<RawImage>().texture = Textures[4];
                break;
        }
 
    }
}
