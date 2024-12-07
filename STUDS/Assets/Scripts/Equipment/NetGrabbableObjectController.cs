using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEngine.GraphicsBuffer;

public class NetGrabbableObjectController : NetworkBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public GameObject target;

    [Header("Dodgeball Aimer Prefab")]
    public GameObject throwableArrow;

    [Header("Dodgeball Picked Up")]
    public bool isDodgeball = false;
    [SyncVar] public bool isDropped = true;
    public Material PickedUpMaterial;
    public Material DroppedMaterial;
    public MeshRenderer dodgeballRenderer;
    public float materialLerpDuration = 1.5f;

    [Header("Dodgeball Clean-Up")]
    public bool isDeleteBallTimerStarted;
    public float DeleteBallTimer;
    public GameObject DeleteBallFX;

    [Header("Homing")]
    private bool homing = false, dirMagCaptured = false;
    private Vector2 ogDir = new Vector2();
    public string throwerColor = "";
    private Transform holderTransform = null;
    private GameObject holder;
    [SerializeField] private GameObject localPrefab;
    private GameObject localGO;
    [SerializeField] private GameObject effects;
    [SyncVar] private bool canPickup = true;

    public void Start()
    {
        if (isDodgeball)
        {
            Instantiate(effects, transform.position, transform.rotation);
            //set dodgeball material without highlight outline
            dodgeballRenderer.material = DroppedMaterial;
            isDeleteBallTimerStarted = false;
            DeleteBallTimer = 0;
        }
    }

    /// <summary>
    /// Movement is server-authoritative, so we update throwable positions in the update method
    /// when they are being held.
    /// </summary>
    private void Update()
    {
        if (isServer) 
        {
            if (homing)
            {
                Vector3 newDir = target.transform.position - transform.position;
                newDir = newDir.normalized * ogDir.magnitude;
                GetComponent<Rigidbody>().velocity = new Vector3((newDir.x + ogDir.x) / 2, GetComponent<Rigidbody>().velocity.y, (newDir.z + ogDir.y) / 2);
            }

            if (isDodgeball && isDeleteBallTimerStarted)
            {
                StartDeleteBallTimer();
            }

            if (!homing && holderTransform)
            {
                transform.position = holderTransform.position + (transform.forward * distance) + (transform.up * height);
                transform.rotation = holderTransform.rotation;
            }
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
            if (GetComponent<CombatThrow>())
            {
                GetComponent<CombatThrow>().knockBack.GetComponent<KnockBack>().owner = "";
                GetComponent<CombatThrow>().knockBack.SetActive(false);
            }
        }
    }

    public void OnDestroy()
    {
        Destroy(localGO);
        if (isDodgeball && isServer)
            NetDBGameManager.Instance.deListDodgeball(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_holderTransform">The transform of the holder of this object.</param>
    /// <returns>A reference to the offline copy instantiated on this client.</returns>
    public GameObject LocalPickupObject(Transform _holderTransform)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;
        canPickup = false;
        holderTransform = _holderTransform;
        holder = _holderTransform.gameObject;
        throwerColor = holder.GetComponent<NetworkCharacterMovementController>().GetColorName();
        foreach (Collider collider in additionalColliders)
        {
            collider.enabled = false;
        }
        if (GetComponent<ShoppingItem>())
        {
            GetComponent<ShoppingItem>().isBeingHeld = true;
        }
        if (GetComponent<NetExplodingPropane>())
        {
            GetComponent<NetExplodingPropane>().setOwnerName(holder.name);
            SteamAchievements.UnlockAchievement("SS_PROPANE");
        }

        // Make networked version of object invisible (because it always lags) and spawn a local version instead.
        if (localPrefab)
        {
            GameObject offlineCopy = Instantiate(localPrefab, transform.position, transform.rotation);
            foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = false;
            }
            offlineCopy.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO = gameObject;

            if(GetComponent<NetCart>()) { 
                bool[] isActiveArr = new bool[8];
                int i = 0;
                foreach (GameObject obj in GetComponent<NetCart>().cartItems)
                {
                    isActiveArr[i] = obj.activeSelf;
                    ++i;
                }
                offlineCopy.GetComponentInChildren<LocalCart>().LocalSetActiveItems(isActiveArr);
            } else if (GetComponent<StrollerController>()) // The else if is necessary since shopping carts also have StrollerControllers.
            {
                offlineCopy.GetComponent<StrollerController>().SetID(GetComponent<StrollerController>().GetID());
                offlineCopy.GetComponent<StrollerController>().DetermineColor(GetComponent<StrollerController>().GetColor());
            }
            localGO = offlineCopy;
        }
        if (isDodgeball)
            PickUpDodgeball(holder.GetComponent<NetworkCharacterMovementController>().isLocalPlayer);
        return localGO;
    }

    /// <summary>
    /// Erases holder transform, deals with collisions and physics, re-enables colliders, 
    /// sets isbeingheld to false on shopping item, 
    /// Spawns a local object which linear interpolates from local GO to net GO.
    /// </summary>
    /// <returns>A reference to this gameobject</returns>
    public GameObject LocalLetGo()
    {
        holderTransform = null;
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
        
        if (localGO)
        {
            GameObject lerpGO = Instantiate(localGO, localGO.transform.position, localGO.transform.rotation);
            //stop rendering the localGO
            foreach (MeshRenderer rend in localGO.GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = false;
            }
            //Create a new localGO
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().SetLocalT(localGO.transform);
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().SetLerp(true);
            lerpGO.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO = gameObject;
        }

        if (isDodgeball)
        {
            DropDodgeball();
        }
         
        return gameObject;
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

    public void PickUpDodgeball(bool _isLocalPlayer)
    {
        dodgeballRenderer.material = PickedUpMaterial;
        isDropped = false;
        ResetDeleteBallTimer();
        if(isServer)
            NetDBGameManager.Instance.deListDodgeball(gameObject);
        if (_isLocalPlayer)
        {
            localGO.GetComponent<LocalGrabbableObjectController>().throwableArrow.SetActive(true);
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

        throwableArrow.SetActive(false);
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
            if(isServer)
                NetworkServer.Destroy(gameObject);
        }

    }

    public void ResetDeleteBallTimer()
    {
        materialLerpDuration = 1.5f;
        isDeleteBallTimerStarted = false;
        DeleteBallTimer = 0;
    }

    public void RenderNetworkedGO()
    {
        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.enabled = true;
        }
        Destroy(localGO);
    }

    public void PropaneExplode(GameObject particleEffect)
    {

        GameObject effect = Instantiate(particleEffect, transform.position, Quaternion.identity);
        NetworkServer.Spawn(effect, holder);
        Destroy(gameObject);
    }

    public bool GetCanPickup()
    {
        return canPickup;
    }

    public void SetCanPickup(bool tf)
    {
        canPickup = tf;
    }
}
