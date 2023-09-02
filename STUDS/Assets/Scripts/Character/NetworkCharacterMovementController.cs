using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class NetworkCharacterMovementController : NetworkBehaviour
{
    public AK.Wwise.Event JumpSound;
    public AK.Wwise.Event GrabSound;
    public AK.Wwise.Event ThrowSound;
    public AK.Wwise.Event RunSound;

    private CharacterController controller;
    private Rigidbody _rigidbody;

    public SteamAchievements sa;

    public Transform camPos;
    public Animator animator;
    public Renderer renderer;
    public CameraShake cameraShake;
    public NetGameManager hub;

    public GameObject playerCamera;
    public GameObject binky;
    public GameObject target;
    public GameObject aimAssist;

    public float moveSpeed;
    public float moveSpeedKid;
    private float knockBackCounter;
    private float pickupCooldown;
    private float stepSoundCooldown;

    public float gravity;
    public float jumpHeight;
    public float moveSpeedGrab;
    public float moveSpeedNormal;
    public float turnTime;
    public float jumpHeightGrab;
    public float throwForce;
    public float throwCoolDown = 0;
    public float knockBackTime;
    public float timeUntilMoveEnabled = 0;

    public Vector3 velocity;

    private Vector2 direction;

    float turnSmoothVelocity;

    private GameObject grabbedObject;
    [SyncVar(hook = nameof(HookSetGrabbedObject))]
    public uint grabbedObjectID = 0;
    private GameObject electronicObject;

    private Material savedMaterial;
    public Material[] kidsMaterials;

    [SerializeField]
    private bool hasGrabbed = false;
    public bool pickupPressed;
    public bool throwPressed = false;
    [SyncVar]
    private bool isReady = false;
    private bool drop;

    [SyncVar]
    public bool isMini = false;
    public bool isJumping = false;
    public bool airborn;
    public bool beingKnockedBack;
    public bool movementEnabled = true;
    public bool isMoving;
    public bool isAI = false;

    [SyncVar]
    [SerializeField]
    private int playerID;
    private int completedCheckpoints;
    [SyncVar]
    public int finishPosition;

    private string selectedLevel;
    public string color;

    //Particle effects
    //private bool isBlinking;
    public bool hasAimAssist;
    private PLR_ParticleController PlayerParticles;

    public bool CanJump, CanMove;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        hub = GameObject.Find("GameManager").GetComponent<NetGameManager>();
        if (!isLocalPlayer)
        {
            Destroy(GetComponent<Rigidbody>()); // This was causing characters to fly off over network.
            if (!isServer && !isAI)
            {
                CmdGetColorInfo();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = moveSpeedNormal;
        direction = new Vector2();
        isReady = false;
        finishPosition = 0;
        controller = GetComponent<CharacterController>();
        PlayerParticles = GetComponent<PLR_ParticleController>();
        CanJump = true;
        CanMove = true;
        if (!isAI)
        {
            sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
            hub.NetworkPlayerJoin(this.gameObject); // This is unique to network characters.
        }
        if (!isServer)
            CmdRequestPlayerVariables();
    }

    // Update is called once per frame
    void Update()
    {
        // We don't update other player's actions on our end unless they tell us to.
        if (!isLocalPlayer)
        {
            return;
        }
        if (movementEnabled && !isAI)
            Move();
        else
        {
            timeUntilMoveEnabled -= Time.deltaTime;
            if (timeUntilMoveEnabled < 0)
            {
                timeUntilMoveEnabled = 0;
                movementEnabled = true;
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        if (!isAI)
        {
            PlaySteps();
        }


        Jump();

        CollisionDetection();

        if (knockBackCounter <= 0)
        {
            //Grab input
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Grab")))
            {
                pickupPressed = true;
                if (electronicObject != null)
                {
                    electronicObject.GetComponent<NetInteraction>().CmdToggleVisual(isMini);
                }
            }
            else
            {
                pickupPressed = false;
            }
            // Throw input
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Throw"))
            {
                throwPressed = true;
            }
            else
            {
                throwPressed = false;
            }
            //Handle moving and rotating the object that has been grabbed
            // Note that this is handled in the grabbableObjectController for non-local players
            if (hasGrabbed)
            {
                if (grabbedObject)
                {
                    // Defaults
                    float grabDistance = 1.3f;
                    float grabHeight = 0.7f;
                    //Quaternion grabRotation = Quaternion.; //Maybe we'll need this for the hammer in shopping spree
                
                    if (grabbedObject.GetComponent<NetGrabbableObjectController>())
                    {
                        grabDistance = grabbedObject.GetComponent<NetGrabbableObjectController>().distance;
                        grabHeight = grabbedObject.GetComponent<NetGrabbableObjectController>().height;
                    }
                    grabbedObject.transform.position = transform.position + (transform.forward * grabDistance) + (transform.up * grabHeight);
                    grabbedObject.transform.rotation = transform.rotation;


                    //If player released "e" then let go
                    if (pickupPressed)
                    {
                        GrabSound.Post(gameObject);
                        //grabSound.Play();
                        DropGrabbedItem();
                        pickupPressed = false;

                        animator.SetBool("isHoldingSomething", false);
                    }
                    else if (throwPressed && throwCoolDown <= 0)
                    {
                        StartCoroutine(performThrow());
                        animator.SetBool("isHoldingSomething", false);
                        throwCoolDown = 1;
                    }
                } else { // HasGrabbed is true but object is Null.  This happens when we pick up an object and it is destroyed without dropping it
                    hasGrabbed = false;
                    animator.SetBool("isHoldingSomething", false);
                }
            }
            else
            {
                HandleGrabObject();
            }
        }
        else
        {
            knockBackCounter -= Time.deltaTime;
            if (hasGrabbed)
            {
                grabbedObject.transform.position = transform.position + (transform.forward * 1.3f) + (transform.up * 0.7f); // Added transform.up because stroller is in the ground with new dad model.
                grabbedObject.transform.rotation = transform.rotation;
                //grabbedObject.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
        }

        if (throwCoolDown > 0)
            throwCoolDown -= Time.deltaTime;

        if (pickupCooldown > 0)
        {
            pickupCooldown -= Time.deltaTime;
        }

    }

    private void CollisionDetection()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (airborn)
        {
            // if we hit the ground after being knocked back mid-air
            if (Physics.Raycast(ray, out hit, 0.1f) && beingKnockedBack
                && drop && hit.transform.CompareTag("Ground"))
            {
                FallDown(1.5f);
                animator.SetTrigger("Land");
                velocity.x = 0;
                velocity.z = 0;
                airborn = false;
            } // Normal landings
            else if (Physics.Raycast(ray, 0.1f))
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Land");
                velocity.x = 0;
                velocity.z = 0;
                airborn = false;
            }
        }
        else // If not airborn
        {
            // Check if airborn
            if (Physics.Raycast(ray, out hit, 0.1f) == false || !hit.transform.CompareTag("Ground"))
            {
                airborn = true;
            } // If you are hit while on the ground
            else if (Physics.Raycast(ray, 0.1f) && beingKnockedBack && drop)
            {
                FallDown(1.3f);
                animator.ResetTrigger("Land");
            }
        }
    }

    private void FallDown(float cooldown)
    {
        animator.ResetTrigger("Jump");
        CmdAnimSetTrigger("FallDown");
        beingKnockedBack = false;
        movementEnabled = false;
        timeUntilMoveEnabled = cooldown;
        drop = false;
    }

    public void Jump()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump"))
        {
            isJumping = true;
            animator.ResetTrigger("Land");
            animator.SetTrigger("Jump");
        }
        else
        {
            isJumping = false;
        }
        //Jumping off the ground sound and physics
        if (airborn == false && isJumping && CanJump)
        {
            if (!isAI)
            {
                JumpSound.Post(gameObject);
            }
            if (hasGrabbed)
            {
                velocity.y = Mathf.Sqrt(jumpHeightGrab * -2 * gravity);
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
        }
        Gravity();
    }

    private void Gravity()
    {
        // Gravity
        if (!gameObject.GetComponent<CharacterController>().isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else // Gravity while grounded.
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            velocity.y = 0;
        }
    }

    /// <summary>
    /// Controls Movement animations and character controller movement.
    /// </summary>
    private void Move()
    {
        if (CanMove)
        {
            // Use of old input system for network play
            direction.x = Input.GetAxis("Horizontal");
            direction.y = Input.GetAxis("Vertical");

            if (PlayerParticles)
                PlayerParticles.TurnOnRunning();
            Vector3 dirVector = new Vector3(direction.x, 0f, direction.y).normalized;

            if (dirVector.magnitude >= 0.1f)
            {

                animator.SetBool("isRunning", true);
                isMoving = true;
                float targetAngle = Mathf.Atan2(dirVector.x, dirVector.z) * Mathf.Rad2Deg + camPos.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move((moveDir.normalized * moveSpeed) * Time.deltaTime);

            }
            else
            {
                animator.SetBool("isRunning", false);
                isMoving = false;
                if (airborn == false)
                {
                    velocity.x = 0;
                    velocity.z = 0;
                }
                if (PlayerParticles)
                    PlayerParticles.TurnOffRunning();
            }
        }
    }

    /// <summary>
    /// A Helper method to move the character in a specific direction
    /// </summary>
    /// <param name="dirVector">direction vector to move the character</param>
    public void Move(Vector3 dirVector)
    {
        if (CanMove)
        {
            if (dirVector.magnitude >= 0.1f && movementEnabled)
            {
                animator.SetBool("isRunning", true);
                controller.Move((dirVector.normalized * moveSpeed) * Time.deltaTime);
            }
            else
            {
                animator.SetBool("isRunning", false);
                velocity.x = 0;
                velocity.z = 0;
            }
        }
    }

    private void PlaySteps()
    {
        float timeBetween = 0.25f;
        if (isMoving && !airborn && stepSoundCooldown < 0)
        {
            RunSound.Post(gameObject);
            //runSound.Play();
            stepSoundCooldown = timeBetween;
        }
        else
        {
            stepSoundCooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Handles move input with the new input system.
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (PlayerParticles)
            PlayerParticles.TurnOnRunning();
    }

    /// <summary>
    /// Handles camera move input with the new input system.
    /// </summary>
    /// <param name="context"></param>
    public void OnCameraMove(CallbackContext context)
    {
        playerCamera.GetComponent<CameraMoveFG>().SetDirectionVector(context.ReadValue<Vector2>());
    }

    public void OnJump(CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            isJumping = true;
            animator.ResetTrigger("Land");
            animator.SetTrigger("Jump");
        }
        else
        {
            isJumping = false;
        }

    }

    public void OnQuit(CallbackContext context)
    {
        if (context.performed)
        {
            Application.Quit();
        }
    }

    public void OnRestart(CallbackContext context)
    {
        if (context.performed)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            gameManager.GetComponent<ManagePlayerHub>().DeletePlayers();
            Destroy(gameManager);
            SceneManager.LoadScene("TheBlock_LevelSelect"); // TODO: Should this be changed or is this method even used?
        }
    }

    public void OnPickup(CallbackContext context)
    {
        if (context.performed)
        {
            pickupPressed = true;
            if (electronicObject != null)
            {
                electronicObject.GetComponent<NetInteraction>().CmdToggleVisual(isMini);
            }
        }
        else
        {
            pickupPressed = false;
        }
    }

    public void OnThrow(CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            throwPressed = true;
        }
        else
        {
            throwPressed = false;
        }
    }

    public void OnInteract(CallbackContext context)
    {
        if (context.performed)
        {
            if (electronicObject != null)
            {
                electronicObject.GetComponent<NetInteraction>().CmdToggleVisual(isMini);
                PlayerParticles.inRange = false;
                //PlayerParticles.DisableEmote();
            }
        }
        else
        {
            throwPressed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Electronics"))
        {
            electronicObject = other.transform.parent.gameObject;
        }
    }

    /// <summary>
    /// Sets the reference to the electronic object while the character is in the trigger.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Electronics"))
        {
            electronicObject = other.transform.parent.gameObject;
            //Debug.Log("Assign to variable");
        }
    }

    /// <summary>
    /// Resets electronic object reference if player is no longer in trigger.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Electronics"))
        {
            electronicObject = null;
        }
    }

    /// <summary>
    /// Knocks player in the direction given and can also cause player to drop their item.
    /// </summary>
    /// <param name="direction">The direction to send the player</param>
    /// <param name="_drop">If the player should drop the object when hit</param>
    public void KnockBack(Vector3 direction, bool _drop)
    {
        Debug.Log("KBCalled with: " + direction.ToString());
        knockBackCounter = knockBackTime;
        beingKnockedBack = true;
        drop = _drop;
        if (_drop)
        {
            DropGrabbedItem();
            if (cameraShake)
                StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
        }
        velocity = direction;

    }

    /// <summary>
    /// Handles animation and physics for dropping items.
    /// </summary>
    public void DropGrabbedItem()
    {
        if (grabbedObject)
        {
            animator.SetBool("isHoldingSomething", false);
            if (grabbedObject.CompareTag("ShoppingCart"))
            {
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            }
            CmdLetGo(grabbedObject.GetComponent<NetworkIdentity>().netId);
            grabbedObject = null;
            grabbedObjectID = 0;
            hasGrabbed = false;
            moveSpeed = moveSpeedNormal;
        }
    }

    /// <summary>
    /// Sets aim assist to toggle
    /// </summary>
    /// <param name="toggle"></param>
    public void SetAimAssist(bool toggle)
    {
        aimAssist.SetActive(toggle);
    }

    /// <summary>
    /// Handles animation and physics for throwing objects.
    /// </summary>
    /// <returns></returns>
    private IEnumerator performThrow()
    {
        animator.ResetTrigger("Land");
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(0.0f);
        ThrowSound.Post(gameObject);
        Vector3 forward = transform.forward;
        grabbedObject.transform.forward = forward;
        if (hasAimAssist)
        {
            forward = Vector3.Normalize(target.transform.position - transform.position);
        }
        Vector3 throwingForce = forward * throwForce * 2.3f;
        Vector3 movementAdjust = forward * direction.magnitude * moveSpeedGrab * 40;
        throwingForce += movementAdjust;
        throwingForce.y = 300f;
        CmdLetGo(grabbedObject.GetComponent<NetworkIdentity>().netId);

        if (isServer)
        {
            RpcThrow(grabbedObject, throwingForce);
            if (grabbedObject.GetComponent<CombatThrow>())
            {
                RpcEnableKnockBack(grabbedObject);
            }
        }
        else
        {
            //grabbedObject.GetComponent<Rigidbody>().AddForce(throwingForce);
            CmdThrow(grabbedObject, throwingForce, grabbedObject.transform.position);
            if (grabbedObject.GetComponent<CombatThrow>())
            {
                CmdEnableKnockBack(grabbedObject);
            }
        }

        if (grabbedObject.GetComponent<StrollerController>())
        {
            sa.UnlockAchievement("SR_STROLLER");
        }

        hasGrabbed = false;
        moveSpeed = moveSpeedNormal;
        grabbedObject = null;
        grabbedObjectID = 0;
        animator.ResetTrigger("Throw");
    }



    private void LocalEnableKnockBack(GameObject grabbedObject)
    {
        grabbedObject.GetComponent<CombatThrow>().EnableKnockBack(name);
    }

    /// <summary>
    /// Handles all grabbing logic for the various grabbable items.
    /// </summary>
    private void HandleGrabObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Grabbable") || collider.CompareTag("ShoppingItem"))
            {
                if (collider.CompareTag("ShoppingItem"))
                {
                    collider.gameObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
                }

                if (pickupPressed && !hasGrabbed)
                {
                    animator.SetBool("isHoldingSomething", true);
                    GrabSound.Post(gameObject);
                    grabbedObject = collider.gameObject;
                    grabbedObjectID = collider.GetComponent<NetworkIdentity>().netId;
                    grabbedObject.GetComponent<NetGrabbableObjectController>().LocalPickupObject(name);
                    CmdPickupObject(grabbedObject);
                    moveSpeed = moveSpeedGrab;
                    hasGrabbed = true;
                    pickupPressed = false;
                    if (grabbedObject.name.StartsWith("Grabpoint")) // Designed to only use Grabpoint as the name of the Utility Cart Grabbing point.
                    {
                        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
            else if (collider.CompareTag("Player") && collider.GetComponent<NetworkCharacterMovementController>().isMini)
            {
                if (pickupPressed && isMini == false)
                {
                    sa.UnlockAchievement("PB_TIMEOUT");
                    CmdTimeout(collider.GetComponent<NetworkIdentity>().netId);
                }
            }
            else if (collider.CompareTag("ShoppingCart") && pickupPressed && !hasGrabbed)
            {
                CmdPickupFromCart(collider.GetComponent<NetworkIdentity>().netId);
                
                GrabSound.Post(gameObject);
                moveSpeed = moveSpeedGrab;
                pickupPressed = false;
            }
        }
    }

    [Command]
    private void CmdTimeout(uint kidID)
    {
        NetworkIdentity.spawned[kidID].GetComponent<NetKidTimeout>().RpcTimeout();
    }

    [Command]
    public void CmdDestroyObject(uint objID)
    {
        if (NetworkIdentity.spawned.ContainsKey(objID))
        {
            Debug.Log($"Destroying {NetworkIdentity.spawned[objID].name}");
            NetworkServer.Destroy(NetworkIdentity.spawned[objID].gameObject);
        }
        else
            Debug.LogError($"Object ID: <{objID}> not found");
    }

    [Command]
    private void CmdRequestPlayerVariables()
    {
        hub.RpcSetPlayerVariables();
    }

    [Command]
    private void CmdChangePlayerColor(string colorName)
    {
        hub.ChangePlayerColor(playerID, colorName);
    }

    [Command]
    private void CmdGetColorInfo()
    {
        RpcSetColor(color);
    }

    [Command]
    private void CmdPickupFromCart(uint cartId)
    {
        uint objId = NetworkIdentity.spawned[cartId].GetComponent<NetCart>().GiveObject(transform);
        if (objId != 0)
            RpcPickupFromCart(objId);
    }

    [Command]
    private void CmdAssignNetworkAuthority(NetworkIdentity item, NetworkIdentity clientId)
    {
        if (!item)
        {
            Debug.LogError("Woah!  NO ITEM???");
        }
        if (item.connectionToClient == null)
        {
            Debug.LogError("Connection to client is null");
        }
        if (item.connectionToClient != null && item.hasAuthority == false)
        {
            item.RemoveClientAuthority();
        }
        if (item.connectionToClient == null)
        {
            item.AssignClientAuthority(clientId.connectionToClient);
        }
    }

    [Command]
    private void CmdRemoveNetworkAuthority(NetworkIdentity item)
    {
        //RpcRemoveNetworkAuthority(item);
        item.RemoveClientAuthority();
    }

    [Command]
    private void CmdPickupObject(GameObject obj)
    {
        Debug.Log("CmdPickupObject: " + obj.name);
        hasGrabbed = true;
        grabbedObjectID = obj.GetComponent<NetworkIdentity>().netId;
        obj.GetComponent<NetGrabbableObjectController>().LocalPickupObject(name);
        RpcPickupObject(grabbedObjectID);
        if (obj.GetComponent<ShoppingItem>())
            obj.GetComponent<ShoppingItem>().SetPlayer(gameObject);
    }

    [Command]
    private void CmdLetGo(uint objID)
    {
        if (NetworkIdentity.spawned[objID])
        {
            NetworkIdentity.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalLetGo();
            RpcLetGo(objID);
        } else
        {
            Debug.LogError("Network Identity not found : " + objID);
        }
    }

    [Command]
    private void CmdThrow(GameObject obj, Vector3 throwingForce, Vector3 itemPosition)
    {
        /*NetworkIdentity netId = obj.GetComponent<NetworkIdentity>();
        if(netId)
        {
            netId.RemoveClientAuthority();
        } */
        obj.transform.position = itemPosition;
        obj.GetComponent<Rigidbody>().AddForce(throwingForce);
        //RpcThrow(obj, throwingForce);
    }

    [Command]
    private void CmdEnableKnockBack(GameObject grabbedObject)
    {
        Debug.Log("CommandEnableKnockBack");
        //LocalEnableKnockBack(grabbedObject);
        RpcEnableKnockBack(grabbedObject);
    }


    [Command]
    private void CmdAnimSetBool(string varName, bool tf)
    {
        animator.SetBool(varName, tf);
        RpcAnimSetBool(varName, tf);
    }


    [Command]
    private void CmdAnimSetTrigger(string varName)
    {
        animator.SetTrigger(varName);
        RpcAnimSetTrigger(varName);
    }


    [Command]
    public void CmdSetFinishPosition(int pos)
    {
        finishPosition = pos;
    }

    [ClientRpc]
    private void RpcSetColor(string _color)
    {
        color = _color;
    }

    [ClientRpc]
    private void RpcPickupFromCart(uint netId)
    {
        GameObject takenObject = NetworkIdentity.spawned[netId].gameObject;
        animator.SetBool("isHoldingSomething", true);
        grabbedObject = takenObject;
        grabbedObjectID = netId;
        takenObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
        takenObject.GetComponent<NetGrabbableObjectController>().LocalPickupObject(name);
        hasGrabbed = true;
    }

    [ClientRpc]
    private void RpcRemoveNetworkAuthority(NetworkIdentity item)
    {
        Debug.Log("Entered removal");
        item.RemoveClientAuthority();
    }

    [ClientRpc]
    private void RpcPickupObject(uint objID)
    {
        if (isServer || isLocalPlayer)
            return;
        Debug.Log("RpcPickupObject: " + objID);
        NetworkIdentity.spawned[objID].gameObject.GetComponent<NetGrabbableObjectController>().LocalPickupObject(name);
    }

    [ClientRpc]
    private void RpcLetGo(uint objID)
    {
        if (NetworkIdentity.spawned.ContainsKey(objID))
            NetworkIdentity.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalLetGo();
        else
        {
            Debug.LogError($"Network Identity <{objID}> not found.");
        }
    }

    [ClientRpc]
    private void RpcThrow(GameObject obj, Vector3 throwingForce)
    {
        if (isLocalPlayer && !isServer)
            return;
        obj.GetComponent<Rigidbody>().AddForce(throwingForce);
    }

    [ClientRpc]
    private void RpcEnableKnockBack(GameObject grabbedObject)
    {
        LocalEnableKnockBack(grabbedObject);
    }


    [ClientRpc]
    private void RpcAnimSetBool(string varName, bool tf)
    {
        animator.SetBool(varName, tf);
    }


    [ClientRpc]
    private void RpcAnimSetTrigger(string varName)
    {
        animator.SetTrigger(varName);
    }

    [ClientRpc]
    public void RpcSetToMini(bool setMini)
    {
        SetToMini(setMini);
    }

    [ClientRpc]
    public void RpcSetColorName(string _color)
    {
        color = _color;
        if(isServer)
            hub.ChangePlayerColor(playerID, _color);
    }

    /// <summary>
    /// Modifies character to kid size and binkiness for Penny Pincher level.
    /// </summary>
    /// <param name="setMini"></param>
    public void SetToMini(bool setMini)
    {
        if (setMini)
        {
            transform.localScale = new Vector3(20, 20, 20); //Shrink the player. OG size is 30, 30, 30
            SetBinky(true); //Activate the binky!!!!
            isMini = true; //The Eugine will now act as a child.
            moveSpeed = moveSpeedKid;
            savedMaterial = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = kidsMaterials[GetColorIndex(color)];
        }
        else
        {
            transform.localScale = new Vector3(30, 30, 30); // size is 30, 30, 30
            SetBinky(false); // Deactivate Binky
            isMini = false; //The Eugine will now act as a dad.
            moveSpeed = moveSpeedNormal;
            if (savedMaterial)
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = savedMaterial;
        }
    }

    /// <summary>
    /// Gets the index of the given color for enumeration purposes. Returns -1 if color is not associated.
    /// </summary>
    /// <param name="_color">color name with the first letter capitalized</param>
    /// <returns>an integer index</returns>
    private int GetColorIndex(string _color)
    {
        return _color switch
        {
            "red" => 0,
            "blue" => 1,
            "purple" => 2,
            "yellow" => 3,
            "green" => 4,
            _ => -1,
        };
    }

    public bool GetHasGrabbed()
    {
        return hasGrabbed;
    }

    public void SetPlayerID(int ID)
    {
        playerID = ID;
    }

    public int getPlayerID()
    {
        return playerID;
    }

    public void SetCheckpointCount(int count)
    {
        completedCheckpoints = count;
    }

    public int getCheckpointCount()
    {
        return completedCheckpoints;
    }

    public void setMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public float getMoveSpeed()
    {
        return moveSpeed;
    }

    /// <summary>
    /// Activates the animations for holding an object, gives the player a ref to the object, and adjusts physics
    /// </summary>
    /// <param name="obj"></param>
    public void SetGrabbedObject(GameObject obj)
    {
        animator.SetBool("isHoldingSomething", true);
        grabbedObject = obj;
        grabbedObjectID = obj.GetComponent<NetworkIdentity>().netId;
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
        moveSpeed = moveSpeedGrab;
        hasGrabbed = true;
        pickupPressed = false;
    }

    private void HookSetGrabbedObject(uint oldObj, uint objID)
    {
        grabbedObjectID = objID;
        if (objID != 0)
        {
            animator.SetBool("isHoldingSomething", true);
            grabbedObject = NetworkIdentity.spawned[objID].gameObject;
            grabbedObject.GetComponent<NetGrabbableObjectController>().LocalPickupObject(name);
            moveSpeed = moveSpeedGrab;
            hasGrabbed = true;
            pickupPressed = false;
        }
    }

    public GameObject GetGrabbedObject()
    {
        return grabbedObject;
    }
    
    public void SetColorName(string colorName)
    { 
        color = colorName;
        //GameObject gameManager = GameObject.Find("GameManager");
        CmdChangePlayerColor(color);
        
    }

    public string GetColorName()
    {
        return color;
    }

    /// <summary>
    /// Sets the isReady and selected level parameter of a character
    /// </summary>
    /// <param name="_isReady"></param>
    /// <param name="_selectedLevel"></param>
    public void ReadyPlayer(bool _isReady, string _selectedLevel)
    {
        isReady = _isReady;
        selectedLevel = _selectedLevel;
    }

    /// <summary>
    /// Get whether or not the player is ready, and if the selected level is the correct level
    /// </summary>
    /// <param name="_selectedLevel"></param>
    /// <returns></returns>
    public bool GetReadyPlayer(string _selectedLevel)
    {
        if (_selectedLevel.Equals("ManagePlayerHub"))
        {
            return isReady;
        }
        return isReady && _selectedLevel == selectedLevel;
    }


    public int GetFinishPosition()
    {
        return finishPosition;
    }

    /// <summary>
    /// Sets the binky to appear or not.
    /// </summary>
    /// <param name="isActive"></param>
    public void SetBinky(bool isActive)
    {
        binky.SetActive(isActive);
    }

    /// <summary>
    /// Gets Character Controller
    /// </summary>
    /// <returns>CharacterController</returns>
    public CharacterController GetController()
    {
        return controller;
    }

    /// <summary>
    /// Should only be called on the server.
    /// </summary>
    /// <param name="pos">Number to set player's position to.</param>
    public void SetFinishPosition(int pos)
    {
        finishPosition = pos;
    }
}
