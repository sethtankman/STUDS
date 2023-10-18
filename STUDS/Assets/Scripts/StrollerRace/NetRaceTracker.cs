using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NetRaceTracker : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject>();
    public List<NetPlaceTracker> PT = new List<NetPlaceTracker>();

    public Texture BlueIcon;
    public Texture YellowIcon;
    public Texture RedIcon;
    public Texture PinkIcon;
    public Texture OrangeIcon;
    public Texture GreenIcon;

    public RawImage[] Positions;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player")){
            Players.Add(obj);
        }
        if(Players.Count < 4)
        {
            Color invis;
            invis = new Color32(0, 0, 0, 0);
            Positions[3].color = invis;
        }

        for(int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponentInChildren<NetPlaceTracker>());

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        PT = PT.OrderByDescending(e => e.GetComponentInChildren<NetPlaceTracker>().Progress).ToList();

        for(int i = 0; i < Players.Count; i++) 
        {
            if (i >= 4)
            {
                GetComponent<NetDynamicAICount>().FillWithAI();
                break;
            }
            string PlrColor = PT[i].GetComponentInChildren<NetPlaceTracker>().PLRCol;

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
