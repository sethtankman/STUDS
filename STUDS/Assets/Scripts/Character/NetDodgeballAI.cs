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

    private IEnumerator Start()
    {
        // We disable the character controller so it doesn't override the navmesh agent.
        GetComponent<CharacterController>().enabled = false;
        agent.autoTraverseOffMeshLink = false;
        if(isServer)
        {
            Invoke(nameof(StartTick), 10.0f);
            while (true)
            {
                if (agent.isOnOffMeshLink)
                {
                    yield return StartCoroutine(Parabola(2.0f, 0.8f));
                    agent.CompleteOffMeshLink();
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// Copied from https://github.com/Unity-Technologies/NavMeshComponents/blob/master/Assets/Examples/Scripts/AgentLinkMover.cs
    /// </summary>
    /// <param name="height"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator Parabola(float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            animator.SetTrigger("Jump");
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }

    private void StartTick()
    {
        StartCoroutine(AITick());
    }

    private IEnumerator AITick()
    {
        while (gameObject)
        {
            patience--;
            loiter--;
            if (loiter > 0)
                continue;
            if (!hasTarget)
            { // Agent finds target
                if (!GetComponent<NetworkCharacterMovementController>().GetHasGrabbed())
                {
                    AcquireTargetDodgeball();
                }
                else
                {
                    AcquireTargetPlayer();
                }
            }
            else if (patience < 0) // Agent runs out of patience trying to get target.
            {
                hasTarget = false;
                coroutineOn = false;
            }
            else if (agent.hasPath) // Agent pursues target
            {
                Vector3 lookPos;
                Quaternion targetRot;


                lookPos = agent.desiredVelocity;
                lookPos.y = 0;
                targetRot = Quaternion.LookRotation(lookPos);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            }
            else if (!agent.pathPending)
            {
                Loiter(true);
            }
            yield return new WaitForSeconds(0.5f);
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

        while (coroutineOn && target && Vector3.SqrMagnitude(transform.position - target.position) > 0.1f)
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
            int random = Random.Range(0, dodgeballs.Count);
            if (dodgeballs[random]) 
                target = dodgeballs[random].transform;
            else { return; }
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
