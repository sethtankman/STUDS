using System.Collections;
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
            if (other.GetComponent<CharacterMovementController>())
            {
                if (gameObject.GetComponentInParent<StrollerController>())
                {
                    string otherColor = other.gameObject.GetComponent<CharacterMovementController>().GetColorName();
                    string myColor = gameObject.GetComponentInParent<StrollerController>().GetColor();
                    if (otherColor != null && myColor.Equals(otherColor))
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
                Debug.Log("direction: " + direction.ToString() + " Force " + KBForce + " MakePlayerDrop " + makePlayerDrop);

                other.gameObject.GetComponent<CharacterMovementController>().KnockBack(direction * KBForce, makePlayerDrop);
                Instantiate(KnockBackFX, other.transform.position, Quaternion.identity);
                if (!KBSound.Equals("") && other.gameObject.GetComponent<CharacterMovementController>().isAI == false)
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
                Debug.Log("direction: " + direction.ToString() + " Force " + KBForce + " MakePlayerDrop " + makePlayerDrop);

                other.gameObject.GetComponent<NetworkCharacterMovementController>().KnockBack(direction * KBForce, makePlayerDrop);
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
 