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
    private Transform target;
    private Vector3 desiredVelocity;

    public float turnSpeed = 1;

    public int speed = 1;


    // Start is called before the first frame update
    void Start()
    {
        target = switches[0];

        agent.destination = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            Vector3 lookPos;
            Quaternion targetRot;

            agent.destination = target.position;
            desiredVelocity = agent.desiredVelocity;

            agent.updatePosition = false;
            agent.updateRotation = false;

            lookPos = target.position - this.transform.position;
            lookPos.y = 0;
            targetRot = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

            movementController.GetController().Move(desiredVelocity.normalized * speed * Time.deltaTime);

            agent.velocity = movementController.GetController().velocity;
        }
        else
        {
            start = GameObject.Find("Game Manager").GetComponent<PBInitializeLevel>().IsLevelLoaded();
        }
    }
}
