using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObjectController : MonoBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    public GameObject throwableArrowMedium;
    public GameObject throwableArrowHeavy;
    public GameObject throwableArrowLight;
    public GameObject target;
    public bool isDodgeball = false;
    private bool homing = false, dirMagCaptured = false;
    private float ogDirMag = 0;

    private void Update()
    {
        if (homing)
        {
            if (!dirMagCaptured && GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
            {
                ogDirMag = new Vector2(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.z).magnitude;
                dirMagCaptured = true;
            }
            Vector3 newDir = target.transform.position - transform.position;
            newDir = newDir.normalized * ogDirMag;
            Debug.Log($"Homing! OG: {ogDirMag}, newDir: {newDir}");
            GetComponent<Rigidbody>().velocity = new Vector3(newDir.x, GetComponent<Rigidbody>().velocity.y, newDir.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            homing = false;
            dirMagCaptured = false;
        }
    }

    public void PickupObject()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;
        foreach (Collider collider in additionalColliders)
        {
            collider.enabled = false;
        }
        if (GetComponent<ShoppingItem>())
        {
            GetComponent<ShoppingItem>().isBeingHeld = true;
        }
        
        if (isDodgeball)
        {
            if (gameObject.layer == 10)
            {
                throwableArrowMedium.SetActive(true);
            }
            if (gameObject.layer == 11)
            {
                throwableArrowHeavy.SetActive(true);
            }
            if (gameObject.layer == 12)
            {
                throwableArrowLight.SetActive(true);
            }
        }

    }

    public void LetGo()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().enabled = true;
        foreach (Collider collider in additionalColliders)
        {
            collider.enabled = true;
        }
        if (GetComponent<ShoppingItem>())
        {
            GetComponent<ShoppingItem>().isBeingHeld = false;
        }
        
        if (isDodgeball)
        {
            if (gameObject.layer == 10)
            {
                throwableArrowMedium.SetActive(false);
            }
            if (gameObject.layer == 11)
            {
                throwableArrowHeavy.SetActive(false);
            }
            if (gameObject.layer == 12)
            {
                throwableArrowLight.SetActive(false);
            }
        }        
    }

    public void SetHoming(bool tf, GameObject _target)
    {
        homing = tf;
        target = _target;
    }
}
