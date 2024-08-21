using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Used to inherit from CombatThrow, now inherits from NetworkBehavior so explosions spawn over network.
/// </summary>
public class NetExplodingPropane : CombatThrow
{
    public GameObject explosionEffect;
    private string ownerName;

    public new void EnableKnockBack()
    {
        if (knockBack)
        {
            StartCoroutine(KnockBackTimer());
        }
        else
        {
            Debug.LogError("No knockback collider assigned to stroller controller");
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (knockBack.activeInHierarchy && !collision.gameObject.name.Equals(ownerName))
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

    public void setOwnerName(string _ownerName)
    {
        ownerName = _ownerName;
    }
}
