using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DB_UI : MonoBehaviour
{
    [SerializeField] private Text[] Scores = new Text[4];

    public void UpdateScores()
    {
        int i = 0, sz = 0;
        foreach (KeyValuePair<string, int> pair in DBGameManager.Instance.scores)
        {
            if (i >= sz)
            {
                Scores[i].text = pair.Value.ToString();
                sz++;
            }
            i++;
        }
    }
}
