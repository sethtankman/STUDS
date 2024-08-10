using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetExplodingPropane : CombatThrow
{
    public GameObject explosionEffect;

    public new void EnableKnockBack()
    {
        if (knockBack)
        {
            StartCoroutine(knockBackTimer());
        }
        else
        {
            Debug.LogError("No knockback collider assigned to stroller controller");
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (knockBack.activeInHierarchy)
        {
            SteamAchievements.UnlockAchievement("SS_PROPANE");
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }


    private IEnumerator knockBackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        knockBack.SetActive(true);
        yield return new WaitForSeconds(knockBackCooldown); //Knockback enabled for 1.5 seconds when thrown.
        knockBack.SetActive(false);

    }

}
