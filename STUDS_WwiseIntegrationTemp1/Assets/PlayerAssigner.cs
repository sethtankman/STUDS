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
        if (other.gameObject.tag == "Player")
        {
            if (hasPLRLink == false)
            {
                if (other.GetComponentInChildren<StrollerLocator>().HasLink() == false)
                {
                    LocatorArrows = other.GetComponentInChildren<StrollerLocator>();
                    LocatorArrows.PassStrollerID(this.gameObject);
                    hasPLRLink = true;
                }
            }
        }
    }
}
