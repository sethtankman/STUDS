using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountObstacleHit : MonoBehaviour
{
    [SerializeField] private int obstacleNum;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.GetComponent<CharacterMovementController>().isAI)
        {
            GameObject.Find("GameManager").GetComponent<DBGameManager>().GetHitBy(obstacleNum);
        }
    }
}
