using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceTracker : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject>();
    public List<PlaceTracker> PT = new List<PlaceTracker>();

    public Texture BlueIcon;
    public Texture YellowIcon;
    public Texture RedIcon;

    public RawImage[] Positions;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(GameObject PT in GameObject.FindGameObjectsWithTag("Player")){
            Players.Add(PT);
        }
        for(int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponentInChildren<PlaceTracker>());

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Positions.Length - 1; i++)
        {
            string PlrColor = Players[i].GetComponent<CharacterMovementController>().GetColorName();
            if (PT[i].Progress > PT[i + 1].Progress)
            {
                
               if(PlrColor == "Blue")
                {
                    Positions[i].texture = BlueIcon;
                }
                if(PlrColor == "Red")
                {
                    Positions[i].texture = RedIcon;
                }if(PlrColor == "Yellow")
                {
                    Positions[i].texture = YellowIcon;
                }

            }
            // Last Place
            if (PlrColor == "Blue")
            {
                Positions[Positions.Length].texture = BlueIcon;
            }
            if (PlrColor == "Red")
            {
                Positions[Positions.Length].texture = RedIcon;
            }
            if (PlrColor == "Yellow")
            {
                Positions[Positions.Length].texture = YellowIcon;
            }
        }
        
    }

}
