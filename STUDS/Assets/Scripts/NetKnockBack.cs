using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetKnockBack : NetworkBehaviour
{
    public Vector3 directionVector;
    public short KBForce;
    public bool makePlayerDrop;
    public AK.Wwise.Event KBSound;
    public bool slidethrough = false, reflective, directional;
    private Collider PaintColl;
    public string owner;
    [SerializeField]
    private bool clientAuthority;

    public GameObject KnockBackFX;
    

    private void Start()
    {
        PaintColl = gameObject.GetComponent<BoxCollider>();
        if(PaintColl == null)
        {
            PaintColl = gameObject.GetComponent<MeshCollider>();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkCharacterMovementController netCMC = other.GetComponent<NetworkCharacterMovementController>();
            if (netCMC)
            {
                // Cancel hit if your own item hits you (network only)
                if (owner.Equals(other.name) 
                    || (clientAuthority && // In client authority mode, ...
                        (netCMC.isAI && !isServer) // ai outside the server cannot be hit
                        || (netCMC.isAI == false && isServer && !netCMC.isLocalPlayer) // nonlocal players on the server cannot be hit.
                    )
                    || (!clientAuthority && !isServer) // In server authority mode, don't collide outside server.

                ) 
                {
                    return;
                }


                Vector3 direction = Vector3.zero; //Temporarily set.
                if (reflective)
                {
                    direction += (other.transform.position - transform.position).normalized;
                }
                if (directional)
                {
                    direction += directionVector.normalized;
                }

                other.gameObject.GetComponent<NetworkCharacterMovementController>().KnockBack(direction * KBForce, makePlayerDrop);

                if (KnockBackFX)
                    Instantiate(KnockBackFX, other.transform.position, Quaternion.identity); 
                if (GetComponent<HitScore>())
                    GetComponent<HitScore>().RecordHit(GetComponentInParent<NetGrabbableObjectController>().throwerColor);
                if (!KBSound.Equals("") && other.gameObject.GetComponent<NetworkCharacterMovementController>().isAI == false)
                {
                    KBSound.Post(gameObject);
                }
                if (slidethrough)
                {
                    StartCoroutine(ColliderToggle());
                }
            }
        }
    }

    IEnumerator ColliderToggle()
    {
        PaintColl.enabled = false;
        yield return new WaitForSeconds(2);
        PaintColl.enabled = true;
        yield return null;
    }
}
 