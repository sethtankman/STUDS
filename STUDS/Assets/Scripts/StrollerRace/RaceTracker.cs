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
        for(int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponentInChildren<PlaceTracker>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        PT = PT.OrderByDescending(e => e.GetComponentInChildren<PlaceTracker>().Progress).ToList();

        for(int i = 0; i < Players.Count; i++)
        {
            string PlrColor = PT[i].GetComponentInChildren<PlaceTracker>().PLRCol;

            Positions[i].texture = IconPicker(PlrColor);

        }
        
    }

    private Texture IconPicker(string Color)
    {
        if (Color == "blue")
        {
            return BlueIcon;
        }
        if (Color == "red")
        {
            return RedIcon;
        }
        if (Color == "yellow")
        {
            return YellowIcon;
        }
        if (Color == "purple")
        {
            return PinkIcon;
        }
        if (Color == "green")
        {
            return GreenIcon;
        }
        if (Color == "orange")
        {
            return OrangeIcon;
        }
        return null;
    }

}
