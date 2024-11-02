using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetDBUI : NetworkBehaviour
{
    [SerializeField] private Text[] Scores = new Text[4];
    [SerializeField] private Texture[] Textures = new Texture[5];
    /// <summary>
    ///  Maps placement to color (places 0-3)
    /// </summary>
    private string[] scoreOrder = new string[4];

    /// <summary>
    /// Sets the colors initially of the scoreboard and backend values.
    /// </summary>
    public void UpdateSpriteColors()
    {
        int i = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            scoreOrder[i] = player.GetComponent<NetworkCharacterMovementController>().color;
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
    public string[] UpdateScores(string updatedOwner)
    {
        // Iterate through each <place, name> pair on the scoreOrder dictionary
        // And re-order the scores if someone overtakes someone else.
        for(int i = 0; i< scoreOrder.Length; i++)
        {
            if (scoreOrder[i].Equals(updatedOwner))
                break;
            if (NetDBGameManager.Instance.scores[scoreOrder[i]] < NetDBGameManager.Instance.scores[updatedOwner])
            {
                Queue<string> next = new Queue<string>();
                next.Enqueue(scoreOrder[i]);
                scoreOrder[i] = updatedOwner;
                i++;
                while(i < scoreOrder.Length-1 && scoreOrder[i] != updatedOwner)
                {
                    next.Enqueue(scoreOrder[i]);
                    scoreOrder[i] = next.Dequeue();
                    i++;
                }
                scoreOrder[i] = next.Dequeue();
                break;
            } 
        }
        int[] scoreVals = new int[scoreOrder.Length];
        // Put the updated scores on the screen in order.
        for (int i=0; i < scoreOrder.Length; i++)
        {
            scoreVals[i] = NetDBGameManager.Instance.scores[scoreOrder[i]];
        }
        RpcUpdateScoreboard(scoreOrder, scoreVals); 
        return scoreOrder;
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(string[] _scoreOrder, int[] _scoreVals)
    {
        for (int i = 0; i < scoreOrder.Length; i++)
        {
            Scores[i].text = $"{i + 1}   {_scoreVals[i]}";
            SetSpriteColor(_scoreOrder[i], i);
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
