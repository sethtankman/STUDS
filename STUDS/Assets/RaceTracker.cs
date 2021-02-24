using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceTracker : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject>();
    public List<PlaceTracker> PT = new List<PlaceTracker>();
    public Text PositionTXT;


    // Start is called before the first frame update
    void Awake()
    {
        foreach(GameObject PT in GameObject.FindGameObjectsWithTag("Player")){
            Players.Add(PT);
        }
        for(int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponent<PlaceTracker>());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Players.Count; i++)
        { 
                if (PT[i].Progress > PT[i + 1].Progress)
            {
                PositionTXT.text = i.ToString() + "st";
            }
        }
    }
}
