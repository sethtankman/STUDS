using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIThrowTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponentInParent<CharacterMovementController>().performThrow();
        }
    }
}
