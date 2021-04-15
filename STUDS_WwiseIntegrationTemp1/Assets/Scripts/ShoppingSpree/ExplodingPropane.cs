using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPropane : CombatThrow
{
    public GameObject explosionEffect;

    public new void EnableKnockBack() //Vector3 direction, bool dropStroller)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (knockBack.activeInHierarchy)
        {
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
