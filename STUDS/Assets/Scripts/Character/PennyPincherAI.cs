using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PennyPincherAI : MonoBehaviour
{
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private NavMeshAgent agent;
    public List<Transform> availableSwitches;
    [SerializeField] private bool hasTarget = false, active = false;
    private bool gameStarted = false;
    [SerializeField] private Transform target, previousTarget;

    public float turnSpeed = 1;

    public int speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] set2 = GameObject.FindGameObjectsWithTag("Electronics");
        availableSwitches = new List<Transform>();
        int i = 0;
        foreach (GameObject item in set2)
        {
            availableSwitches.Add(item.transform);
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
            }
            else if (Mathf.Abs(target.position.x - transform.position.x) < 0.1f
              && Mathf.Abs(target.position.z - transform.position.z) < 0.1f
              && target.position.y - transform.position.y < 3)
            {
                movementController.isJumping = true;
            }

        }
        else if (active && !hasTarget)
        {
            active = false;
            previousTarget = target;
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
            yield return new WaitForSecondsRealtime(0.5f);
            int i = Random.Range(0, availableSwitches.Count);
            if (availableSwitches[i] != null && availableSwitches[i].GetComponent<VolumeTrigger>().isSwitchActive == true)
            {
                active = true;
                hasTarget = true;
                target = availableSwitches[i];
                break;
            } else if (availableSwitches[i] != null && availableSwitches[i].GetComponent<VolumeTrigger>().isSwitchActive == false)
            {
                availableSwitches.RemoveAt(i);
                Debug.Log("switch in availableSwitches was not actually available");
            }
            else if (!availableSwitches[i].GetComponent<VolumeTrigger>())
            {
                Debug.LogError("Could not find Volume Trigger for: " + availableSwitches[i].name);
            }

        }

    }

    public void CheckUpdateTarget(GameObject flippedSwitch, bool isSwitchActive)
    {
        if (isSwitchActive)
            availableSwitches.Remove(flippedSwitch.transform);
        else
            availableSwitches.Add(flippedSwitch.transform);

        if (flippedSwitch.transform == target)
        {
            active = false;
            StartCoroutine("FindNewTarget");
        }
    }
}
