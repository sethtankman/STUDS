using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.AI;

public class NetworkCharacterMovementController : NetworkBehaviour
{
    public AK.Wwise.Event JumpSound;
    public AK.Wwise.Event GrabSound;
    public AK.Wwise.Event ThrowSound;
    public AK.Wwise.Event RunSound;

    private CharacterController controller;
    private Rigidbody _rigidbody;

    public Transform camPos;
    public Animator animator;
    public NetworkAnimator netAnim;
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
    [SerializeField] private float throwForce;
    [SerializeField] private float homingSpeed;
    public float throwCoolDown = 0;
    public float knockBackTime;
    public float timeUntilMoveEnabled = 0;

    private float speedMultiplier = 1;

    public Vector3 velocity;

    /// <summary>
    /// Stores the direction the user is inputing through their controls.
    /// </summary>
    private Vector2 direction;

    private Vector3 throwingForce;

    float turnSmoothVelocity;

    public GameObject grabbedObject;
    public uint grabbedObjectID = 0;
    private NetInteraction electronicObject;

    private Material savedMaterial;
    public Material[] kidsMaterials;

    [SerializeField]
    private bool hasGrabbed = false;
    public bool pickupPressed;
    public bool inStrollerRace = false;
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
        hub = GameObject.Find("NetGameManager").GetComponent<NetGameManager>();
        if (!isLocalPlayer && !isAI)
        {   
            Destroy(GetComponent<Rigidbody>()); // This was causing characters to fly off over network.
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = moveSpeedNormal;
        direction = new Vector2();
        isReady = false;
        if(SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
            finishPosition = 0;
        controller = GetComponent<CharacterController>();
        PlayerParticles = GetComponent<PLR_ParticleController>();
        CanJump = true;
        CanMove = true;
        if (!isAI)
        {
            if (SceneManager.GetActiveScene().name.Equals("TheBlock_LevelSelectOnlineMultiplayer"))
                hub.NetworkPlayerJoin(this.gameObject); // This is unique to network characters.
        }
        if (isLocalPlayer)
            CmdRequestPlayerVariables();
        if (GetComponentInChildren<Camera>()) { 
            if (SceneManager.GetActiveScene().name.Equals("NetVictoryStands"))
            {
                GetComponentInChildren<Camera>().enabled = false;
            } else
            {
                GetComponentInChildren<Camera>().enabled = true;
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!isAI && connectionToClient.authenticationData != null)
        {
            PlayerInfo info = (PlayerInfo)connectionToClient.authenticationData;
            color = info.color;
            playerID = info.id;
            SetFinishPosition(info.finishPos);
            GetComponentInChildren<SkinnedMeshRenderer>().material = NetGameManager.Instance.colorMaterials[color];
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        PlayerInfo info = new PlayerInfo(color, playerID, finishPosition);
        if(connectionToClient != null)
            connectionToClient.authenticationData = info;
    }

    // Update is called once per frame
    void Update()
    {
        // only local players and AI on the server can control their own movements.
        // So if you are not the local player or you are not an AI on the server, only move the held item.
        if (!isLocalPlayer && !(isAI && isServer))
        {
            if (grabbedObject)
            {
                // Defaults
                float grabDistance = 1.3f;
                float grabHeight = 0.7f;
                if (grabbedObject.GetComponent<LocalGrabbableObjectController>())
                {
                    grabDistance = grabbedObject.GetComponent<LocalGrabbableObjectController>().distance;
                    grabHeight = grabbedObject.GetComponent<LocalGrabbableObjectController>().height;
                }
                grabbedObject.transform.position = transform.position + (transform.forward * grabDistance) + (transform.up * grabHeight);
                grabbedObject.transform.rotation = transform.rotation;
            }
            return;
        }

        if (movementEnabled && !isAI)
            Move();
        else if(!movementEnabled)
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
        if(GetComponentInChildren<CharacterController>().enabled)
            Jump(); // Was calling CharacterController.Move when CharacterController is disabled.
        CollisionDetection();

        ThrowAndPickup();
    }

    private void ThrowAndPickup()
    {
        if (knockBackCounter <= 0)
        {
            //Grab input
            if (!isAI && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Grab")))
            {
                pickupPressed = true;
                if (electronicObject != null)
                {
                    electronicObject.CmdToggleVisual(isMini);
                }
            }
            else
            {
                pickupPressed = false;
            }
            // Throw input
            if (!isAI && (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Throw")))
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
                    //Quaternion grabRotation = Quaternion.; // TODO: Maybe we'll need this for the hammer in shopping spree

                    if (grabbedObject.GetComponent<NetGrabbableObjectController>())
                    {
                        grabDistance = grabbedObject.GetComponent<NetGrabbableObjectController>().distance;
                        grabHeight = grabbedObject.GetComponent<NetGrabbableObjectController>().height;
                    }
                    else if (grabbedObject.GetComponent<LocalGrabbableObjectController>())
                    {
                        grabDistance = grabbedObject.GetComponent<LocalGrabbableObjectController>().distance;
                        grabHeight = grabbedObject.GetComponent<LocalGrabbableObjectController>().height;
                    }
                    grabbedObject.transform.position = transform.position + (transform.forward * grabDistance) + (transform.up * grabHeight);
                    grabbedObject.transform.rotation = transform.rotation;

                    Vector3 forward = transform.forward;
                    grabbedObject.transform.forward = forward;
                    if (hasAimAssist)
                    {
                        forward = Vector3.Normalize(target.transform.position - transform.position);
                    }
                    throwingForce = forward * (throwForce + (direction.magnitude * moveSpeedGrab * 40));
                    throwingForce.y = 300f;
                    if (!isAI)
                    {
                        ThrowSpline spline = grabbedObject.GetComponentInChildren<ThrowSpline>(true);
                        if (spline)
                        {
                            spline.SetThrowForce(throwingForce, hasAimAssist);
                        }
                    }

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
                        performThrow();
                        animator.SetBool("isHoldingSomething", false);
                        throwCoolDown = 1;
                    }
                }
                else
                { // HasGrabbed is true but object is Null.  This happens when we pick up an object and it is destroyed without dropping it
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
                netAnim.SetTrigger("Land");
                velocity.x = 0;
                velocity.z = 0;
                airborn = false;
            } // Normal landings
            else if (Physics.Raycast(ray, out hit, 0.1f))
            {
                if(!hit.collider.isTrigger)
                {
                    netAnim.ResetTrigger("Jump");
                    netAnim.SetTrigger("Land");
                    velocity.x = 0;
                    velocity.z = 0;
                    airborn = false;
                }
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
                netAnim.ResetTrigger("Land");
            }
        }
    }

    /// <summary>
    /// Handles animations, dropping bools, movementbools for falling down.
    /// </summary>
    /// <param name="cooldown"></param>
    private void FallDown(float cooldown)
    {
        // FallDown is only called if they are the local player or they are a server AI.
        netAnim.ResetTrigger("Jump");
        if (isLocalPlayer)
            CmdAnimSetTrigger("FallDown");
        else if (isAI) 
            netAnim.SetTrigger("FallDown"); 
        beingKnockedBack = false;
        movementEnabled = false;
        timeUntilMoveEnabled = cooldown;
        drop = false;
    }

    /// <summary>
    /// Handles Jump input, animations, jump sounds, physics, calls Gravity
    /// </summary>
    public void Jump()
    {
        if (!isAI && (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump")))
        {
            isJumping = true;
            netAnim.ResetTrigger("Land");
            netAnim.SetTrigger("Jump");
        }
        else if(!isAI)
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

    /// <summary>
    /// simulates gravity 
    /// </summary>
    private void Gravity()
    {
        // Gravity
        if (airborn)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else // Gravity while grounded.
        {
            controller.Move(velocity * Time.deltaTime);
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
                controller.Move(moveDir.normalized * moveSpeed * speedMultiplier * Time.deltaTime);

            }
            else
            {
                animator.SetBool("isRunning", false);
                isMoving = false;
                if (airborn == false) // TODO: is this stopping us from receiving collisions when we don't have input?
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
                controller.Move(dirVector.normalized * moveSpeed * speedMultiplier * Time.deltaTime);
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
            netAnim.ResetTrigger("Land");
            netAnim.SetTrigger("Jump");
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
                electronicObject.CmdToggleVisual(isMini);
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
                electronicObject.CmdToggleVisual(isMini);
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
            electronicObject = other.GetComponent<NetVolumeTrigger>().interact;
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
            electronicObject = other.GetComponent<NetVolumeTrigger>().interact;
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
        if (isServer) // This makes it so only the server can say when a player is hit by a knockback
        {
            RpcKnockBack(direction, _drop);
        }
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
            if(isLocalPlayer)
            {
                GameObject networkedGO = grabbedObject.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO;
                grabbedObject.GetComponentInChildren<LocalGrabbableObjectController>().LocalLetGo();
                grabbedObject = networkedGO;
                if(inStrollerRace && grabbedObject.GetComponent<StrollerController>())
                    GetComponentInChildren<StrollerLocator>().SetActive(true);
                CmdLetGo(grabbedObject.GetComponent<NetworkIdentity>().netId); 
            }
            if (GetComponentInChildren<NetAIThrowTrigger>())
            {
                GetComponentInChildren<NetAIThrowTrigger>().setCanThrow(false);
                NetDBGameManager.Instance.enlistDodgeball(grabbedObject.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO);
            }
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
        if (aimAssist)
            aimAssist.SetActive(toggle);
        else
            Debug.Log($"Aim Assist not found on {name}");
    }

    /// <summary>
    /// Handles animation and physics for throwing objects.
    /// </summary>
    /// <returns></returns>
    public void performThrow()
    {
        netAnim.ResetTrigger("Land");
        netAnim.SetTrigger("Throw");
        ThrowSound.Post(gameObject);
        GameObject networkedObj = grabbedObject.GetComponentInChildren<LocalGrabbableObjectController>().networkedGO;
        Vector3 localPos = grabbedObject.transform.position;
        Vector3 forward = transform.forward;
        grabbedObject.transform.forward = forward;
        if (hasAimAssist)
        {
            Vector3 upCompensation = Vector3.up;
            if (!isAI)
                upCompensation = Vector3.zero;
            if (networkedObj.GetComponent<NetGrabbableObjectController>().isDodgeball)
                networkedObj.GetComponent<NetGrabbableObjectController>().HomingThrow(true, target, (forward + upCompensation) * homingSpeed);
        }
        grabbedObject.GetComponentInChildren<LocalGrabbableObjectController>().LocalLetGo();
        grabbedObject = networkedObj;
        if(inStrollerRace && grabbedObject.GetComponent<StrollerController>())
            GetComponentInChildren<StrollerLocator>().SetActive(true);
        if (isLocalPlayer)
            CmdLetGo(grabbedObject.GetComponent<NetworkIdentity>().netId);
        else
            grabbedObject.GetComponent<NetGrabbableObjectController>().LocalLetGo();

        if (isServer)
        {
            grabbedObject.GetComponent<Rigidbody>().AddForce(throwingForce);
            if (grabbedObject.GetComponent<CombatThrow>())
            {
                RpcEnableKnockBack(grabbedObject.GetComponent<NetworkIdentity>().netId);
            }
        }
        else if (isLocalPlayer)
        {
            CmdThrow(grabbedObject.GetComponent<NetworkIdentity>().netId, throwingForce, localPos);
            if (grabbedObject.GetComponent<CombatThrow>())
            {
                CmdEnableKnockBack(grabbedObject.GetComponent<NetworkIdentity>().netId);
            }
        }

        if (grabbedObject.GetComponent<StrollerController>() && !isAI)
        {
            SteamAchievements.UnlockAchievement("SR_STROLLER");
        }

        hasGrabbed = false;
        moveSpeed = moveSpeedNormal;
        grabbedObject = null;
        grabbedObjectID = 0;
        netAnim.ResetTrigger("Throw");
        if (GetComponentInChildren<NetAIThrowTrigger>())
            GetComponentInChildren<NetAIThrowTrigger>().setCanThrow(false);
    }



    private void LocalEnableKnockBack(GameObject grabbedObject)
    {
        if (grabbedObject.GetComponent<CombatThrow>())
            grabbedObject.GetComponent<CombatThrow>().EnableKnockBack(name);
        else if (grabbedObject.GetComponent<NetExplodingPropane>())
            grabbedObject.GetComponent<NetExplodingPropane>().EnableKnockBack();
    }

    /// <summary>
    /// Handles all grabbing logic for the various grabbable items.
    /// </summary>
    private void HandleGrabObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
        if (pickupPressed)
        {
            foreach (Collider collider in hitColliders)
            {
                if (collider.GetComponent<NetGrabbableObjectController>()
                    && collider.GetComponent<NetGrabbableObjectController>().GetCanPickup())
                {
                    if (collider.CompareTag("Grabbable") || collider.CompareTag("ShoppingItem"))
                    {
                        if (collider.isTrigger)
                        {
                            // TODO: Changed to take fix dodgeball pickup.  If picking up isn't broken anywhere in the game, simply remove and modify if/else.
                            /*GrabSound.Post(gameObject);
                            moveSpeed = moveSpeedGrab;
                            pickupPressed = false;
                            */
                        }
                        else
                        {
                            if (collider.CompareTag("ShoppingItem"))
                            {
                                collider.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
                            }
                            animator.SetBool("isHoldingSomething", true);
                            GrabSound.Post(gameObject);
                            grabbedObjectID = collider.GetComponent<NetworkIdentity>().netId;
                            CmdPickupObject(grabbedObjectID);
                            if (!isServer)
                            {
                                grabbedObject = collider.GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform);
                                if (collider.GetComponent<StrollerController>())
                                    GetComponentInChildren<StrollerLocator>().SetActive(false);
                            }
                            if (!grabbedObject)
                                grabbedObject = collider.gameObject;
                            moveSpeed = moveSpeedGrab;
                            hasGrabbed = true;
                            pickupPressed = false;
                            if (grabbedObject.name.StartsWith("Grabpoint")) // Designed to only use Grabpoint as the name of the Utility Cart Grabbing point.
                            {
                                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                            }
                        }
                        break;
                    }
                } // The following are items without grabbable object controllers
                else if (collider.CompareTag("Player") && collider.GetComponent<NetworkCharacterMovementController>().isMini
                    && isMini == false)
                {
                    SteamAchievements.UnlockAchievement("PB_TIMEOUT");
                    CmdTimeout(collider.GetComponent<NetworkIdentity>().netId);
                }
                else if (collider.CompareTag("ShoppingCart"))
                {
                    CmdPickupFromCart(collider.transform.parent.GetComponent<NetworkIdentity>().netId);
                    GrabSound.Post(gameObject);
                    moveSpeed = moveSpeedGrab;
                    pickupPressed = false;
                    break; // Ensures players can't perform multiple pickups simultaneuosly.
                }
            }
        }
    }

    public void AIPickup(uint objID)
    {
        CmdPickupObject(objID);
        if (!isServer)
        {
            grabbedObject = NetworkServer.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform);
        }
    }

    [Command]
    private void CmdTimeout(uint kidID)
    {
        NetworkServer.spawned[kidID].GetComponent<NetKidTimeout>().RpcTimeout();
    }

    [Command]
    public void CmdDestroyObject(uint objID)
    {
        if (NetworkServer.spawned.ContainsKey(objID))
        {
            Debug.Log($"Destroying {NetworkServer.spawned[objID].name}");
            NetworkServer.Destroy(NetworkServer.spawned[objID].gameObject);
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
    private void CmdPickupFromCart(uint cartId)
    {
        uint objId = NetworkServer.spawned[cartId].GetComponent<NetCart>().GiveObject(transform);
        if (objId != 0)
            RpcPickupFromCart(objId);
    }

    [Command]
    private void CmdRemoveNetworkAuthority(NetworkIdentity item)
    {
        //RpcRemoveNetworkAuthority(item);
        item.RemoveClientAuthority();
    }

    [Command]
    private void CmdPickupObject(uint objID)
    {
        hasGrabbed = true;
        grabbedObject = NetworkServer.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform);
        RpcPickupObject(objID);
        if (NetworkServer.spawned[objID].GetComponent<ShoppingItem>())
            NetworkServer.spawned[objID].GetComponent<ShoppingItem>().SetPlayer(gameObject);
        if(NetworkServer.spawned[objID].GetComponent<StrollerController>())
            GetComponentInChildren<StrollerLocator>().SetActive(false);
    }

    [Command]
    private void CmdLetGo(uint objID)
    {
        if (NetworkServer.spawned[objID])
        {
            //NetworkServer.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalLetGo();
            RpcLetGo(objID);
        } else
        {
            Debug.LogError("Network Identity not found : " + objID);
        }
    }

    [Command]
    private void CmdThrow(uint objID, Vector3 throwingForce, Vector3 itemPosition)
    {
        Debug.Log("Command Throw");
        NetworkServer.spawned[objID].GetComponent<Rigidbody>().isKinematic = false;
        NetworkServer.spawned[objID].GetComponent<Rigidbody>().useGravity = true;
        NetworkServer.spawned[objID].transform.position = itemPosition;
        NetworkServer.spawned[objID].GetComponent<Rigidbody>().AddForce(throwingForce);
    }

    [Command]
    private void CmdEnableKnockBack(uint grabbedObjectID)
    {
        RpcEnableKnockBack(grabbedObjectID);
    }


    /*[Command] //TODO: Delete this if nothing's broken with animations
    private void CmdAnimSetBool(string varName, bool tf)
    {
        animator.SetBool(varName, tf);
        RpcAnimSetBool(varName, tf);
    }*/


    [Command]
    private void CmdAnimSetTrigger(string varName)
    {
        netAnim.SetTrigger(varName);
        RpcAnimSetTrigger(varName);
    }


    [Command]
    public void CmdSetFinishPosition(int pos)
    {
        finishPosition = pos;
    }

    /// <summary>
    /// Called on all clients after knockback happens on the server.
    /// Only servers and local players have authority over their objects.
    /// </summary>
    /// <param name="_direction"></param>
    /// <param name="_drop"></param>
    [ClientRpc]
    private void RpcKnockBack(Vector3 _direction, bool _drop)
    {
        if (isServer || isLocalPlayer)
        {
            Debug.Log($"KBCalled on {name} with: {_direction}");
            knockBackCounter = knockBackTime;
            beingKnockedBack = true;
            drop = _drop;
            if (_drop)
            {
                DropGrabbedItem();
                if (cameraShake)
                    StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
                if (GetComponent<DodgeballAI>())
                { // Only knockbacks that make the AI drop an item will affect it.  This allows navmesh link traversal over trampolines.
                    GetComponent<DodgeballAI>().Loiter(true);
                    StartCoroutine(KnockbackAI(_direction));
                }
            }
            velocity = _direction;
        }
    }

    [ClientRpc]
    private void RpcPickupFromCart(uint netId)
    {
        GameObject takenObject = NetworkClient.spawned[netId].gameObject;
        animator.SetBool("isHoldingSomething", true);
        grabbedObject = takenObject.GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform);
        grabbedObjectID = netId;
        takenObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
        hasGrabbed = true;
    } 

    [ClientRpc]
    private void RpcPickupObject(uint objID)
    {
        if (isServer || isLocalPlayer)
            return;
        Debug.Log("RpcPickupObject: " + objID);
        hasGrabbed = true;
        grabbedObject = NetworkClient.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform);
        if (NetworkClient.spawned[objID].GetComponent<StrollerController>())
            GetComponentInChildren<StrollerLocator>().SetActive(false);
    }

    [ClientRpc]
    private void RpcLetGo(uint objID)
    {
        if (isLocalPlayer)
            return;
        if (NetworkClient.spawned.ContainsKey(objID))
        {
            GameObject releasedObj = NetworkClient.spawned[objID].GetComponent<NetGrabbableObjectController>().LocalLetGo();
            if(inStrollerRace && releasedObj.GetComponent<StrollerController>())
                GetComponentInChildren<StrollerLocator>().SetActive(true);
        } 
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
    private void RpcEnableKnockBack(uint grabbedObjectID)
    {
        LocalEnableKnockBack(NetworkClient.spawned[grabbedObjectID].gameObject);
    }


    [ClientRpc]
    private void RpcAnimSetBool(string varName, bool tf)
    {
        animator.SetBool(varName, tf);
    }


    [ClientRpc]
    private void RpcAnimSetTrigger(string varName)
    {
        netAnim.SetTrigger(varName);
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
            savedMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
            GetComponentInChildren<SkinnedMeshRenderer>().material = kidsMaterials[GetColorIndex(color)];
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

    private IEnumerator KnockbackAI(Vector3 _direction)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        agent.enabled = false;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.velocity = _direction;
        yield return new WaitForFixedUpdate();
        float initTime = Time.time;
        yield return new WaitUntil(() => Time.time > knockBackTime + initTime);
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        agent.Warp(transform.position);
        agent.enabled = true;
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

    public void setSpeedMultiplier(float _mult)
    {
        speedMultiplier = _mult;
    }

    /// <summary>
    /// Activates the animations for holding an object, gives the player a ref to the object, and adjusts physics.
    /// Should only be called on objects with NetworkIdentities
    /// </summary>
    /// <param name="obj"></param>
    public void SetGrabbedObjectNet(GameObject obj)
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

    /// <summary>
    /// Sets the grabbedObject variable to the local object. 
    /// Only applies to Local versions of networked objects.
    /// </summary>
    /// <param name="obj"></param>
    public void SetGrabbedObjectLocal(GameObject obj)
    {
        grabbedObject = obj;
    }

    public GameObject GetGrabbedObject()
    {
        return grabbedObject;
    }
    
    public void SetColorName(string colorName)
    { 
        color = colorName;
        if(isLocalPlayer)
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

    public struct PlayerInfo
    {
        public string color;
        public int id;
        public int finishPos;

        public PlayerInfo(string _color, int _id, int _finishPos)
        {
            color = _color;
            id = _id;
            finishPos = _finishPos;
        }
    }
}
