using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the AI for dodgeball
/// </summary>
public class DodgeballAI : MonoBehaviour
{
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    [SerializeField] private bool hasTarget = false;

    private float turnSpeed = 1;
    private int speed = 1;
    [SerializeField] private int loiter = 540, patience, maxPatience;


    private void Start()
    {
        // We disable the character controller so it doesn't override the navmesh agent.
        GetComponent<CharacterController>().enabled = false;
    }

    private void FixedUpdate()
    {
        loiter--;
        patience--;
    }

    // Update is called once per frame
    void Update()
    {
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
        } else
        {
            Vector3 lookPos;
            Quaternion targetRot;

            agent.SetDestination(target.position);

            lookPos = agent.desiredVelocity;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            //GetComponent<CharacterMovementController>().Move(agent.desiredVelocity.normalized * speed);
            //agent.Move(agent.desiredVelocity.normalized * speed);
        } 
    }

    private void AcquireTargetPlayer()
    {
        GameObject[] tagMatch = GameObject.FindGameObjectsWithTag("Player");

        bool foundMatch = false;
        for(int i = 0; i < tagMatch.Length; i++)
        {
            if (tagMatch[i].name == name)
            {
                foundMatch = true;
            }
            if(foundMatch && i+1 < tagMatch.Length)
            {
                tagMatch[i] = tagMatch[i + 1];
            }
        }
        GameObject[] candidates = new GameObject[tagMatch.Length-1];
        System.Array.Copy(tagMatch, candidates, tagMatch.Length - 1);
        target = candidates[Random.Range(0, candidates.Length)].transform;
        hasTarget = true;
        patience = maxPatience;
    }

    private void AcquireTargetDodgeball()
    {
        List<GameObject> dodgeballs = DBGameManager.Instance.GetAvailableDodgeballs();
        target = dodgeballs[Random.Range(0, dodgeballs.Count)].transform;
        hasTarget = true;
        patience = maxPatience;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GetComponent<CharacterMovementController>().GetHasGrabbed() && other.CompareTag("Grabbable")
            && other.GetComponent<GrabbableObjectController>().getCanPickup()
            && other.name.Equals(target.name))
        {
            GetComponent<CharacterMovementController>().SetGrabbedObject(other.gameObject);
            other.GetComponent<GrabbableObjectController>().PickupObject(GetComponent<CharacterMovementController>().GetColorName());
            hasTarget = false;
            GetComponentInChildren<AIThrowTrigger>().setCanThrow(true);
        }
    }

    public void Loiter()
    {
        loiter = 60;
        hasTarget = false;
    }
}
