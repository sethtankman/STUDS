using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConnection : MonoBehaviour
{
    public ManagePlayerHub playerHub;

    public Image[] playerImages;

    public void OnEnable()
    {
        playerHub = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>();
        IEnumerator enumerator = playerHub.players.GetEnumerator();
        for(int i = 0; i < 4; i++)
        {
            if(enumerator.MoveNext())
            {
                playerImages[i].color = Color.white;
            }
            else
            {
                playerImages[i].color = Color.grey;
            }

        }
    }

}
