using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NetKidTimeout : NetworkBehaviour
{
    public double timeoutTimer;
    [SerializeField] private double currTime;
    private bool isTimeout;
    GameObject mini;

    [SerializeField] GameObject[] timeoutPos;
    [SerializeField] GameObject[] backInPos;
    public bool inPowerBill = false;
    int currIdx;
    
    /// <summary>
    /// Called at the beginning of PowerBill to enable timeout capability and set timeout positions
    /// </summary>
    public void Init()
    {
        inPowerBill = true;
        timeoutPos = NetPWRBill_Manager.timeoutPos;
        backInPos = NetPWRBill_Manager.backInPos;
    }

    private void Start()
    {
        if (GetComponent<NetworkCharacterMovementController>().isAI)
            Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (inPowerBill && isTimeout)
        {
            if (currTime > timeoutTimer)
            {
                transform.position = backInPos[currIdx].transform.position;
                // We need to reset the navigation agent when we teleport it as well.
                if (currTime > (timeoutTimer + 0.1))
                {
                    currTime = 0;
                    if (mini.GetComponent<CharacterMovementController>())
                    {
                        mini.GetComponent<CharacterMovementController>().CanMove = true;
                        if (mini.GetComponent<CharacterMovementController>().isAI)
                        {
                            mini.GetComponent<NavMeshAgent>().Warp(backInPos[currIdx].transform.position);
                            mini.GetComponent<PennyPincherAI>().CanMove = true;
                        }
                    }
                    else
                    {
                        mini.GetComponent<NetworkCharacterMovementController>().CanMove = true;
                        if (mini.GetComponent<NetworkCharacterMovementController>().isAI)
                        {
                            mini.GetComponent<NavMeshAgent>().Warp(backInPos[currIdx].transform.position);
                            mini.GetComponent<NetPennyPincherAI>().CanMove = true;
                        }
                    }
                    mini = null;
                    isTimeout = false;
                }
                currTime += Time.deltaTime;
            }
            else
            {
                currTime += Time.deltaTime;
                transform.position = timeoutPos[currIdx].transform.position;
            }
        }
    }

    [ClientRpc]
    public void RpcTimeout()
    {
        if (GetComponent<NetworkCharacterMovementController>().isAI || isLocalPlayer)
            Timeout(gameObject);
    }

    public void Timeout(GameObject mini)
    {
        int randomidx = Random.Range(0, 3);
        if (randomidx > 2)
        {
            randomidx = 2;
        }
        currIdx = randomidx;
        GameObject g = timeoutPos[currIdx];
        Vector3 newPos = g.transform.position;
        mini.transform.position = newPos;
        if (mini.GetComponent<NetworkCharacterMovementController>())
        {
            mini.GetComponent<NetworkCharacterMovementController>().CanMove = false;
            if (mini.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                mini.GetComponent<NetPennyPincherAI>().CanMove = false;
            }
        }
        this.mini = mini;
        isTimeout = true;
    }
}
