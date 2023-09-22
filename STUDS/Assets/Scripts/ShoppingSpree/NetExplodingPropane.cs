using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetExplodingPropane : CombatThrow
{
    public GameObject explosionEffect;
    public SteamAchievements sa;

    private void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }

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
            //sa.UnlockAchievement("SS_PROPANE"); // TODO: Add this back in?
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
