using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public Vector3 directionVector;
    public short kBForce;
    public bool makePlayerDrop;
    public AK.Wwise.Event KBSound;
    public bool slidethrough = false, reflective, directional;
    private BoxCollider PaintColl;
    

    private void Start()
    {
        PaintColl = gameObject.GetComponent<BoxCollider>();
        /*
        if(!KBSound.Equals(""))
        {
            //Debug.Log("Searching for : " + KBSound);
            GameObject sfx = GameObject.Find("SFX");
            Transform trans = sfx.transform;
        }
        */
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            if(gameObject.GetComponentInParent<StrollerController>())
            {
                string otherColor = other.gameObject.GetComponent<CharacterMovementController>().GetColorName();
                string myColor = gameObject.GetComponentInParent<StrollerController>().GetColor();
                if(otherColor != null && myColor.Equals(otherColor))
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
                direction += directionVector.normalized;
            }
            //Debug.Log("direction: " + direction.ToString() + " Force " + kBForce + " MakePlayerDrop " + makePlayerDrop);

            other.gameObject.GetComponent<CharacterMovementController>().KnockBack(direction * kBForce, makePlayerDrop);
            if(!KBSound.Equals("") && other.gameObject.GetComponent<CharacterMovementController>().isAI == false)
            {
                KBSound.Post(gameObject);
            }
            if (slidethrough)
            {
                StartCoroutine(ColliderToggle());
            }

        } 
        // This would be used if we were to make the stroller as reactive to knockback as the player, which we don't need to.
        /* else if (other.tag == "Grabbable")
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized + directionVector;
            other.gameObject.GetComponent<StrollerController>().KnockBack(direction * (kBForce), makePlayerDrop);
        } */  
    }

    public void OnCollisionEnter(Collision other)
    {
        // This would be used if we were to make the stroller as reactive to knockback as the player, which we don't need to.
        /*
        if(other.gameObject.tag == "Grabbable")
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized + directionVector;
            other.gameObject.GetComponent<StrollerController>().KnockBack(direction * (kBForce), makePlayerDrop);
        }
        */

        
    }
    IEnumerator ColliderToggle()
    {
        PaintColl.enabled = false;
        yield return new WaitForSeconds(2);
        PaintColl.enabled = true;
        yield return null;
    }
}
 