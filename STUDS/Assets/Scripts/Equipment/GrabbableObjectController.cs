using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObjectController : MonoBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    public GameObject target;

    [Header("Dodgeball Aimer Prefabs")]
    public GameObject throwableArrowMedium;
    public GameObject throwableArrowHeavy;
    public GameObject throwableArrowLight;

    [Header("Dodgeball Picked Up")]
    public bool isDodgeball = false;
    public bool isDropped = true;
    public Material PickedUpMaterial;
    public Material DroppedMaterial;
    public MeshRenderer dodgeballRenderer;
    public float materialLerpDuration = 1.5f;

    [Header("Dodgeball Clean-Up")]
    public bool isDeleteBallTimerStarted;
    public float DeleteBallTimer;
    public GameObject DeleteBallFX;

    [Header("Homing")]
    private bool homing = false, dirMagCaptured = false, canPickup = true;
    private Vector2 ogDir = new Vector2();
    public string throwerColor = "";
    

    public void Start()
    {
        if (isDodgeball)
        {
            //set dodgeball material without highlight outline
            dodgeballRenderer.material = DroppedMaterial;
            isDeleteBallTimerStarted = false;
            DeleteBallTimer = 0;
        }
    }

    private void Update()
    {
        if (homing)
        {
            /*if (!dirMagCaptured && GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
            {
                ogDir = new Vector2(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.z);
                dirMagCaptured = true;
            }*/

            Vector3 newDir = target.transform.position - transform.position;
            newDir = newDir.normalized * ogDir.magnitude;
            GetComponent<Rigidbody>().velocity = new Vector3((newDir.x + ogDir.x)/2, GetComponent<Rigidbody>().velocity.y, (newDir.z +ogDir.y)/2);
        }

        if (isDodgeball && isDeleteBallTimerStarted)
        {
            StartDeleteBallTimer();
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
            throwerColor = "";
            GetComponent<CombatThrow>().knockBack.GetComponent<KnockBack>().owner = "";
            GetComponent<CombatThrow>().knockBack.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (isDodgeball)
            DBGameManager.Instance.deListDodgeball(gameObject);
    }

    public void PickupObject(string _color, bool isLocalPlayer = false)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;
        canPickup = false;
        throwerColor = _color;
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
            PickUpDodgeball(isLocalPlayer);
        }

    }

    public void LetGo()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().enabled = true;
        canPickup = true;
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
            DropDodgeball();
        }
    }

    /// <summary>
    /// Enables homing throw and sets inital direction
    /// </summary>
    /// <param name="tf">Set homing to true or false</param>
    /// <param name="_target">Target gameobject</param>
    /// <param name="_ogDir">Original throw direction (Entry doesn't matter if setting homing to false</param>
    public void HomingThrow(bool tf, GameObject _target, Vector3 _ogDir)
    {
        ogDir = new Vector2(_ogDir.x, _ogDir.z);
        homing = tf;
        target = _target;
    }

    public void BallMaterialLerp()
    {
        float lerp = Mathf.PingPong(Time.time, materialLerpDuration) / materialLerpDuration;
        dodgeballRenderer.material.Lerp(PickedUpMaterial, DroppedMaterial, lerp);
    }

    public void PickUpDodgeball(bool isLocalPlayer = false)
    {
        dodgeballRenderer.material = PickedUpMaterial;
        isDropped = false;
        ResetDeleteBallTimer();
        DBGameManager.Instance.deListDodgeball(gameObject);
        if (isLocalPlayer)
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

    /// <summary>
    /// Set material and throw line preview
    /// </summary>
    public void DropDodgeball()
    {
        dodgeballRenderer.material = DroppedMaterial;
        isDropped = true;
        isDeleteBallTimerStarted = true;

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

    public void StartDeleteBallTimer()
    {
        isDeleteBallTimerStarted = true;
        DeleteBallTimer += Time.deltaTime;

        if (DeleteBallTimer >= 27)
        {
            materialLerpDuration = 0.25f;
        }

        if (DeleteBallTimer >= 30)
        {
            Instantiate(DeleteBallFX, gameObject.transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(gameObject, 1);
        }

    }

    public void ResetDeleteBallTimer()
    {
        materialLerpDuration = 1.5f;
        isDeleteBallTimerStarted = false;
        DeleteBallTimer = 0;
    }

    public bool getCanPickup()
    {
        return canPickup;
    }

    public void SetCanPickup(bool tf)
    {
        canPickup = tf;
    }
}
