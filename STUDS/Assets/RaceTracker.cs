using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RaceTracker : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject>();
    public List<PlaceTracker> PT = new List<PlaceTracker>();

    public Texture BlueIcon;
    public Texture YellowIcon;
    public Texture RedIcon;
    public Texture PinkIcon;
    public Texture OrangeIcon;
    public Texture GreenIcon;

    public RawImage[] Positions;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(GameObject PT in GameObject.FindGameObjectsWithTag("Player")){
            Players.Add(PT);
        }
        if(Players.Count < 4)
        {
            Color invis;
            invis = new Color32(0, 0, 0, 0);
            Positions[3].color = invis;
        }

        for(int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponentInChildren<PlaceTracker>());

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        PT = PT.OrderByDescending(e => e.GetComponentInChildren<PlaceTracker>().Progress).ToList();

        for(int i = 0; i <= Players.Count; i++)
        {
            string PlrColor = PT[i].GetComponentInChildren<PlaceTracker>().PLRCol;

            Positions[i].texture = IconPicker(PlrColor);

        }
        
    }

    private Texture IconPicker(string Color)
    {
        if (Color == "Blue")
        {
            return BlueIcon;
        }
        if (Color == "Red")
        {
            return RedIcon;
        }
        if (Color == "Yellow")
        {
            return YellowIcon;
        }
        if (Color == "Purple")
        {
            return PinkIcon;
        }
        if (Color == "Green")
        {
            return GreenIcon;
        }
        if (Color == "Orange")
        {
            return OrangeIcon;
        }
        return null;
    }

}
