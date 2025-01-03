using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NetRaceTracker : NetworkBehaviour
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

    public void SetColorsGivePlaceTrackers()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            Players.Add(obj);
        }
        if (Players.Count < 4)
        {
            Color invis;
            invis = new Color32(0, 0, 0, 0);
            Positions[3].color = invis;
        }

        for (int i = 0; i < Players.Count; i++)
        {
            PT.Add(Players[i].GetComponentInChildren<NetPlaceTracker>());

        }
        if (isServer)
            StartCoroutine(nameof(UpdatePlacements));
    }

    // Update is called once per frame
    private IEnumerator UpdatePlacements()
    {
        while (true)
        {
            PT = PT.OrderByDescending(e => e.GetComponentInChildren<NetPlaceTracker>().Progress).ToList();

            for (int i = 0; i < Players.Count; i++)
            {
                string PlrColor = PT[i].GetComponentInChildren<NetPlaceTracker>().PLRCol;

                RpcSetIcon(i, PlrColor);

            }
            yield return new WaitForSeconds(1.0f);
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

    [ClientRpc]
    private void RpcSetIcon(int i, string color)
    {
        Positions[i].texture = IconPicker(color);
    }
}
