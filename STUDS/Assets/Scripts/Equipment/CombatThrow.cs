﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatThrow : MonoBehaviour
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
            Debug.LogError("No knockback collider assigned.");
        }
    }

    private IEnumerator knockBackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        knockBack.SetActive(true);
        yield return new WaitForSeconds(knockBackCooldown); //Knockback enabled for 1.5 seconds when thrown.
        knockBack.GetComponent<KnockBack>().owner = "";
        knockBack.SetActive(false);
    }

}
