﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetExplodingPropane : CombatThrow
{
    public GameObject explosionEffect;
    private string ownerName;
    private bool live;

    public new void EnableKnockBack()
    {
        live = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (live && !collision.gameObject.name.Equals(ownerName))
        {
            if (GetComponent<NetGrabbableObjectController>().isServer) {
                GetComponent<NetGrabbableObjectController>().PropaneExplode(explosionEffect);
            }
        }
    }


    private IEnumerator KnockBackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        knockBack.SetActive(true);
        yield return new WaitForSeconds(knockBackCooldown); //Knockback enabled for 1.5 seconds when thrown.
        knockBack.SetActive(false);

    }

    public void SetOwnerName(string _ownerName)
    {
        ownerName = _ownerName;
    }
}
