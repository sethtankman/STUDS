using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Trigger for Dodgeball AI.  Throws an object when a player enters the aimAssist trigger.
/// </summary>
public class AIThrowTrigger : MonoBehaviour
{
    private bool canThrow = false;

    private void OnTriggerEnter(Collider other)
    {
        if(canThrow && other.CompareTag("Player"))
        {
            GetComponentInParent<CharacterMovementController>().target = other.gameObject;
            GetComponentInParent<CharacterMovementController>().performThrow();
            GetComponentInParent<DodgeballAI>().Loiter();
        }
    }

    public void setCanThrow(bool tf)
    {
        canThrow = tf;
    }
}
