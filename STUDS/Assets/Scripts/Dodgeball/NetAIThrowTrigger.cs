using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Trigger for Dodgeball AI.  Throws an object when a player enters the aimAssist trigger.
/// </summary>
public class NetAIThrowTrigger : MonoBehaviour
{
    private bool canThrow = false;

    private void OnTriggerEnter(Collider other)
    {
        if(canThrow && other.CompareTag("Player"))
        {
            GetComponentInParent<NetworkCharacterMovementController>().target = other.gameObject;
            GetComponentInParent<NetworkCharacterMovementController>().hasAimAssist = true;
            GetComponentInParent<NetworkCharacterMovementController>().performThrow();
            GetComponentInParent<NetDodgeballAI>().Loiter(false);
        }
    }

    public void setCanThrow(bool tf)
    {
        canThrow = tf;
    }
}
