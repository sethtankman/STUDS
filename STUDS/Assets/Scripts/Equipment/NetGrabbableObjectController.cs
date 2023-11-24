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
    [SerializeField] private GameObject localPrefab;
    private GameObject localGO;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_holderTransform">The transform of the holder of this object.</param>
    /// <returns>A reference to the offline copy instantiated on this client.</returns>
    public GameObject LocalPickupObject(Transform _holderTransform)
    {
        holderTransform = _holderTransform;
        Debug.Log("Local Pickup Object");
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

        // Make networked version of object invisible (because it always lags) and spawn a local version instead.
        if (localPrefab)
        {
            GameObject offlineCopy = Instantiate(localPrefab, transform.position, transform.rotation);
            foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = false;
            }
            offlineCopy.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO = gameObject;

            if(GetComponent<NetCart>()) { 
                bool[] isActiveArr = new bool[8];
                int i = 0;
                foreach (GameObject obj in GetComponent<NetCart>().cartItems)
                {
                    isActiveArr[i] = obj.activeSelf;
                    ++i;
                }
                offlineCopy.GetComponentInChildren<LocalCart>().LocalSetActiveItems(isActiveArr);
            }
            if (GetComponent<StrollerController>())
            {
                offlineCopy.GetComponent<StrollerController>().StrollerID = GetComponent<StrollerController>().StrollerID;
                offlineCopy.GetComponent<StrollerController>().DetermineColor(GetComponent<StrollerController>().GetColor());
            }
            localGO = offlineCopy;
            return offlineCopy;
        }
        return null;
    }

    public GameObject LocalLetGo()
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
        
        if (localGO)
        {
            foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = true;
            }
            Destroy(localGO);
        }
         
        return gameObject;
    }

}
