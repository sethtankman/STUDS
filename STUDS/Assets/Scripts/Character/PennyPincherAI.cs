using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PennyPincherAI : MonoBehaviour
{
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private NavMeshAgent agent;
    public Transform[] switches;
    private bool start = false;
    private bool hasTarget = false;
    [SerializeField] private Transform target;

    public float turnSpeed = 1;

    public int speed = 1;

    public int currentTargetIndex = -1;


    // Start is called before the first frame update
    void Start()
    {
        var set2 = GameObject.FindGameObjectsWithTag("Electronics");
        int i = 0;
        foreach (GameObject item in set2)
        {
            switches[i] = item.transform;
            i++;
        }
        target = null;
    }


    // Update is called once per frame
    void Update()
    {
        if (start && hasTarget)
        {
            Vector3 lookPos;
            Quaternion targetRot;

            agent.SetDestination(target.position);

            //agent.updatePosition = false;
            //agent.updateRotation = false;

            lookPos = agent.desiredVelocity;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            movementController.GetController().Move(agent.desiredVelocity.normalized * speed * Time.deltaTime);

            agent.velocity = movementController.GetController().velocity;

            if(Vector3.Distance(transform.position, target.position) < 1.5f)
            {
                hasTarget = false;
            }
        } else if (start && !hasTarget)
        {
            currentTargetIndex++;
            if (currentTargetIndex > switches.Length)
            {
                movementController.isJumping = true;
            }
            else
            {
                while (hasTarget == false)
                {
                    if (switches[currentTargetIndex] != null)
                    {
                        hasTarget = true;
                        target = switches[currentTargetIndex];
                    }
                    else
                    {
                        currentTargetIndex++;
                    }
                }
            }
        }
        else
        {
            start = GameObject.Find("Game Manager").GetComponent<PBInitializeLevel>().IsLevelLoaded();
        }
    }

    public void CheckUpdateTarget()
    {

    }
}
