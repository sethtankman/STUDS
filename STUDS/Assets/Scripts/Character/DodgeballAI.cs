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

    [SerializeField] private bool hasTarget = false, isHoldingSomething = false;

    private float turnSpeed = 1;
    private int speed = 1;

    // Update is called once per frame
    void Update()
    {
        if (!hasTarget) { 
            if (!isHoldingSomething) {
                AquireTarget("Grabbable");
            } else
            {
                AquireTarget("Player");
            }
        } else 
        {
            Vector3 lookPos;
            Quaternion targetRot;

            agent.SetDestination(target.position);

            lookPos = agent.desiredVelocity;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            GetComponent<CharacterMovementController>().Move(agent.desiredVelocity.normalized * speed);
        } 
    }

    private void AquireTarget(string tag)
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
        target = candidates[Random.Range(0, candidates.Length)].transform;
        hasTarget = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable") && !isHoldingSomething)
        {
            GetComponent<CharacterMovementController>().SetGrabbedObject(other.gameObject);
            other.GetComponent<GrabbableObjectController>().PickupObject("AI");
            isHoldingSomething = true;
            hasTarget = false;
        }
    }
}
