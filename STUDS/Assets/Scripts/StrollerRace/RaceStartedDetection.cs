using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStartedDetection : MonoBehaviour
{
    public AK.Wwise.Event StartSound;

    public GameObject AI1;

    public GameObject AI2;
    
    public GameObject AI3;

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag.Equals("Player"))
        {
            AI1.GetComponent<PlayerAI>().StartAI();
            AI2.GetComponent<PlayerAI>().StartAI();
            AI3.GetComponent<PlayerAI>().StartAI();

            StartSound.Post(gameObject);
        }

    }
}
