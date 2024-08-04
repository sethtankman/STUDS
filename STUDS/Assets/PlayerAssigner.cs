using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAssigner : MonoBehaviour
{
    public bool hasPLRLink = false;
    private StrollerLocator LocatorArrows;

    //private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkCharacterMovementController>().inStrollerRace && hasPLRLink == false)
            {
                LocatorArrows = other.GetComponentInChildren<StrollerLocator>();
                if (LocatorArrows && LocatorArrows.HasLink() == false)
                {
                    LocatorArrows.PassStrollerID(this.gameObject);
                    hasPLRLink = true;
                }
            }
        }
    }
}
