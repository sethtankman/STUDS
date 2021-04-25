using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class CharacterMovementController : MonoBehaviour
{
    public AK.Wwise.Event JumpSound;
    public AK.Wwise.Event GrabSound;
    public AK.Wwise.Event ThrowSound;
    public AK.Wwise.Event RunSound;

    private CharacterController controller;

    public SteamAchievements sa;

    public Transform camPos;
    public Animator animator;
    public Renderer renderer;
    public CameraShake cameraShake;

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
    public bool movementEnabled = true;
    public bool isMoving;
    public bool isAI = false;

    private int playerID;
    private int completedCheckpoints;
    private int finishPosition;

    private String color;

    //Particle effects
    //private bool isBlinking;
    public bool hasAimAssist;
    private PLR_ParticleController PlayerParticles;

    public bool CanJump;

    public bool CanMove;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = moveSpeedNormal;
        direction = new Vector2();
        isReady = false;
        finishPosition = -1;
        controller = GetComponent<CharacterController>();
        PlayerParticles = GetComponent<PLR_ParticleController>();
        CanJump = true;
        CanMove = true;
        sa = GameObject.Find("SteamAchievements").GetComponent<SteamAchievements>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player: " + playerID + " has object: " + hasGrabbed);
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
            //Handle moving and rotating the object that has been grabbed
            if (hasGrabbed)
            {
                float grabDistance = 1.3f;
                float grabHeight = 0.7f;
                //Quaternion grabRotation = Quaternion.; //Maybe we'll need this for the hammer in shopping spree
                if(grabbedObject.GetComponent<GrabbableObjectController>())
                {
                    grabDistance = grabbedObject.GetComponent<GrabbableObjectController>().distance;
                    grabHeight = grabbedObject.GetComponent<GrabbableObjectController>().height;
                }
                grabbedObject.transform.position = transform.position + (transform.forward * grabDistance) + (transform.up * grabHeight);
                grabbedObject.transform.rotation = transform.rotation;
                //grabbedObject.transform.rotation *= Quaternion.Euler(0, 90, 0);
                //grabbedObject.transform.LookAt(transform.position);

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
                    movementEnabled = false;
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
                movementEnabled = false;
                drop = false;
                timeUntilMoveEnabled = 1.3f;
            }
        }
    }

    public void Jump()
    {
        //GameObject menu = GameObject.Find("GameManager");
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
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            velocity.y = 0;
        }
        //Debug.Log("velocity y is: " + velocity.y);
    }

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

    public void Move(Vector3 dirVector)
    {
        if (CanMove)
        {
            if (dirVector.magnitude >= 0.1f && movementEnabled)
            {
                animator.SetBool("isRunning", true);
                //runSound.Play();
                controller.Move((dirVector.normalized * moveSpeed) * Time.deltaTime);

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
        //else
            //Debug.LogError("Running Particle effect not assigned.");
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

    /*
    void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.tag == "Ground")
        {
            velocity.x = 0;
            velocity.z = 0;
        }
    } */

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Electronics"))
        {
            electronicObject = other.transform.parent.gameObject;
            Debug.Log("ElectronicObject Found");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Electronics"))
        {
            electronicObject = other.transform.parent.gameObject;
            //Debug.Log("Assign to variable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Electronics"))
        {
            electronicObject = null;
            Debug.Log("Electronic Object lost");
        }
    }

    public void KnockBack(Vector3 direction, bool _drop)
    {
        Debug.Log("KBCalled with: " + direction.ToString());
        knockBackCounter = knockBackTime;
        beingKnockedBack = true;
        drop = _drop;
        if (_drop)
        {
            DropGrabbedItem();
            if(cameraShake)
                StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
        }
        velocity = direction;

    }

    public void DropGrabbedItem()
    {
        if (grabbedObject)
        {
            animator.SetBool("isHoldingSomething", false);
            if (grabbedObject.tag.Equals("ShoppingCart"))
            {
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
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
        aimAssist.SetActive(toggle);
    }

    /*
    public void Blink(bool _isBlinking)
    {
        isBlinking = _isBlinking;
        if (isBlinking)
        {
            //Debug.Log("Blink...Start!");
            StartCoroutine(BlinkC());
        }
        else
        {
            //Debug.Log("Blink...Stop.");
            StopCoroutine(BlinkC());
            renderer.material.SetColor("_Color", Color.white);
        }
    }

    private IEnumerator BlinkC()
    {
        while (isBlinking)
        {
            renderer.material.SetColor("_Color", Color.red);
            yield return new WaitForSeconds(0.5f);
            renderer.material.SetColor("_Color", Color.white);
            yield return new WaitForSeconds(0.5f);
        }
    }
    */
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
            //Debug.Log("Resetting forward vector");
            forward = Vector3.Normalize(target.transform.position - transform.position);
        }
        Vector3 throwingForce = forward * throwForce * 2.3f;
        Vector3 movementAdjust = forward * direction.magnitude * moveSpeedGrab * 40;
        throwingForce += movementAdjust;
        throwingForce.y = 300f;
        //Debug.Log("Threw with Vector force: " + throwingForce);
        grabbedObject.GetComponent<GrabbableObjectController>().LetGo();
        //Debug.Log("Velocity: " + direction.magnitude);
        grabbedObject.GetComponent<Rigidbody>().AddForce(throwingForce);

        if (grabbedObject.GetComponent<CombatThrow>())
        {
            grabbedObject.GetComponent<CombatThrow>().EnableKnockBack();
        }
        if (grabbedObject.GetComponent<StrollerController>())
        {
            sa.UnlockAchievement("SR_STROLLER");
        }
        
        hasGrabbed = false;
        moveSpeed = moveSpeedNormal;
        grabbedObject = null;
        animator.ResetTrigger("Throw");
    }

    /*
    private IEnumerator runSound()
    {
        runSound.Play();
    }
    */

    private void HandleGrabObject()
    {
        bool foundGrabbable = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider collider in hitColliders)
        {
            if (collider.tag == "Grabbable" || collider.tag == "ShoppingItem")
            {
                if (collider.tag == "ShoppingItem")
                {
                    collider.gameObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
                }
                foundGrabbable = true;
                if (hasGrabbed)
                {
                    //GrabText.text = "";
                }
                else
                {
                    //GrabText.text = "Press e to grab this object";
                }

                if (pickupPressed && !hasGrabbed)
                {
                    animator.SetBool("isHoldingSomething", true);
                    GrabSound.Post(gameObject);
                    //grabSound.Play();
                    grabbedObject = collider.gameObject;
                    grabbedObject.GetComponent<GrabbableObjectController>().PickupObject();
                    moveSpeed = moveSpeedGrab;
                    hasGrabbed = true;
                    pickupPressed = false;
                    if (grabbedObject.name == "Grabpoint") // Designed to only use Grabpoint as the name of the Utility Cart Grabbing point.
                    {
                        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
            else if (collider.tag == "Player" && collider.gameObject.GetComponent<CharacterMovementController>().isMini)
            {
                // Debug.Log("Collided with child");
                if (pickupPressed && isMini == false)  // This is just making it so timeout doesn't work...
                {
                    collider.gameObject.GetComponent<KidTimeout>().Timeout(collider.gameObject);
                }
            }
            else if (collider.tag == "ShoppingCart" && pickupPressed && !hasGrabbed)
            {
                GameObject givenObject = collider.GetComponent<ShoppingCartController>().GiveObject();
                if (givenObject)
                {
                    GameObject takenObject = Instantiate(givenObject, gameObject.transform.position + (transform.forward * 1.3f) + (transform.up * 0.7f), Quaternion.identity);
                    takenObject.GetComponent<GrabbableObjectController>().PickupObject();
                    grabbedObject = takenObject;
                    takenObject.GetComponent<ShoppingItem>().SetPlayer(this.gameObject);
                    foundGrabbable = true;
                    animator.SetBool("isHoldingSomething", true);
                    GrabSound.Post(gameObject);
                    moveSpeed = moveSpeedGrab;
                    hasGrabbed = true;
                    pickupPressed = false;
                }

            }
        }
        if (!foundGrabbable)
        {
            //GrabText.text = "";
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
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = kidsMaterials[GetColorIndex(color)];
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
            case "Red":
                return 0;
            case "Blue":
                return 1;
            case "Purple":
                return 2;
            case "Yellow":
                return 3;
            case "Green":
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

    public void SetColorName(String colorName)
    {
        color = colorName;
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<ManagePlayerHub>().ChangePlayerColor(playerID, colorName);
    }

    public String GetColorName()
    {
        return color;
    }

    public void ReadyPlayer(bool _isReady)
    {
        isReady = _isReady;
    }

    public bool GetReadyPlayer()
    {
        return isReady;
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
