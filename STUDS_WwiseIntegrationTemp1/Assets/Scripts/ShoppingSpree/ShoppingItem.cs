using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItem : MonoBehaviour
{
    private GameObject player;

    public bool isBeingHeld = true;

    public string name;

    public void SetPlayer(GameObject player) 
    {
        this.player = player;
        //isBeingHeld = true;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
