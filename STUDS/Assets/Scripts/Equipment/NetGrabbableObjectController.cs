using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetGrabbableObjectController : NetworkBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    private Transform holderTransform = null;

    /// <summary>
    /// Movement is server-authoritative, so we update throwable positions in the update method
    /// when they are being held.
    /// </summary>
    private void Update()
    {
        if (isServer && holderTransform)
        {
            transform.position = holderTransform.position + (transform.forward * distance) + (transform.up * height);
            transform.rotation = holderTransform.rotation;
        }
    }

    public void LocalPickupObject(string _holderName)
    {
        holderTransform = GameObject.Find(_holderName).transform;
        //Debug.Log("Local Pickup Object");
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;
        foreach (Collider collider in additionalColliders)
        {
            collider.enabled = false;
        }
        if (GetComponent<ShoppingItem>())
        {
            GetComponent<ShoppingItem>().isBeingHeld = true;
        }
    }

    public void LocalLetGo()
    {
        holderTransform = null;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().enabled = true;
        foreach (Collider collider in additionalColliders)
        {
            collider.enabled = true;
        }
        if (GetComponent<ShoppingItem>())
        {
            GetComponent<ShoppingItem>().isBeingHeld = false;
        }
    }

}
