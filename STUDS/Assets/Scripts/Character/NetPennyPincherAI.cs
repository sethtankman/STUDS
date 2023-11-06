using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetPennyPincherAI : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterMovementController movementController;
    [SerializeField] private NavMeshAgent agent;
    public List<Transform> availableSwitches;
    [SerializeField] private bool hasTarget = false, active = false;
    private bool gameStarted = false;
    [SerializeField] private Transform target;

    public float turnSpeed = 1;

    public int speed = 1;

    public bool CanMove;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            GameObject[] set2 = GameObject.FindGameObjectsWithTag("Electronics");
            availableSwitches = new List<Transform>();
            for (int i = 0; i < set2.Length; i++)
            {
                availableSwitches.Add(set2[i].transform);
            }
        }
        target = null;
        CanMove = true;
        FindObjectOfType<NetGameManager>().AddPlayer(gameObject);
    }




    // Update is called once per frame
    void Update()
    {
        if (isServer && active && hasTarget && CanMove)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 1.9f)
            {
                hasTarget = false;
                target.GetComponent<NetVolumeTrigger>().FlipSwitch();
            } /*
            else if (Mathf.Abs(target.position.x - transform.position.x) < 0.1f
              && Mathf.Abs(target.position.z - transform.position.z) < 0.1f
              && target.position.y - transform.position.y < 3)
            {
                Debug.Log("Jump!");
                movementController.isJumping = true;
            } */
            else
            {
                Vector3 lookPos;
                Quaternion targetRot;

                agent.SetDestination(target.position);

                // agent.updatePosition = false;
                // agent.updateRotation = false;

                lookPos = agent.desiredVelocity;
                lookPos.y = 0;
                targetRot = Quaternion.LookRotation(lookPos);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

                // These are the two lines I commented out and replaced with the new line below to get animations on the move
                // movementController.GetController().Move(agent.desiredVelocity.normalized * speed * Time.deltaTime);
                // agent.velocity = movementController.GetController().velocity;
                GetComponent<NetworkCharacterMovementController>().Move(agent.desiredVelocity.normalized * speed);
            }
        }
        else if (isServer && active && !hasTarget)
        {
            // Debug.Log("Active set to false in Update");
            active = false;
            StartCoroutine("FindNewTarget");
        }
        else if (!gameStarted && GameObject.Find("Game Manager"))
        {
            if (GameObject.Find("Game Manager").GetComponent<NetPBInitLvl>())
            {
                active = GameObject.Find("Game Manager").GetComponent<NetPBInitLvl>().IsLevelLoaded();
            }
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
            Transform selected = availableSwitches[i];
            if (selected != null && availableSwitches.Contains(selected)
                && selected.GetComponent<NetVolumeTrigger>().isSwitchActive == true)
            {
                active = true;
                hasTarget = true;
                target = selected;
                selected.GetComponent<NetVolumeTrigger>().NotifyInteractionOfSwitchTargetted(true, this.gameObject);
                break;
            }
            else if (selected != null && availableSwitches.Contains(selected)
              && selected.GetComponent<NetVolumeTrigger>().isSwitchActive == false)
            {
                availableSwitches.Remove(selected);
            }
            else if(availableSwitches[i] == null)
            {
                break;
            }
            else if (!availableSwitches[i].GetComponent<NetVolumeTrigger>())
            {
                Debug.LogError("Could not find Volume Trigger for: " + availableSwitches[i].name);
            }
            else
            {
                Debug.LogError("Control reached here somehow");
            }

        }
        yield return new WaitForSecondsRealtime(0.5f);

    }

    public void CheckUpdateTarget(GameObject flippedSwitch, bool isNowOn)
    {
        if (isNowOn)
            availableSwitches.Remove(flippedSwitch.transform);
        else
            availableSwitches.Add(flippedSwitch.transform);

        if (flippedSwitch.transform == target)
        {
            hasTarget = false;
            active = false;
            StartCoroutine("FindNewTarget");
        }
    }

    public void CheckUpdateTarget(GameObject flippedSwitch, bool isNowOn, GameObject immuneOne)
    {
        if (this.gameObject == immuneOne)
            return;
        if (isNowOn)
            availableSwitches.Remove(flippedSwitch.transform);
        else
            availableSwitches.Add(flippedSwitch.transform);

        if (flippedSwitch.transform == target)
        {
            active = false;
            StartCoroutine("FindNewTarget");
        }
    }

    public void SetActive(bool tf)
    {
        active = tf;
    }
}
