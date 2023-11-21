using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCart : MonoBehaviour
{
    public GameObject[] shoppingItems, cartItems;

    public void LocalSetActiveItems(bool[] isActive)
    {
        for (int i = 0; i < isActive.Length; i++)
        {
            cartItems[i].SetActive(isActive[i]);
        }
    }
}
