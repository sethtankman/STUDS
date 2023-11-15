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
            GetComponent<MeshRenderer>().enabled = false;
            GameObject offlineCopy = Instantiate(localPrefab, transform.position, transform.rotation);
            offlineCopy.GetComponent<LocalGrabbableObjectController>().networkedGO = gameObject;
            localGO = offlineCopy;
            return offlineCopy;
        }
        return null;
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
        if (localGO) {
            GetComponent<MeshRenderer>().enabled = true;
            Destroy(localGO);
        }
    }

}
