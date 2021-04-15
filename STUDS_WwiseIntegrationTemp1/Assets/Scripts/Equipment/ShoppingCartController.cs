using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingCartController : MonoBehaviour
{
    public GameObject[] shoppingItems, cartItems;

    public GameObject GiveObject()
    {
        for(int i = 0; i < cartItems.Length; i++)
        {
            if (cartItems[i].activeSelf)
            {
                cartItems[i].SetActive(false);
                return shoppingItems[i];
            }
        }
        Debug.Log("There's nothing on the cart, dummy!");
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ShoppingItem" && other.GetComponent<ShoppingItem>().isBeingHeld == false)
        {
            string itemName = other.gameObject.GetComponent<ShoppingItem>().name;
            switch (itemName) {
                case "Boombox":
                    if (cartItems[0].activeSelf == false)
                    {
                        cartItems[0].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Helmet":
                    if (cartItems[1].activeSelf == false)
                    {
                        cartItems[1].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Cooler":
                    if (cartItems[2].activeSelf == false)
                    {
                        cartItems[2].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Hammer":
                    if (cartItems[3].activeSelf == false)
                    {
                        cartItems[3].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Shovel":
                    if (cartItems[4].activeSelf == false)
                    {
                        cartItems[4].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Sprinkler":
                    if (cartItems[5].activeSelf == false)
                    {
                        cartItems[5].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Toolbox":
                    if (cartItems[6].activeSelf == false)
                    {
                        cartItems[6].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                case "Vacuum":
                    if (cartItems[7].activeSelf == false)
                    {
                        cartItems[7].SetActive(true);
                        Destroy(other.gameObject);
                    }
                    break;
                default:
                    Debug.Log("Item name not found.");
                    break;
            }
                
        }
    }
}
