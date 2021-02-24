using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [SerializeField] private CharacterMovementController myController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && myController.target == null)
        {
            myController.target = other.gameObject;
            var charController = other.gameObject.GetComponent<CharacterMovementController>();
            charController.Blink(true);
            myController.hasAimAssist = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            myController.hasAimAssist = false;
            myController.target = null;
            var charController = other.gameObject.GetComponent<CharacterMovementController>();
            charController.Blink(false);
        }
    }
}
