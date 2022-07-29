using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetHUB_Lineup : NetworkBehaviour
{
    public Transform[] Positions;
    public int NumJoined = 0;

    private List<GameObject> PlayerList = new List<GameObject>();


    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.CompareTag("Player") && !PlayerList.Contains(other.gameObject))
            {
                MovePlayer(other.gameObject, NumJoined);
                PlayerList.Add(other.gameObject);
                NumJoined += 1;
            }
        }
        
    }

    [ClientRpc]
    private void MovePlayer(GameObject other, int _numJoined)
    {
        other.transform.position = Positions[_numJoined].position;
        other.transform.rotation = Positions[_numJoined].rotation;
    }
}
