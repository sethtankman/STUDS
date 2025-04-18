﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class CharacterMovementController : MonoBehaviour
{
    public AK.Wwise.Event JumpSound;
    public AK.Wwise.Event GrabSound;
    public AK.Wwise.Event ThrowSound;
    public AK.Wwise.Event RunSound;

    private CharacterController controller;

    public Transform camPos;
    public Animator animator;
    public Renderer renderer;
    public CameraShake cameraShake;

    public GameObject playerCamera;
    public GameObject binky;
    public GameObject target;
    public GameObject aimAssist;

    public float moveSpeed; // Current move speed (changes dynamically)
    public float moveSpeedKid; // Set movespeed for kid
    private float knockBackCounter;
    private float pickupCooldown;
    private float stepSoundCooldown;

    public float gravity;
    public float jumpHeight;
    public float moveSpeedGrab; // Set movespeed for grab
    public float moveSpeedNormal; // Set movespeed for normal
    /// <summary>
    /// Designed only to be modified by mud, water, etc. through SpeedUpSlowDown.cs
    /// </summary>
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private float turnTime;
    public float jumpHeightGrab;
    public float throwForce;
    [SerializeField] private float homingSpeed;
    public float throwCoolDown = 0;
    public float knockBackTime;
    public float timeUntilMoveEnabled = 0;

    public Vector3 velocity;

    /// <summary>
    /// Stores the direction the user is inputing through their controls.
    /// </summary>
    private Vector2 direction;
    private Vector3 throwingForce;

    float turnSmoothVelocity;

    private GameObject grabbedObject;
    private GameObject electronicObject;

    private Material savedMaterial;
    public Material[] kidsMaterials;

    private bool hasGrabbed = false;
    private bool pickupPressed;
    private bool throwPressed;
    private bool isReady;
    private bool drop;

    public bool isMini = false;
    public bool isJumping = false;
    public bool airborn;
    public bool beingKnockedBack;
    public bool movementOnCooldown = false;
    public bool isMoving;
    public bool isAI = false;

    private int playerID;
    private int completedCheckpoints;
    public int finishPosition;

    private string selectedLevel;
    public string color;

    //Particle effects
    //private bool isBlinking;
    public bool hasAimAssist;
    private PLR_ParticleController PlayerParticles;

    public bool CanJump;
    public bool CanMove = true;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (movementOnCooldown == false && !isAI)
            Move();
        else
        {
            timeUntilMoveEnabled -= Time.deltaTime;
            if (timeUntilMoveEnabled < 0)
            {
                timeUntilMoveEnabled = 0;
                movementOnCooldown = false;
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

        ThrowAndPickup();
    }

    private void ThrowAndPickup()
    {
        if (knockBackCounter <= 0)
        {
            //Handle moving and rotating the object that has been grabbed
            if (hasGrabbed)
            {
                if (grabbedObject)
                {
                    float grabDistance = 1.3f;
                    float grabHeight = 0.7f;
                    if (grabbedObject.GetComponent<GrabbableObjectController>())
                    {
                        grabDistance = grabbedObject.GetComponent<GrabbableObjectController>().distance;
                        grabHeight = grabbedObject.GetComponent<GrabbableObjectController>().height;
                    }
                    grabbedObject.transform.position = transform.position + (transform.forward * grabDistance) + (transform.up * grabHeight);
                    grabbedObject.transform.rotation = transform.rotation;

                    //If player released "e" then let go
                    if (!isAI)
                    {
                        Vector3 forward = transform.forward;
                        grabbedObject.transform.forward = forward;
                        if (hasAimAssist)
                        {
                            forward = Vector3.Normalize(target.transform.position - transform.position);
                        }
                        throwingForce = forward * (throwForce + (direction.magnitude * moveSpeedGrab * 40));
                        throwingForce.y = 300f;

                        ThrowSpline spline = grabbedObject.GetComponentInChildren<ThrowSpline>();
                        if (spline)
                        {
                            spline.SetThrowForce(throwingForce, hasAimAssist);
                        }
                        if (pickupPressed)
                        {
                            GrabSound.Post(gameObject);
                            DropGrabbedItem();
                            pickupPressed = false;
                            animator.SetBool("isHoldingSomething", false);
                        }
                        else if (throwPressed && throwCoolDown <= 0)
                        {
                            performThrow();
                        }
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
            if (Physics.Raycast(ray, out hit, 0.1f) && beingKnockedBack && drop)
            {
                if (hit.transform.tag == "Ground")
                {
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("Land");
                    animator.SetTrigger("FallDown");
                    beingKnockedBack = false;
                    velocity.x = 0;
                    velocity.z = 0;
                    airborn = false;
                    movementOnCooldown = true;
                    timeUntilMoveEnabled = 1.5f;
                    drop = false;
                }
            }
            else if (Physics.Raycast(ray, out hit, 0.1f))
            {

                animator.ResetTrigger("Jump");
                animator.SetTrigger("Land");
                velocity.x = 0;
                velocity.z = 0;
                airborn = false;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 0.1f) == false || hit.transform.tag != "Ground")
            {
                airborn = true;
            }
            else if (Physics.Raycast(ray, out hit, 0.1f) && beingKnockedBack && drop)
            {
                animator.ResetTrigger("Jump");
                animator.ResetTrigger("Land");
                animator.SetTrigger("FallDown");
                beingKnockedBack = false;
                movementOnCooldown = true;
                drop = false;
                timeUntilMoveEnabled = 1.3f;
            }
        }
    }

    /// <summary>
    /// Handles Jumping and gravity
    /// </summary>
    public void Jump()
    {
        if (airborn == false && isJumping && CanJump )
        {
            if (!isAI)
            {
                JumpSound.Post(gameObject);
                //jumpSound.Play();
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
        if (!gameObject.GetComponent<CharacterController>().isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            if(controller.enabled)
                controller.Move(velocity * Time.deltaTime); 
        }
        else
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
            Vector3 dirVector = new Vector3(direction.x, 0f, direction.y).normalized;

            if (dirVector.magnitude >= 0.1f)
            {

                animator.SetBool("isRunning", true);
                isMoving = true;
                float targetAngle = Mathf.Atan2(dirVector.x, dirVector.z) * Mathf.Rad2Deg + camPos.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move((moveDir.normalized * moveSpeed * speedMultiplier) * Time.deltaTime);

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

    private void PlaySteps()
    {
        float timeBetween = 0.25f;
        if (CanMove && isMoving && !airborn && stepSoundCooldown < 0)
        {
            RunSound.Post(gameObject);
            stepSoundCooldown = timeBetween;
        }
        else
        {
            stepSoundCooldown -= Time.deltaTime;
        }
    }

    public void Move(Vector3 dirVector)
    {
        if (CanMove)
        {
            if (dirVector.magnitude >= 0.1f && movementOnCooldown == false)
            {
                animator.SetBool("isRunning", true);
                //runSound.Play();
                controller.Move(dirVector.normalized * moveSpeed * speedMultiplier * Time.deltaTime);

            }
            else
            {
                animator.SetBool("isRunning", false);
                //runSound.Stop();
                velocity.x = 0;
                velocity.z = 0;
            }
        }


    }

    public void OnMove(CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (PlayerParticles)
            PlayerParticles.TurnOnRunning();
    }

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
            SceneManager.LoadScene("TheBlock_LevelSelect");
        }
    }

    public void OnPickup(CallbackContext context)
    {
        if (context.performed)
        {
            pickupPressed = true;
            if (electronicObject != null)
            {
                electronicObject.GetComponent<Interaction>().ToggleVisual(isMini);
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
                electronicObject.GetComponent<Interaction>().ToggleVisual(isMini);
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
            Debug.Log("ElectronicObject Found");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Electronics"))
        {
            electronicObject = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Electronics"))
        {
            electronicObject = null;
            Debug.Log("Electronic Object lost");
        }
    }

    public void KnockBack(Vector3 _direction, bool _drop)
    {
        Debug.Log("KBCalled with: " + _direction.ToString());
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

    private IEnumerator KnockbackAI(Vector3 _direction)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        agent.enabled = false;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.linearVelocity = _direction;
        yield return new WaitForFixedUpdate();
        float initTime = Time.time;
        yield return new WaitUntil(() => Time.time > knockBackTime + initTime);
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        agent.Warp(transform.position);
        agent.enabled = true;
    }

    public void DropGrabbedItem()
    {
        if (grabbedObject)
        {
            animator.SetBool("isHoldingSomething", false);
            if (grabbedObject.CompareTag("ShoppingCart"))
            {
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            }
            if (GetComponentInChildren<AIThrowTrigger>())
            {
                GetComponentInChildren<AIThrowTrigger>().setCanThrow(false);
                DBGameManager.Instance.enlistDodgeball(grabbedObject);
            }
            grabbedObject.GetComponent<GrabbableObjectController>().LetGo();
            grabbedObject = null;
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
            Debug.Log($"No aim assist on {name}");
    }

    
    public void performThrow()
    {
        animator.ResetTrigger("Land");
        animator.SetTrigger("Throw");
        ThrowSound.Post(gameObject);
        Vector3 forward = transform.forward;
        grabbedObject.transform.forward = forward;
        if (hasAimAssist)
        {
            forward = Vector3.Normalize(target.transform.position - transform.position);
            Vector3 upCompensation = Vector3.up;
            if (!isAI)
                upCompensation = Vector3.zero;
            if (grabbedObject.GetComponent<GrabbableObjectController>().isDodgeball)
                grabbedObject.GetComponent<GrabbableObjectController>().HomingThrow(true, target, (forward+upCompensation)*homingSpeed);
        }
        grabbedObject.GetComponent<GrabbableObjectController>().LetGo();
        grabbedObject.GetComponent<Rigidbody>().AddForce(throwingForce);

        if (grabbedObject.GetComponent<CombatThrow>())
        {
            grabbedObject.GetComponent<CombatThrow>().EnableKnockBack();
        }
        if (grabbedObject.GetComponent<StrollerController>())
        {
            SteamAchievements.UnlockAchievement("SR_STROLLER");
        }
        
        hasGrabbed = false;
        moveSpeed = moveSpeedNormal;
        grabbedObject = null;
        animator.ResetTrigger("Throw");
        animator.SetBool("isHoldingSomething", false);
        throwCoolDown = 1;
        if (GetComponentInChildren<AIThrowTrigger>())
            GetComponentInChildren<AIThrowTrigger>().setCanThrow(false);
    }

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

                if (pickupPressed && !hasGrabbed && collider.GetComponent<GrabbableObjectController>() 
                    && collider.GetComponent<GrabbableObjectController>().getCanPickup())
                {
                    animator.SetBool("isHoldingSomething", true);
                    GrabSound.Post(gameObject);
                    grabbedObject = collider.gameObject;
                    grabbedObject.GetComponent<GrabbableObjectController>().PickupObject(color, true);
                    moveSpeed = moveSpeedGrab;
                    hasGrabbed = true;
                    pickupPressed = false;
                    if (grabbedObject.name == "Grabpoint") // Designed to only use Grabpoint as the name of the Utility Cart Grabbing point.
                    {
                        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
            else if (collider.CompareTag("Player") && collider.gameObject.GetComponent<CharacterMovementController>().isMini)
            {
                if (pickupPressed && isMini == false)  // This is just making it so timeout doesn't work...
                {
                    SteamAchievements.UnlockAchievement("PB_TIMEOUT");
                    Debug.Log("Calling timeout");
                    collider.gameObject.GetComponent<KidTimeout>().Timeout(collider.gameObject);
                }
            }
            else if (collider.CompareTag("ShoppingCart") && pickupPressed && !hasGrabbed)
            {
                GameObject givenObject = collider.GetComponent<ShoppingCartController>().GiveObject();
                if (givenObject)
                {
                    GameObject takenObject = Instantiate(givenObject, gameObject.transform.position + (transform.forward * 1.3f) + (transform.up * 0.7f), Quaternion.identity);
                    takenObject.GetComponent<GrabbableObjectController>().PickupObject(color);
                    grabbedObject = takenObject;
                    takenObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
                    animator.SetBool("isHoldingSomething", true);
                    GrabSound.Post(gameObject);
                    moveSpeed = moveSpeedGrab;
                    hasGrabbed = true;
                    pickupPressed = false;
                }

            }
        }
    }

    public void SetToMini(bool setMini)
    {
        if (setMini)
        {
            transform.localScale = new Vector3(20, 20, 20); //Shrink the player. OG size is 30, 30, 30
            SetBinky(true); //Activate the binky!!!!
            isMini = true; //The Eugine will now act as a child.
            moveSpeed = moveSpeedKid;
            savedMaterial = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            Debug.Log($"Color: {color}, Index: {GetColorIndex(color)}");
            if(GetColorIndex(color) != -1)
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = kidsMaterials[GetColorIndex(color)];
            else
            {
                Debug.LogWarning($"Color not found: {color}");
            }
        } else
        {
            transform.localScale = new Vector3(30, 30, 30); // size is 30, 30, 30
            SetBinky(false); // Deactivate Binky
            isMini = false; //The Eugine will now act as a dad.
            moveSpeed = moveSpeedNormal;
            if(savedMaterial)
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = savedMaterial;
        }
    }

    private int GetColorIndex(string _color)
    {
        switch(_color)
        {
            case "red":
                return 0;
            case "blue":
                return 1;
            case "purple":
                return 2;
            case "yellow":
                return 3;
            case "green":
                return 4;
            default:
                return -1;

        }
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

    public void SetGrabbedObject(GameObject obj)
    {
        animator.SetBool("isHoldingSomething", true);
        grabbedObject = obj;
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
        moveSpeed = moveSpeedGrab;
        hasGrabbed = true;
        pickupPressed = false;
    }

    public GameObject GetGrabbedObject()
    {
        return grabbedObject;
    }

    public void SetColorName(string colorName)
    {
        Debug.Log("Setting character color: " + colorName);
        color = colorName;
        //GameObject gameManager = GameObject.Find("GameManager");
        //gameManager.GetComponent<ManagePlayerHub>().ChangePlayerColor(playerID, colorName);
    }

    public string GetColorName()
    {
        return color;
    }

    public void ReadyPlayer(bool _isReady, string _selectedLevel)
    {
        isReady = _isReady;
        selectedLevel = _selectedLevel;
    }

    public bool GetReadyPlayer(string _selectedLevel)
    {
        if(_selectedLevel.Equals("ManagePlayerHub"))
        {
            return isReady;
        }
        return isReady && _selectedLevel == selectedLevel;
    }

    public void SetFinishPosition(int pos)
    {
        finishPosition = pos;
    }

    public int GetFinishPosition()
    {
        return finishPosition;
    }

    public void SetBinky(bool isActive)
    {
        binky.SetActive(isActive);
    }

    public CharacterController GetController()
    {
        return controller;
    }
}
