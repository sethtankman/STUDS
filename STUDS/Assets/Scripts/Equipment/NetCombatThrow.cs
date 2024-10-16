﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetCombatThrow : NetworkBehaviour
{
    public GameObject knockBack;
    public float knockBackCooldown;

    public void EnableKnockBack()
    {
        if (knockBack)
        {
            StartCoroutine(knockBackTimer());
        }
        else
        {
            Debug.Log("No knockback collider assigned to stroller controller");
        }
    }

    public void EnableKnockBack(string owner)
    {
        if (knockBack)
        {
            knockBack.GetComponent<KnockBack>().owner = owner;
            StartCoroutine(knockBackTimer());
        }
        else
        {
            Debug.Log("No knockback collider assigned to stroller controller");
        }
    }

    private IEnumerator knockBackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        knockBack.SetActive(true);
        Debug.Log("SetActive!");
        yield return new WaitForSeconds(knockBackCooldown); //Knockback enabled for 1.5 seconds when thrown.
        knockBack.GetComponent<KnockBack>().owner = "";
        knockBack.SetActive(false);
    }

}
