using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetRaceStartedDetection : NetworkBehaviour
{
    public AK.Wwise.Event StartSound;

    public GameObject AI1;

    public GameObject AI2;
    
    public GameObject AI3;

    void OnTriggerEnter(Collider obj)
    {
        if (isServer) {
            if (obj.CompareTag("Player"))
            {
                AI1.GetComponent<NetPlayerAI>().StartAI();
                AI2.GetComponent<NetPlayerAI>().StartAI();
                AI3.GetComponent<NetPlayerAI>().StartAI();
                RpcStartSound();
            }
        }
    }

    [ClientRpc]
    public void RpcStartSound()
    {
        StartSound.Post(gameObject);
    }
}
