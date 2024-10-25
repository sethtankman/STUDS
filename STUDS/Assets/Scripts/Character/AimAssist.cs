using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets CMC or NetCMC's target and hasAimAssist values.
/// </summary>
public class AimAssist : MonoBehaviour
{
    [SerializeField] private CharacterMovementController myController;
    [SerializeField] private NetworkCharacterMovementController myNetworkController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (myController && myController.target == null)
            {
                myController.target = other.gameObject;
                myController.hasAimAssist = true;
            }
            else if (myNetworkController.target == null)
            {
                myNetworkController.target = other.gameObject;
                myNetworkController.hasAimAssist = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (myController)
            {
                myController.hasAimAssist = false;
                myController.target = null;
            } else
            {
                myNetworkController.hasAimAssist = false;
                myNetworkController.target = null;
            }
        }
    }
}
