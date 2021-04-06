using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PennyPincherAI : MonoBehaviour
{
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private NavMeshAgent agent;
    public Transform[] switches;
    private bool active = false;
    private bool hasTarget = false;
    private bool gameStarted = false;
    [SerializeField] private Transform target;

    public float turnSpeed = 1;

    public int speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] set2 = GameObject.FindGameObjectsWithTag("Electronics");
        switches = new Transform[set2.Length];
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
        if (active && hasTarget)
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

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 1.5f)
            {
                hasTarget = false;
                target.GetComponent<VolumeTrigger>().FlipSwitch();
            } else if(Mathf.Abs(target.position.x - transform.position.x) < 0.1f 
                && Mathf.Abs(target.position.z - transform.position.z) < 0.1f 
                && target.position.y - transform.position.y < 3)
            {
                movementController.isJumping = true;
            }

        } else if (active && !hasTarget)
        {
            active = false;
            StartCoroutine("FindNewTarget");
        }
        else if (!gameStarted)
        {
            active = GameObject.Find("Game Manager").GetComponent<PBInitializeLevel>().IsLevelLoaded();
            gameStarted = active;
        }
    }

    public IEnumerator FindNewTarget()
    {
        movementController.isJumping = false;
        while (hasTarget == false)
        {
            yield return new WaitForSecondsRealtime(1);
            for (int i = 0; i < switches.Length; i++)
            {
                if (switches[i] != null && switches[i].GetComponent<VolumeTrigger>().isSwitchActive == true)
                {
                    active = true;
                    hasTarget = true;
                    target = switches[i];
                    break;
                }
            }
        }

    }

    public void CheckUpdateTarget()
    {

    }
}
