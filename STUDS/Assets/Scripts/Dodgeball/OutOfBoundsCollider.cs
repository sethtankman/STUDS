using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsCollider : MonoBehaviour
{
    [SerializeField] private Transform[] Spawns;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.transform.position = Spawns[Random.Range(0, 3)].position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
            DBGameManager.Instance.AddPoints(other.gameObject.GetComponent<CharacterMovementController>().color, 0);
        }
    }
}
