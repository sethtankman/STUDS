using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Counts the obstacle you were hit by for Achievement purposes.
/// Online script.
/// </summary>
public class NetCountObstacleHit : NetworkBehaviour
{
    [SerializeField] private int obstacleNum;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.GetComponent<NetworkCharacterMovementController>().isLocalPlayer)
        {
            GameObject.Find("GameManagerStatic").GetComponent<NetDBGameManager>().GetHitBy(obstacleNum);
        }
    }
}
