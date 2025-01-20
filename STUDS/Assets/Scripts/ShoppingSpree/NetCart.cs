using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetCart : NetworkBehaviour
{
    public GameObject[] cartItemTransforms;
    private GameObject[] cartItems;
    private bool cartCooldownActive = false;

    private void Start()
    {
        cartItems = new GameObject[cartItemTransforms.Length];
    }

    public uint GiveObject(Transform owner)
    {
        if (isServer && cartCooldownActive == false)
        {
            for (int i = 0; i < cartItemTransforms.Length; i++)
            {
                if (cartItemTransforms[i].activeSelf)
                {
                    cartItemTransforms[i].SetActive(false);
                    RpcSetItemActive(i, false);
                    GameObject item = cartItems[i];
                    item.transform.parent = null;
                    item.AddComponent<Rigidbody>();
                    cartItems[i] = null;
                    return item.GetComponent<NetworkIdentity>().netId;
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
                int index = -1;
                switch (itemName)
                {
                    case "Boombox":
                        if (cartItemTransforms[0].activeSelf == false)
                        {
                            index = 0;
                        }
                        break;
                    case "Helmet":
                        if (cartItemTransforms[1].activeSelf == false)
                        {
                            index = 1;
                        }
                        break;
                    case "Cooler":
                        if (cartItemTransforms[2].activeSelf == false)
                        {
                            index = 2;
                        }
                        break;
                    case "Hammer":
                        if (cartItemTransforms[3].activeSelf == false)
                        {
                            index = 3;
                        }
                        break;
                    case "Shovel":
                        if (cartItemTransforms[4].activeSelf == false)
                        {
                            index = 4;
                        }
                        break;
                    case "Sprinkler":
                        if (cartItemTransforms[5].activeSelf == false)
                        {
                            index = 5;
                        }
                        break;
                    case "Toolbox":
                        if (cartItemTransforms[6].activeSelf == false)
                        {
                            index = 6;
                        }
                        break;
                    case "Vacuum":
                        if (cartItemTransforms[7].activeSelf == false)
                        {
                            index = 7;
                        }
                        break;
                    default:
                        Debug.LogError("Item name not found.");
                        break;
                }
                cartItemTransforms[index].SetActive(true);
                RpcSetItemActive(index, true);
                other.transform.parent = cartItemTransforms[index].transform;
                other.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                cartItems[index] = other.gameObject;
                other.GetComponent<NetGrabbableObjectController>().AddToCart();
                Destroy(other.GetComponent<Rigidbody>());
                cartCooldownActive = true;
                Invoke("ResetCartCooldown", 0.6f);
            }
        }
    }

    private void ResetCartCooldown() { cartCooldownActive = false; }

    [ClientRpc]
    private void RpcSetItemActive(int num, bool isActive)
    {
        if (isServer)
            return;
        cartItemTransforms[num].SetActive(isActive);
    }
}
