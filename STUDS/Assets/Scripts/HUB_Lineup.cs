using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUB_Lineup : MonoBehaviour
{
    public Transform[] Positions;
    public int NumJoined = 0;

    private List<GameObject> PlayerList = new List<GameObject>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PlayerList.Contains(other.gameObject))
        {
            other.transform.position = Positions[NumJoined].position;
            other.transform.rotation = Positions[NumJoined].rotation;
            PlayerList.Add(other.gameObject);
            NumJoined += 1;
        }
        
    }
}
