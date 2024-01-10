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
    public bool isDropped = true;
    public Material PickedUpMaterial;
    public Material DroppedMaterial;
    public MeshRenderer dodgeballRenderer;
    private bool homing = false, dirMagCaptured = false;
    private float ogDirMag = 0;
    public float materialLerpDuration = 1.5f;

    public void Start()
    {
        if (isDodgeball)
        {
            //set dodgeball material without highlight outline
            dodgeballRenderer.material = DroppedMaterial;
        }
    }

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

        if (isDodgeball && isDropped)
        {
            //lerp between highlight outline and non-outlined materials
            BallMaterialLerp();
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
            //set material and throw line preview
            PickUpDodgeball();
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
            //set material and throw line preview
            DropDodgeball();
        }        
    }

    public void SetHoming(bool tf, GameObject _target)
    {
        homing = tf;
        target = _target;
    }

    public void BallMaterialLerp()
    {
        float lerp = Mathf.PingPong(Time.time, materialLerpDuration) / materialLerpDuration;
        dodgeballRenderer.material.Lerp(PickedUpMaterial, DroppedMaterial, lerp);
    }

    public void PickUpDodgeball()
    {
        dodgeballRenderer.material = PickedUpMaterial;
        isDropped = false;

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

    public void DropDodgeball()
    {
        dodgeballRenderer.material = DroppedMaterial;
        isDropped = true;

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
