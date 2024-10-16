using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Counts the obstacle you were hit by for Achievement purposes.
/// Offline script.
/// </summary>
public class CountObstacleHit : MonoBehaviour
{
    [SerializeField] private int obstacleNum;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.GetComponent<CharacterMovementController>().isAI)
        {
            GameObject.Find("GameManagerStatic").GetComponent<DBGameManager>().GetHitBy(obstacleNum);
        }
    }
}
