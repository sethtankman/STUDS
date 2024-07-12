using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetCart : NetworkBehaviour
{
    public GameObject[] shoppingItems, cartItems;

    public uint GiveObject(Transform owner)
    {
        if (isServer)
        {
            for (int i = 0; i < cartItems.Length; i++)
            {
                if (cartItems[i].activeSelf)
                {
                    cartItems[i].SetActive(false);
                    RpcSetItemActive(i, false); 
                    GameObject shoppingItem = Instantiate(shoppingItems[i], owner.position + (owner.forward * 1.3f) + (owner.up * 0.7f), Quaternion.identity);
                    NetworkServer.Spawn(shoppingItem);
                    return shoppingItem.GetComponent<NetworkIdentity>().netId;
                }
            }
            Debug.Log("There's nothing on the cart, dummy!");
        }
        return 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.CompareTag("ShoppingItem") && other.GetComponent<ShoppingItem>()
                && other.GetComponent<ShoppingItem>().isBeingHeld == false)
            {
                string itemName = other.gameObject.GetComponent<ShoppingItem>().name;
                switch (itemName)
                {
                    case "Boombox":
                        if (cartItems[0].activeSelf == false)
                        {
                            cartItems[0].SetActive(true);
                            RpcSetItemActive(0, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Helmet":
                        if (cartItems[1].activeSelf == false)
                        {
                            cartItems[1].SetActive(true);
                            RpcSetItemActive(1, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Cooler":
                        if (cartItems[2].activeSelf == false)
                        {
                            cartItems[2].SetActive(true);
                            RpcSetItemActive(2, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Hammer":
                        if (cartItems[3].activeSelf == false)
                        {
                            cartItems[3].SetActive(true);
                            RpcSetItemActive(3, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Shovel":
                        if (cartItems[4].activeSelf == false)
                        {
                            cartItems[4].SetActive(true);
                            RpcSetItemActive(4, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Sprinkler":
                        if (cartItems[5].activeSelf == false)
                        {
                            cartItems[5].SetActive(true);
                            RpcSetItemActive(5, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Toolbox":
                        if (cartItems[6].activeSelf == false)
                        {
                            cartItems[6].SetActive(true);
                            RpcSetItemActive(6, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    case "Vacuum":
                        if (cartItems[7].activeSelf == false)
                        {
                            cartItems[7].SetActive(true);
                            RpcSetItemActive(7, true);
                            NetworkServer.Destroy(other.gameObject);
                        }
                        break;
                    default:
                        Debug.LogError("Item name not found.");
                        break;
                }

            }
        }
    }

    [ClientRpc]
    private void RpcSetItemActive(int num, bool isActive)
    {
        if (isServer)
            return;
        cartItems[num].SetActive(isActive);
    }
}
