using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Controls the AI for dodgeball
/// 
/// Used https://github.com/SunnyValleyStudio/Diablo-Like-Movement-in-Unity-using-AI-Navigation-package/blob/main/AgentMover.cs 
/// For ideas on how to get navmesh jumping to look better.
/// </summary>
public class DodgeballAI : MonoBehaviour
{
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    [SerializeField] private bool hasTarget = false;
    /// <summary>
    /// Used so the AI will routinely repath to moving players
    /// </summary>
    [SerializeField] private bool coroutineOn = false;
    [SerializeField] private bool _onNavMeshLink = false;
    private bool agentHasPath = false;
    private bool agentPathPending = false;

    private float turnSpeed = 2;
    [SerializeField] private int loiter = 540, patience, maxPatience;
    [SerializeField] private float _jumpDuration = 0.8f;

    private void Start()
    {
        // We disable the character controller so it doesn't override the navmesh agent.
        GetComponent<CharacterController>().enabled = false;
        agent.autoTraverseOffMeshLink = false;
    }

    private void FixedUpdate()
    {
        loiter--;
        patience--;
    }

    // Update is called once per frame
    void Update()
    {
        agentHasPath = agent.hasPath;
        agentPathPending = agent.pathPending;
        if (loiter > 0)
            return;
        if (!hasTarget) { 
            if (!GetComponent<CharacterMovementController>().GetHasGrabbed()) {
                AcquireTargetDodgeball();
            } else
            {
                AcquireTargetPlayer();
            }
        } else if (patience < 0)
        {
            hasTarget = false;
            coroutineOn = false;
        } else if (agent.hasPath)
        {
            Vector3 lookPos;
            Quaternion targetRot;


            lookPos = agent.desiredVelocity;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            if(agent.isOnOffMeshLink && _onNavMeshLink == false)
            {
                StartNavMeshLinkMovement();
            }
        }
    }

    private void StartNavMeshLinkMovement()
    {
        _onNavMeshLink = true;
        NavMeshLink link = (NavMeshLink)agent.navMeshOwner;
        Spline spline = link.GetComponentInChildren<Spline>();
        PerformJump(link, spline);
    }

    private void PerformJump(NavMeshLink link, Spline spline)
    {
        bool reverseDirection = CheckIfJumpingFromEndToStart(link);
        StartCoroutine(MoveOnOffMeshLink(spline, reverseDirection));

        animator.SetTrigger("Jump");
    }

    private bool CheckIfJumpingFromEndToStart(NavMeshLink link)
    {
        Vector3 startPosWorld
            = link.gameObject.transform.TransformPoint(link.startPoint);
        Vector3 endPosWorld
            = link.gameObject.transform.TransformPoint(link.endPoint);

        float distancePlayerToStart
            = Vector3.Distance(agent.transform.position, startPosWorld);
        float distancePlayerToEnd
            = Vector3.Distance(agent.transform.position, endPosWorld);


        return distancePlayerToStart > distancePlayerToEnd;
    }

    private IEnumerator MoveOnOffMeshLink(Spline spline, bool reverseDirection)
    {
        float currentTime = 0;
        Vector3 agentStartPosition = agent.transform.position;

        while (currentTime < _jumpDuration)
        {
            currentTime += Time.deltaTime;

            float amount = Mathf.Clamp01(currentTime / _jumpDuration);
            amount = reverseDirection ? 1 - amount : amount;

            agent.transform.position =
                reverseDirection ?
                spline.CalculatePositionCustomEnd(amount, agentStartPosition)
                : spline.CalculatePositionCustomStart(amount, agentStartPosition);

            yield return new WaitForEndOfFrame();
        }

        agent.CompleteOffMeshLink();
        animator.SetTrigger("Jump");

        yield return new WaitForSeconds(0.1f);
        _onNavMeshLink = false;

    }

    private void AcquireTargetPlayer()
    {
        
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        players.Remove(gameObject);
        GameObject[] candidates = players.ToArray();
        target = candidates[Random.Range(0, candidates.Length)].transform;
        hasTarget = true;
        patience = maxPatience;
        animator.SetBool("isRunning", true);
        coroutineOn = true;
        StartCoroutine(FollowTarget(target));
    }

    private IEnumerator FollowTarget(Transform target)
    {
        Vector3 previousTargetPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity);

        while (coroutineOn && Vector3.SqrMagnitude(transform.position - target.position) > 0.1f)
        {
            // did target move more than at least a minimum amount since last destination set?
            if (Vector3.SqrMagnitude(previousTargetPosition - target.position) > 0.1f)
            {
                agent.SetDestination(target.position);
                previousTargetPosition = target.position;
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    private void AcquireTargetDodgeball()
    {
        List<GameObject> dodgeballs = DBGameManager.Instance.GetAvailableDodgeballs();
        if (dodgeballs.Count == 0)
        {
            Debug.LogWarning("Dodgeball list is empty");
            Loiter(false);
            return;
        }
        if (target == null)
        {
            target = dodgeballs[Random.Range(0, dodgeballs.Count)].transform;
            DBGameManager.Instance.deListDodgeball(target.gameObject, gameObject);
        }
        patience = maxPatience;
        animator.SetBool("isRunning", true);
        if (_onNavMeshLink == false)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
                hasTarget = true;
            } 
            else
            { // Return navmesh agents to navmesh after being knocked back.
                NavMeshHit hit;
                NavMesh.SamplePosition(transform.position, out hit, 150, NavMesh.AllAreas);
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GetComponent<CharacterMovementController>().GetHasGrabbed() && other.CompareTag("Grabbable")
            && other.GetComponent<GrabbableObjectController>().getCanPickup()
            && target != null && other.name.Equals(target.name))
        {
            GetComponent<CharacterMovementController>().SetGrabbedObject(other.gameObject);
            other.GetComponent<GrabbableObjectController>().PickupObject(GetComponent<CharacterMovementController>().GetColorName());
            hasTarget = false;
            GetComponentInChildren<AIThrowTrigger>().setCanThrow(true);
        }
    }

    /// <summary>
    /// Called to reset targets and have AI stop for a second.
    /// </summary>
    public void Loiter(bool enlistDB)
    {
        loiter = 60;
        hasTarget = false;
        coroutineOn = false;
        if (target && target.GetComponent<GrabbableObjectController>() && enlistDB)
        {
            DBGameManager.Instance.enlistDodgeball(target.gameObject);
        }
        target = null;
        animator.SetBool("isRunning", false);
    }

    public bool CompareTarget(GameObject _target)
    {
        if (target)
            return _target.name == target.name;
        else
            return false;
    }
}
