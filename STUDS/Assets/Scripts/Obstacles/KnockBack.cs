﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public Vector3 directionVector;
    public short KBForce;
    public bool makePlayerDrop;
    public AK.Wwise.Event KBSound;
    public bool slidethrough = false, reflective, directional;
    private Collider PaintColl;
    public string owner;

    public GameObject KnockBackFX;

    public bool autoDisableKnockback = false;
    public float disableDelay = 1.0f;
    

    private void Start()
    {
        PaintColl = gameObject.GetComponent<BoxCollider>();
        if(PaintColl == null)
        {
            PaintColl = gameObject.GetComponent<MeshCollider>();
        }
        if (autoDisableKnockback)
        {
            StartCoroutine(DisableKnockbackAfterTime());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<CharacterMovementController>())
            {
                if (GetComponentInParent<StrollerController>())
                {
                    string otherColor = other.GetComponent<CharacterMovementController>().GetColorName();
                    string myColor = gameObject.GetComponentInParent<StrollerController>().GetColor();
                    if (otherColor != null && myColor.Equals(otherColor))
                    {
                        return;
                    }
                }
                else if (GetComponentInParent<GrabbableObjectController>())
                {
                    GetComponentInParent<GrabbableObjectController>().HomingThrow(false, null, Vector3.zero);
                    if (GetComponentInParent<GrabbableObjectController>().throwerColor == other.GetComponent<CharacterMovementController>().GetColorName())
                    {
                        return;
                    }
                }
                Vector3 direction = Vector3.zero; //Temporarily set.
                if (reflective)
                {
                    direction += (other.transform.position - transform.position).normalized;
                }
                if (directional)
                {
                    directionVector = directionVector.normalized;
                    direction.x *= 1 - directionVector.x;
                    direction.y *= 1 - directionVector.y;
                    direction.z *= 1 - directionVector.z;
                    direction += directionVector.normalized;
                }

                other.GetComponent<CharacterMovementController>().KnockBack(direction * KBForce, makePlayerDrop);
                if(KnockBackFX)
                    Instantiate(KnockBackFX, other.transform.position, Quaternion.identity);
                if (GetComponent<HitScore>())
                    GetComponent<HitScore>().RecordHit(GetComponentInParent<GrabbableObjectController>().throwerColor);
                if (!KBSound.Equals(""))
                {
                    KBSound.Post(gameObject);
                }
                if (slidethrough)
                {
                    StartCoroutine(ColliderToggle());
                }
            }
            else if (other.GetComponent<NetworkCharacterMovementController>())
            {
                // Cancel hit if your own item hits you (network only)
                if (owner.Equals(other.name))
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
                if (!KBSound.Equals(""))
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

    private IEnumerator DisableKnockbackAfterTime()
    {
        yield return new WaitForSeconds(disableDelay);

       SphereCollider sphereCollider = GetComponent<SphereCollider>();
       if (sphereCollider != null)
       {
           sphereCollider.enabled = false;
       }
       else
       {
           Debug.Log("No sphere collider found");
       }
    }
}
 