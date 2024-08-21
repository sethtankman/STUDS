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
    private GameObject holder;
    [SerializeField] private GameObject localPrefab;
    private GameObject localGO;
    [SyncVar] private bool canPickup = true;

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

    public void OnDestroy()
    {
        Destroy(localGO);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_holderTransform">The transform of the holder of this object.</param>
    /// <returns>A reference to the offline copy instantiated on this client.</returns>
    public GameObject LocalPickupObject(Transform _holderTransform)
    {
        canPickup = false;
        holderTransform = _holderTransform;
        holder = _holderTransform.gameObject;
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
        if (GetComponent<NetExplodingPropane>())
        {
            GetComponent<NetExplodingPropane>().setOwnerName(holder.name);
            SteamAchievements.UnlockAchievement("SS_PROPANE");
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
            } else if (GetComponent<StrollerController>()) // The else if is necessary since shopping carts also have StrollerControllers.
            {
                offlineCopy.GetComponent<StrollerController>().StrollerID = GetComponent<StrollerController>().StrollerID;
                offlineCopy.GetComponent<StrollerController>().DetermineColor(GetComponent<StrollerController>().GetColor());
            }
            localGO = offlineCopy;
            return offlineCopy;
        }
        return null;
    }

    /// <summary>
    /// Erases holder transform, deals with collisions and physics, re-enables colliders, 
    /// sets isbeingheld to false on shopping item, linear interpolates from local GO to net GO.
    /// </summary>
    /// <returns>A reference to this gameobject</returns>
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
            GameObject lerpGO = Instantiate(localGO, localGO.transform.position, localGO.transform.rotation);
            //stop rendering the localGO
            foreach (MeshRenderer rend in localGO.GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = false;
            }
            //Create a new localGO
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().SetLocalT(localGO.transform);
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().SetLerp(true);
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO = gameObject;
        }
         
        return gameObject;
    }

    public void RenderNetworkedGO()
    {
        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.enabled = true;
        }
        Destroy(localGO);
    }

    public void PropaneExplode(GameObject particleEffect)
    {

        GameObject effect = Instantiate(particleEffect, transform.position, Quaternion.identity);
        NetworkServer.Spawn(effect, holder);
        Destroy(gameObject);
    }

    public bool GetCanPickup()
    {
        return canPickup;
    }

    public void SetCanPickup(bool tf)
    {
        canPickup = tf;
    }
}
