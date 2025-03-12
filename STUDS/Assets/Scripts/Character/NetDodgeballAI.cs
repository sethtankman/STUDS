using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Mirror;

/// <summary>
/// Controls the AI for dodgeball
/// 
/// Used https://github.com/SunnyValleyStudio/Diablo-Like-Movement-in-Unity-using-AI-Navigation-package/blob/main/AgentMover.cs 
/// For ideas on how to get navmesh jumping to look better.
/// </summary>
public class NetDodgeballAI : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterMovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    private Vector3 targetPosition;

    [SerializeField] private bool hasTarget = false;
    /// <summary>
    /// Used so the AI will routinely repath to moving players
    /// </summary>
    [SerializeField] private bool coroutineOn = false;
    [SerializeField] private bool _onNavMeshLink = false;

    private float turnSpeed = 2;
    [SerializeField] private int loiter = 540, patience, maxPatience;
    [SerializeField] private float _jumpDuration = 0.8f;

    private void Start()
    {
        // We disable the character controller so it doesn't override the navmesh agent.
        GetComponent<CharacterController>().enabled = false;
    }

    private void FixedUpdate()
    {
        patience--;
    }

    // Update is called once per frame
    void Update()
    {
        loiter--;
        if (!isServer || loiter > 0)
            return;
        if (!hasTarget) { // Agent finds target
            if (!GetComponent<NetworkCharacterMovementController>().GetHasGrabbed()) {
                AcquireTargetDodgeball();
            } else
            {
                AcquireTargetPlayer();
            }
        } else if (patience < 0) // Agent runs out of patience trying to get target.
        {
            hasTarget = false;
            coroutineOn = false;
        } else if (agent.hasPath) // Agent pursues target
        {
            Vector3 lookPos;
            Quaternion targetRot;


            lookPos = agent.desiredVelocity;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

        } else if (!agent.pathPending) // Agent gives up finding a path to target.
        {
            Loiter(true);
        }
    }

    private void AcquireTargetPlayer()
    {
        
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        players.Remove(gameObject);
        GameObject[] candidates = players.ToArray();
        target = candidates[Random.Range(0, candidates.Length)].transform;
        targetPosition = target.position;
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
                targetPosition = target.position;
                agent.SetDestination(targetPosition);
                previousTargetPosition = targetPosition;
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    private void AcquireTargetDodgeball()
    {
        List<GameObject> dodgeballs = NetDBGameManager.Instance.GetAvailableDodgeballs();
        if (dodgeballs.Count == 0)
        {
            Debug.LogWarning("Dodgeball list is empty");
            Loiter(false);
            return;
        }
        if (target == null)
        {
            NavMeshHit hit;
            target = dodgeballs[Random.Range(0, dodgeballs.Count)].transform;
            NavMesh.SamplePosition(target.position, out hit, 150, NavMesh.AllAreas);
            targetPosition = hit.position;
            NetDBGameManager.Instance.deListDodgeball(target.gameObject, gameObject);
        }
        patience = maxPatience;
        animator.SetBool("isRunning", true);
        if (_onNavMeshLink == false)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(targetPosition);
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
        if (isServer && !GetComponent<NetworkCharacterMovementController>().GetHasGrabbed() && other.CompareTag("Grabbable")
            && other.GetComponent<NetGrabbableObjectController>().GetCanPickup()
            && target != null && other.name.Equals(target.name))
        {
            GetComponent<NetworkCharacterMovementController>().SetGrabbedObjectNet(other.gameObject);
            GetComponent<NetworkCharacterMovementController>().SetGrabbedObjectLocal(
                other.GetComponent<NetGrabbableObjectController>().LocalPickupObject(transform));
            hasTarget = false;
            GetComponentInChildren<NetAIThrowTrigger>().setCanThrow(true);
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
        if (target && target.GetComponent<NetGrabbableObjectController>() && enlistDB)
        {
            NetDBGameManager.Instance.enlistDodgeball(target.gameObject);
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

    [ClientRpc]
    public void RpcSetActive(bool tf)
    {
        gameObject.SetActive(tf);
    }
}
