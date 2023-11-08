using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGrabbableObjectController : MonoBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    public GameObject networkedGO;

    public GameObject LocalPickupObject(string _holderName)
    {
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

        // Make networked version of object invisible (because it always lags) and spawn a local version instead.
        if (networkedGO)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GameObject offlineCopy = Instantiate(networkedGO, transform.position, transform.rotation);
            return offlineCopy;
        }
        return null;
    }

    /// <summary>
    /// Destroys this local version of the gameobject once we let go.
    /// </summary>
    public void LocalLetGo()
    {
        networkedGO.GetComponent<NetGrabbableObjectController>().LocalLetGo();
        Destroy(gameObject);
    }

}
