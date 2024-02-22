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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget == false) {
            AquireTarget();
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

    private void AquireTarget()
    {
        GameObject[] dodgeballs = GameObject.FindGameObjectsWithTag("Grabbable");
        target = dodgeballs[Random.Range(0, dodgeballs.Length)].transform;
        hasTarget = true;
    }
}
